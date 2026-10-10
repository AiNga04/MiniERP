SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF DB_ID(N'TestingSystem') IS NULL
BEGIN
    CREATE DATABASE TestingSystem;
END;
GO
USE TestingSystem;
GO

CREATE TABLE dbo.Department (
    DepartmentID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Department PRIMARY KEY,
    DepartmentName NVARCHAR(30) NOT NULL,
    DeletedAt DATETIME2(3) NULL
);
CREATE UNIQUE INDEX UX_Department_Name_Active ON dbo.Department(DepartmentName) WHERE DeletedAt IS NULL;
GO
CREATE TABLE dbo.Position (
    PositionID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Position PRIMARY KEY,
    PositionName NVARCHAR(30) NOT NULL,
    DeletedAt DATETIME2(3) NULL
);
CREATE UNIQUE INDEX UX_Position_Name_Active ON dbo.Position(PositionName) WHERE DeletedAt IS NULL;
GO
CREATE TABLE dbo.[Account] (
    AccountID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Account PRIMARY KEY,
    Email NVARCHAR(254) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    DepartmentID INT NOT NULL,
    PositionID INT NOT NULL,
    CreateDate DATETIME2(3) NOT NULL CONSTRAINT DF_Account_CreateDate DEFAULT SYSUTCDATETIME(),
    DeletedAt DATETIME2(3) NULL,
    CONSTRAINT FK_Account_Department FOREIGN KEY (DepartmentID) REFERENCES dbo.Department(DepartmentID),
    CONSTRAINT FK_Account_Position FOREIGN KEY (PositionID) REFERENCES dbo.Position(PositionID)
);
CREATE UNIQUE INDEX UX_Account_Email_Active ON dbo.[Account](Email) WHERE DeletedAt IS NULL;
CREATE UNIQUE INDEX UX_Account_Username_Active ON dbo.[Account](Username) WHERE DeletedAt IS NULL;
CREATE INDEX IX_Account_DepartmentID ON dbo.[Account](DepartmentID);
CREATE INDEX IX_Account_PositionID ON dbo.[Account](PositionID);
GO
CREATE TABLE dbo.[Group] (
    GroupID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Group PRIMARY KEY,
    GroupName NVARCHAR(50) NOT NULL,
    CreatorID INT NULL,
    CreateDate DATETIME2(3) NOT NULL CONSTRAINT DF_Group_CreateDate DEFAULT SYSUTCDATETIME(),
    DeletedAt DATETIME2(3) NULL,
    CONSTRAINT FK_Group_Creator FOREIGN KEY (CreatorID) REFERENCES dbo.[Account](AccountID)
);
CREATE UNIQUE INDEX UX_Group_Name_Active ON dbo.[Group](GroupName) WHERE DeletedAt IS NULL;
CREATE INDEX IX_Group_CreatorID ON dbo.[Group](CreatorID);
GO
CREATE TABLE dbo.GroupAccount (
    GroupAccountID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_GroupAccount PRIMARY KEY,
    GroupID INT NOT NULL,
    AccountID INT NOT NULL,
    JoinDate DATETIME2(3) NOT NULL CONSTRAINT DF_GroupAccount_JoinDate DEFAULT SYSUTCDATETIME(),
    DeletedAt DATETIME2(3) NULL,
    CONSTRAINT FK_GroupAccount_Group FOREIGN KEY (GroupID) REFERENCES dbo.[Group](GroupID),
    CONSTRAINT FK_GroupAccount_Account FOREIGN KEY (AccountID) REFERENCES dbo.[Account](AccountID)
);
CREATE UNIQUE INDEX UX_GroupAccount_ActivePair ON dbo.GroupAccount(GroupID, AccountID) WHERE DeletedAt IS NULL;
CREATE INDEX IX_GroupAccount_AccountID ON dbo.GroupAccount(AccountID);
GO
CREATE TABLE dbo.TypeQuestion (
    TypeID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TypeQuestion PRIMARY KEY,
    TypeName NVARCHAR(30) NOT NULL,
    DeletedAt DATETIME2(3) NULL
);
CREATE UNIQUE INDEX UX_TypeQuestion_Name_Active ON dbo.TypeQuestion(TypeName) WHERE DeletedAt IS NULL;
GO
CREATE TABLE dbo.CategoryQuestion (
    CategoryID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CategoryQuestion PRIMARY KEY,
    CategoryName NVARCHAR(50) NOT NULL,
    DeletedAt DATETIME2(3) NULL
);
CREATE UNIQUE INDEX UX_CategoryQuestion_Name_Active ON dbo.CategoryQuestion(CategoryName) WHERE DeletedAt IS NULL;
GO
CREATE TABLE dbo.Question (
    QuestionID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Question PRIMARY KEY,
    Content NVARCHAR(1000) NOT NULL,
    CategoryID INT NOT NULL,
    TypeID INT NOT NULL,
    CreatorID INT NOT NULL,
    CreateDate DATETIME2(3) NOT NULL CONSTRAINT DF_Question_CreateDate DEFAULT SYSUTCDATETIME(),
    DeletedAt DATETIME2(3) NULL,
    CONSTRAINT FK_Question_Category FOREIGN KEY (CategoryID) REFERENCES dbo.CategoryQuestion(CategoryID),
    CONSTRAINT FK_Question_Type FOREIGN KEY (TypeID) REFERENCES dbo.TypeQuestion(TypeID),
    CONSTRAINT FK_Question_Creator FOREIGN KEY (CreatorID) REFERENCES dbo.[Account](AccountID)
);
CREATE INDEX IX_Question_CategoryID ON dbo.Question(CategoryID);
CREATE INDEX IX_Question_TypeID ON dbo.Question(TypeID);
CREATE INDEX IX_Question_CreatorID ON dbo.Question(CreatorID);
GO
CREATE TABLE dbo.Answer (
    AnswerID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Answer PRIMARY KEY,
    Content NVARCHAR(1000) NOT NULL,
    QuestionID INT NOT NULL,
    IsCorrect BIT NOT NULL CONSTRAINT DF_Answer_IsCorrect DEFAULT (0),
    DeletedAt DATETIME2(3) NULL,
    CONSTRAINT FK_Answer_Question FOREIGN KEY (QuestionID) REFERENCES dbo.Question(QuestionID)
);
CREATE INDEX IX_Answer_QuestionID ON dbo.Answer(QuestionID);
GO
CREATE TABLE dbo.Exam (
    ExamID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Exam PRIMARY KEY,
    Code VARCHAR(10) NOT NULL,
    Title NVARCHAR(100) NOT NULL,
    CategoryID INT NOT NULL,
    Duration SMALLINT NOT NULL,
    CreatorID INT NOT NULL,
    CreateDate DATETIME2(3) NOT NULL CONSTRAINT DF_Exam_CreateDate DEFAULT SYSUTCDATETIME(),
    DeletedAt DATETIME2(3) NULL,
    CONSTRAINT CK_Exam_Duration CHECK (Duration > 0),
    CONSTRAINT FK_Exam_Category FOREIGN KEY (CategoryID) REFERENCES dbo.CategoryQuestion(CategoryID),
    CONSTRAINT FK_Exam_Creator FOREIGN KEY (CreatorID) REFERENCES dbo.[Account](AccountID)
);
CREATE UNIQUE INDEX UX_Exam_Code_Active ON dbo.Exam(Code) WHERE DeletedAt IS NULL;
CREATE INDEX IX_Exam_CategoryID ON dbo.Exam(CategoryID);
CREATE INDEX IX_Exam_CreatorID ON dbo.Exam(CreatorID);
GO
CREATE TABLE dbo.ExamQuestion (
    ExamQuestionID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExamQuestion PRIMARY KEY,
    ExamID INT NOT NULL,
    QuestionID INT NOT NULL,
    DeletedAt DATETIME2(3) NULL,
    CONSTRAINT FK_ExamQuestion_Exam FOREIGN KEY (ExamID) REFERENCES dbo.Exam(ExamID),
    CONSTRAINT FK_ExamQuestion_Question FOREIGN KEY (QuestionID) REFERENCES dbo.Question(QuestionID)
);
CREATE UNIQUE INDEX UX_ExamQuestion_ActivePair ON dbo.ExamQuestion(ExamID, QuestionID) WHERE DeletedAt IS NULL;
CREATE INDEX IX_ExamQuestion_QuestionID ON dbo.ExamQuestion(QuestionID);
GO
