param(
    [Parameter(Mandatory)] [string] $BaseUri,
    [Parameter(Mandatory)] [switch] $IsolatedDatabase
)

$ErrorActionPreference = 'Stop'
if (-not $IsolatedDatabase) { throw 'Run this test only against a disposable database.' }
# Fixture: active department 'CRUD existing', with a referencing row in another table.
$handler = [System.Net.Http.HttpClientHandler]::new()
$handler.AllowAutoRedirect = $false
$handler.CookieContainer = [System.Net.CookieContainer]::new()
$client = [System.Net.Http.HttpClient]::new($handler)
$client.BaseAddress = [Uri]($BaseUri.TrimEnd('/') + '/')

function Send-Request([string] $Path, [hashtable] $Fields = $null) {
    if ($null -eq $Fields) {
        $response = $client.GetAsync($Path).GetAwaiter().GetResult()
    } else {
        $values = [System.Collections.Generic.Dictionary[string,string]]::new()
        foreach ($key in $Fields.Keys) { $values.Add($key, [string]$Fields[$key]) }
        $content = [System.Net.Http.FormUrlEncodedContent]::new($values)
        $response = $client.PostAsync($Path, $content).GetAwaiter().GetResult()
        $content.Dispose()
    }
    $result = @{
        Status = [int]$response.StatusCode
        Html = [System.Net.WebUtility]::HtmlDecode($response.Content.ReadAsStringAsync().GetAwaiter().GetResult())
        Location = [string]$response.Headers.Location
    }
    $response.Dispose()
    return $result
}

function Assert-Status($Response, [int] $Expected) {
    if ($Response.Status -ne $Expected) { throw "Expected HTTP $Expected, got $($Response.Status)." }
}

function Get-Token([string] $Path) {
    $page = Send-Request $Path
    Assert-Status $page 200
    $match = [regex]::Match($page.Html, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"')
    if (-not $match.Success) { throw "Antiforgery token missing at $Path." }
    return $match.Groups[1].Value
}

try {
    Assert-Status (Send-Request '/') 200
    Assert-Status (Send-Request '/Departments') 200
    Assert-Status (Send-Request '/Departments/Create' @{ 'Input.DepartmentName' = 'NoToken' }) 400
    $token = Get-Token '/Departments/Create'
    foreach ($invalid in @('', '   ', ('x' * 31))) {
        $response = Send-Request '/Departments/Create' @{
            '__RequestVerificationToken' = $token; 'Input.DepartmentName' = $invalid
        }
        Assert-Status $response 200
        if ($response.Html -notmatch 'field-validation-error') { throw 'Invalid name was not rejected.' }
    }

    $name = 'CRUD ' + [guid]::NewGuid().ToString('N').Substring(0, 8)
    $response = Send-Request '/Departments/Create' @{
        '__RequestVerificationToken' = $token; 'Input.DepartmentName' = "  $name  "
        'DepartmentId' = '999999'; 'DeletedAt' = '2000-01-01'
    }
    Assert-Status $response 302
    $id = [regex]::Match($response.Location, '/Details/(\d+)').Groups[1].Value
    if (-not $id) { throw 'Created ID missing from redirect.' }
    $details = Send-Request "/Departments/Details/$id"
    Assert-Status $details 200
    if ($details.Html -notmatch [regex]::Escape($name)) { throw 'Created name not rendered.' }
    $duplicate = Send-Request '/Departments/Create' @{
        '__RequestVerificationToken' = $token; 'Input.DepartmentName' = $name
    }
    Assert-Status $duplicate 200
    if ($duplicate.Html -notmatch 'Tên phòng ban đã tồn tại') { throw 'Duplicate create was not rejected.' }

    $editToken = Get-Token "/Departments/Edit/$id"
    Assert-Status (Send-Request "/Departments/Edit/$id" @{ 'Input.DepartmentName' = 'NoToken' }) 400
    Assert-Status (Send-Request "/Departments/Edit/$id" @{
        '__RequestVerificationToken' = $editToken; 'Input.DepartmentName' = '   '
    }) 200
    $duplicate = Send-Request "/Departments/Edit/$id" @{
        '__RequestVerificationToken' = $editToken; 'Input.DepartmentName' = 'CRUD existing'
    }
    Assert-Status $duplicate 200
    if ($duplicate.Html -notmatch 'Tên phòng ban đã tồn tại') { throw 'Duplicate update was not rejected.' }
    $editedName = "Sửa $name"
    Assert-Status (Send-Request "/Departments/Edit/$id" @{
        '__RequestVerificationToken' = $editToken; 'Input.DepartmentName' = " $editedName "
    }) 302
    $details = Send-Request "/Departments/Details/$id"
    if ($details.Html -notmatch [regex]::Escape($editedName)) { throw 'Update was not persisted.' }
    # Saving an unchanged name must not conflict with the same row.
    Assert-Status (Send-Request "/Departments/Edit/$id" @{
        '__RequestVerificationToken' = $editToken; 'Input.DepartmentName' = $editedName
    }) 302

    foreach ($page in @('Details', 'Edit', 'Delete')) {
        Assert-Status (Send-Request "/Departments/$page/2147483647") 404
    }
    $deleteToken = Get-Token "/Departments/Delete/$id"
    Assert-Status (Send-Request "/Departments/Delete/$id" @{}) 400
    Assert-Status (Send-Request "/Departments/Details/$id") 200 # GET Delete did not write.
    Assert-Status (Send-Request "/Departments/Delete/$id" @{
        '__RequestVerificationToken' = $deleteToken
    }) 302
    foreach ($page in @('Details', 'Edit', 'Delete')) {
        Assert-Status (Send-Request "/Departments/$page/$id") 404
    }
    Assert-Status (Send-Request "/Departments/Edit/$id" @{
        '__RequestVerificationToken' = $editToken; 'Input.DepartmentName' = 'Stale edit'
    }) 404
    Assert-Status (Send-Request "/Departments/Delete/$id" @{
        '__RequestVerificationToken' = $deleteToken
    }) 404
    $list = Send-Request '/Departments'
    if ($list.Html -match [regex]::Escape($editedName)) { throw 'Deleted row still visible.' }
    # Filtered uniqueness permits a deleted name to be used again.
    Assert-Status (Send-Request '/Departments/Create' @{
        '__RequestVerificationToken' = $token; 'Input.DepartmentName' = $editedName
    }) 302
    # Concurrent requests must create only one active department with this name.
    $raceName = 'Race ' + [guid]::NewGuid().ToString('N').Substring(0, 8)
    $pending = @()
    $contents = @()
    for ($i = 0; $i -lt 6; $i++) {
        $values = [System.Collections.Generic.Dictionary[string,string]]::new()
        $values.Add('__RequestVerificationToken', $token)
        $values.Add('Input.DepartmentName', $raceName)
        $content = [System.Net.Http.FormUrlEncodedContent]::new($values)
        $contents += $content
        $pending += $client.PostAsync('/Departments/Create', $content)
    }
    $created = 0
    foreach ($task in $pending) {
        $response = $task.GetAwaiter().GetResult()
        if ([int]$response.StatusCode -eq 302) {
            $created++
        } elseif ([int]$response.StatusCode -eq 200) {
            $html = [System.Net.WebUtility]::HtmlDecode($response.Content.ReadAsStringAsync().GetAwaiter().GetResult())
            if ($html -notmatch 'Tên phòng ban đã tồn tại') { throw 'Concurrent duplicate did not return a field error.' }
        } else {
            throw "Concurrent create returned HTTP $([int]$response.StatusCode)."
        }
        $response.Dispose()
    }
    foreach ($content in $contents) { $content.Dispose() }
    if ($created -ne 1) { throw "Concurrent duplicates created $created rows instead of one." }
    # The fixture has a foreign-key reference; soft deletion must still succeed.
    $fixtureToken = Get-Token '/Departments/Delete/1'
    Assert-Status (Send-Request '/Departments/Delete/1' @{
        '__RequestVerificationToken' = $fixtureToken
    }) 302
    Write-Output 'PASS: CRUD, validation, duplicates, antiforgery, overposting, missing IDs, soft delete, stale edits, name reuse and referenced department deletion.'
} finally {
    $client.Dispose()
    $handler.Dispose()
}
