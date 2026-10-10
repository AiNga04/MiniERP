
USE [TestingSystem];
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

INSERT INTO dbo.Department(DepartmentName) VALUES
(N'Marketing'),(N'Sale'),(N'Bảo vệ'),(N'Nhân sự'),(N'Kỹ thuật'),(N'Tài chính'),
(N'Phó giám đốc'),(N'Giám đốc'),(N'Thư kí'),(N'No person'),(N'Bán hàng');
INSERT INTO dbo.Position(PositionName) VALUES (N'Dev'),(N'Test'),(N'Scrum Master'),(N'PM');
INSERT INTO dbo.[Account](Email,Username,FullName,DepartmentID,PositionID,CreateDate) VALUES
(N'Email1@gmail.com',N'Username1',N'Fullname1',5,1,'2020-03-05'),
(N'Email2@gmail.com',N'Username2',N'Fullname2',1,2,'2020-03-05'),
(N'Email3@gmail.com',N'Username3',N'Fullname3',2,2,'2020-03-07'),
(N'Email4@gmail.com',N'Username4',N'Fullname4',3,4,'2020-03-08'),
(N'Email5@gmail.com',N'Username5',N'Fullname5',4,4,'2020-03-10'),
(N'Email6@gmail.com',N'Username6',N'Fullname6',6,3,'2020-04-05'),
(N'Email7@gmail.com',N'Username7',N'Fullname7',2,2,SYSUTCDATETIME()),
(N'Email8@gmail.com',N'Username8',N'Fullname8',8,1,'2020-04-07'),
(N'Email9@gmail.com',N'Username9',N'Fullname9',2,2,'2020-04-07'),
(N'Email10@gmail.com',N'Username10',N'Fullname10',10,1,'2020-04-09'),
(N'Email11@gmail.com',N'Username11',N'Fullname11',10,1,SYSUTCDATETIME()),
(N'Email12@gmail.com',N'Username12',N'Fullname12',10,1,SYSUTCDATETIME());
INSERT INTO dbo.[Group](GroupName,CreatorID,CreateDate) VALUES
(N'Testing System',5,'2019-03-05'),(N'Development',1,'2020-03-07'),
(N'VTI Sale 01',2,'2020-03-09'),(N'VTI Sale 02',3,'2020-03-10'),
(N'VTI Sale 03',4,'2020-03-28'),(N'VTI Creator',6,'2020-04-06'),
(N'VTI Marketing 01',7,'2020-04-07'),(N'Management',8,'2020-04-08'),
(N'Chat with love',9,'2020-04-09'),(N'Vi Ti Ai',10,'2020-04-10');
INSERT INTO dbo.GroupAccount(GroupID,AccountID,JoinDate) VALUES
(1,1,'2019-03-05'),(1,2,'2020-03-07'),(3,3,'2020-03-09'),
(3,4,'2020-03-10'),(5,5,'2020-03-28'),(1,3,'2020-04-06'),
(1,7,'2020-04-07'),(8,3,'2020-04-08'),(1,9,'2020-04-09'),(10,10,'2020-04-10');
INSERT INTO dbo.TypeQuestion(TypeName) VALUES (N'Essay'),(N'Multiple-Choice');
INSERT INTO dbo.CategoryQuestion(CategoryName) VALUES
(N'Java'),(N'ASP.NET'),(N'ADO.NET'),(N'SQL'),(N'Postman'),
(N'Ruby'),(N'Python'),(N'C++'),(N'C Sharp'),(N'PHP');
INSERT INTO dbo.Question(Content,CategoryID,TypeID,CreatorID,CreateDate) VALUES
(N'Câu hỏi về Java',1,1,2,'2020-04-05'),
(N'Câu Hỏi về PHP',10,2,2,'2020-04-05'),
(N'Hỏi về C#',9,2,3,'2020-04-06'),
(N'Hỏi về Ruby',6,1,4,'2020-04-06'),
(N'Hỏi về Postman',5,1,5,'2020-04-06'),
(N'Hỏi về ADO.NET',3,2,6,'2020-04-06'),
(N'Hỏi về ASP.NET',2,1,7,'2020-04-06'),
(N'Hỏi về C++',8,1,8,'2020-04-07'),
(N'Hỏi về SQL',4,2,9,'2020-04-07'),
(N'Hỏi về Python',7,1,10,'2020-04-07');
INSERT INTO dbo.Answer(Content,QuestionID,IsCorrect) VALUES
(N'Trả lời 01',1,0),(N'Trả lời 02',1,1),(N'Trả lời 03',1,0),
(N'Trả lời 04',1,1),(N'Trả lời 05',2,1),(N'Trả lời 06',3,1),
(N'Trả lời 07',4,0),(N'Trả lời 08',8,0),(N'Trả lời 09',9,1),
(N'Trả lời 10',10,1);
INSERT INTO dbo.Exam(Code,Title,CategoryID,Duration,CreatorID,CreateDate) VALUES
('VTIQ001',N'Đề thi C#',1,60,5,'2019-04-05'),
('VTIQ002',N'Đề thi PHP',10,60,2,'2019-04-05'),
('VTIQ003',N'Đề thi C++',9,120,2,'2019-04-07'),
('VTIQ004',N'Đề thi Java',6,60,3,'2020-04-08'),
('VTIQ005',N'Đề thi Ruby',5,120,4,'2020-04-10'),
('VTIQ006',N'Đề thi Postman',3,60,6,'2020-04-05'),
('VTIQ007',N'Đề thi SQL',2,60,7,'2020-04-05'),
('VTIQ008',N'Đề thi Python',8,60,8,'2020-04-07'),
('VTIQ009',N'Đề thi ADO.NET',4,90,9,'2020-04-07'),
('VTIQ010',N'Đề thi ASP.NET',7,90,10,'2020-04-08');
INSERT INTO dbo.ExamQuestion(ExamID,QuestionID) VALUES
(1,5),(2,10),(3,4),(4,3),(5,7),(6,10),(7,2),(8,10),(9,9),(10,8);
GO
