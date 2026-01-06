IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF SCHEMA_ID(N'Identity') IS NULL EXEC(N'CREATE SCHEMA [Identity];');
GO

IF SCHEMA_ID(N'Academics') IS NULL EXEC(N'CREATE SCHEMA [Academics];');
GO

IF SCHEMA_ID(N'Files') IS NULL EXEC(N'CREATE SCHEMA [Files];');
GO

IF SCHEMA_ID(N'Notifications') IS NULL EXEC(N'CREATE SCHEMA [Notifications];');
GO

CREATE TABLE [Identity].[AppRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AppRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Identity].[AppUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(256) NOT NULL,
    [ContactInfo] nvarchar(512) NOT NULL,
    [ProfileImagePath] nvarchar(300) NOT NULL,
    [Age] int NOT NULL,
    [BirthDay] date NOT NULL,
    [Gender] nvarchar(20) NOT NULL,
    [AccountStatus] nvarchar(50) NOT NULL DEFAULT N'Pending',
    [EmailConfirmed] bit NOT NULL,
    [ConfirmationNumber] nvarchar(max) NULL,
    [EmailConfirmationToken] nvarchar(max) NULL,
    [ConfirmationTokenExpiry] datetime2 NULL,
    [PendingRole] nvarchar(max) NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AppUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Academics].[Curricula] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(150) NOT NULL,
    [Code] nvarchar(20) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [Country] nvarchar(max) NULL,
    [Language] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [SortOrder] int NOT NULL,
    [TotalGrades] int NULL,
    [AcademicSystem] nvarchar(max) NULL,
    [DefaultTemplatePath] nvarchar(max) NULL,
    [CertificateHeader] nvarchar(max) NULL,
    [CertificateFooter] nvarchar(max) NULL,
    [GradingScale] nvarchar(max) NULL,
    [PassingGrade] float NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Curricula] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Academics].[StudentExamResults] (
    [Id] nvarchar(450) NOT NULL,
    [StudentId] nvarchar(100) NOT NULL,
    [ExamId] nvarchar(100) NOT NULL,
    [TotalMark] decimal(10,2) NOT NULL,
    [FullMark] decimal(10,2) NOT NULL,
    [MinMark] decimal(10,2) NOT NULL,
    [Percentage] decimal(5,2) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [StudentName] nvarchar(200) NOT NULL,
    [SubjectName] nvarchar(200) NOT NULL,
    [TeacherName] nvarchar(200) NOT NULL,
    [ExamName] nvarchar(200) NOT NULL,
    [Date] datetime2 NOT NULL,
    CONSTRAINT [PK_StudentExamResults] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Identity].[RoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_RoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RoleClaims_AppRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Identity].[AppRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Identity].[Admins] (
    [Id] nvarchar(36) NOT NULL,
    [Type] nvarchar(100) NOT NULL,
    [FullName] nvarchar(255) NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [ContactInfo] nvarchar(500) NULL,
    [AccountStatus] nvarchar(50) NOT NULL DEFAULT N'Pending',
    [AppUserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Admins] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Admins_AppUsers_AppUserId] FOREIGN KEY ([AppUserId]) REFERENCES [Identity].[AppUsers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Identity].[AppUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AppUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AppUserRoles_AppRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Identity].[AppRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AppUserRoles_AppUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[AppUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Files].[FileRecords] (
    [Id] nvarchar(450) NOT NULL,
    [FileName] nvarchar(255) NOT NULL,
    [RelativePath] nvarchar(500) NOT NULL,
    [ControllerName] nvarchar(100) NOT NULL,
    [FileType] nvarchar(10) NOT NULL,
    [FileSize] bigint NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_FileRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FileRecords_AppUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[AppUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Notifications].[Notifications] (
    [Id] nvarchar(450) NOT NULL,
    [Title] nvarchar(255) NOT NULL,
    [Message] nvarchar(1000) NOT NULL,
    [Url] nvarchar(500) NOT NULL,
    [NotificationType] nvarchar(100) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatorUserId] nvarchar(450) NULL,
    [Role] nvarchar(50) NULL,
    [IsBroadcast] bit NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_AppUsers_CreatorUserId] FOREIGN KEY ([CreatorUserId]) REFERENCES [Identity].[AppUsers] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Academics].[Parents] (
    [Id] nvarchar(36) NOT NULL,
    [FullName] nvarchar(255) NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [ContactInfo] nvarchar(500) NULL,
    [AccountStatus] nvarchar(50) NOT NULL DEFAULT N'Pending',
    [AppUserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Parents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Parents_AppUsers_AppUserId] FOREIGN KEY ([AppUserId]) REFERENCES [Identity].[AppUsers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Identity].[RefreshTokens] (
    [Id] nvarchar(36) NOT NULL,
    [Token] nvarchar(255) NOT NULL,
    [JwtId] nvarchar(255) NOT NULL,
    [IsUsed] bit NOT NULL,
    [IsRevoked] bit NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [AppUserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RefreshTokens_AppUsers_AppUserId] FOREIGN KEY ([AppUserId]) REFERENCES [Identity].[AppUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Academics].[Teachers] (
    [Id] nvarchar(36) NOT NULL,
    [FullName] nvarchar(255) NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [ContactInfo] nvarchar(500) NULL,
    [AccountStatus] nvarchar(50) NOT NULL DEFAULT N'Pending',
    [AppUserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Teachers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Teachers_AppUsers_AppUserId] FOREIGN KEY ([AppUserId]) REFERENCES [Identity].[AppUsers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Identity].[UserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserClaims_AppUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[AppUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Identity].[UserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_UserLogins_AppUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[AppUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Identity].[UserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_UserTokens_AppUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[AppUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Academics].[Grades] (
    [Id] nvarchar(450) NOT NULL,
    [GradeName] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [CurriculumId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Grades] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Grades_Curricula_CurriculumId] FOREIGN KEY ([CurriculumId]) REFERENCES [Academics].[Curricula] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Notifications].[UserNotifications] (
    [Id] nvarchar(450) NOT NULL,
    [NotificationId] nvarchar(450) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [IsRead] bit NOT NULL,
    [IsDelivered] bit NOT NULL,
    [DeliveredAt] datetime2 NULL,
    [ReadAt] datetime2 NULL,
    CONSTRAINT [PK_UserNotifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserNotifications_AppUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[AppUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserNotifications_Notifications_NotificationId] FOREIGN KEY ([NotificationId]) REFERENCES [Notifications].[Notifications] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Academics].[Questions] (
    [Id] nvarchar(450) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [CorrectTextAnswer] nvarchar(max) NULL,
    [Degree] decimal(5,2) NOT NULL,
    [Type] int NOT NULL,
    [TrueAndFalses] bit NULL,
    [TeacherId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_Questions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Questions_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Academics].[Teachers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Academics].[TeacherCurricula] (
    [SpecializedCurriculaId] nvarchar(450) NOT NULL,
    [TeachersId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_TeacherCurricula] PRIMARY KEY ([SpecializedCurriculaId], [TeachersId]),
    CONSTRAINT [FK_TeacherCurricula_Curricula_SpecializedCurriculaId] FOREIGN KEY ([SpecializedCurriculaId]) REFERENCES [Academics].[Curricula] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeacherCurricula_Teachers_TeachersId] FOREIGN KEY ([TeachersId]) REFERENCES [Academics].[Teachers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Academics].[Classes] (
    [Id] nvarchar(450) NOT NULL,
    [ClassName] nvarchar(100) NOT NULL,
    [GradeId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Classes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Classes_Grades_GradeId] FOREIGN KEY ([GradeId]) REFERENCES [Academics].[Grades] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Academics].[Subjects] (
    [Id] nvarchar(450) NOT NULL,
    [SubjectName] nvarchar(150) NOT NULL,
    [GradeId] nvarchar(450) NOT NULL,
    [CreditHours] float NOT NULL DEFAULT 3.0E0,
    CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Subjects_Grades_GradeId] FOREIGN KEY ([GradeId]) REFERENCES [Academics].[Grades] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Academics].[Choices] (
    [Id] nvarchar(450) NOT NULL,
    [Text] nvarchar(255) NOT NULL,
    [IsCorrect] bit NOT NULL,
    [QuestionId] nvarchar(450) NULL,
    CONSTRAINT [PK_Choices] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Choices_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Academics].[Questions] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Academics].[ClassAssets] (
    [Id] nvarchar(36) NOT NULL,
    [AssetTypeName] nvarchar(max) NOT NULL,
    [AssetsPath] nvarchar(500) NULL,
    [ClassId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_ClassAssets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ClassAssets_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Academics].[Classes] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Academics].[Students] (
    [Id] nvarchar(36) NOT NULL,
    [ClassYear] nvarchar(50) NOT NULL,
    [Age] int NOT NULL,
    [Nationality] nvarchar(max) NOT NULL,
    [IqamaNumber] nvarchar(max) NOT NULL,
    [PassportNumber] nvarchar(max) NOT NULL,
    [ClassId] nvarchar(450) NULL,
    [CurriculumId] nvarchar(450) NULL,
    [FullName] nvarchar(255) NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [ContactInfo] nvarchar(500) NULL,
    [AccountStatus] nvarchar(50) NOT NULL DEFAULT N'Pending',
    [AppUserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Students] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Students_AppUsers_AppUserId] FOREIGN KEY ([AppUserId]) REFERENCES [Identity].[AppUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Students_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Academics].[Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Students_Curricula_CurriculumId] FOREIGN KEY ([CurriculumId]) REFERENCES [Academics].[Curricula] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Academics].[TeacherClasses] (
    [Id] nvarchar(450) NOT NULL,
    [TeacherId] nvarchar(36) NOT NULL,
    [ClassId] nvarchar(450) NOT NULL,
    [AssignedAt] datetime2 NOT NULL,
    [Notes] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_TeacherClasses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeacherClasses_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Academics].[Classes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeacherClasses_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Academics].[Teachers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Academics].[ClassAppointments] (
    [Id] nvarchar(36) NOT NULL,
    [StartTime] datetime2 NOT NULL,
    [EndTime] datetime2 NOT NULL,
    [Link] nvarchar(max) NOT NULL,
    [Status] nvarchar(50) NOT NULL DEFAULT N'Upcoming',
    [ClassId] nvarchar(450) NOT NULL,
    [TeacherId] nvarchar(36) NOT NULL,
    [SubjectId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_ClassAppointments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ClassAppointments_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Academics].[Classes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ClassAppointments_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Academics].[Subjects] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ClassAppointments_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Academics].[Teachers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Academics].[DegreeComponentTypes] (
    [Id] nvarchar(450) NOT NULL,
    [SubjectId] nvarchar(450) NOT NULL,
    [ComponentName] nvarchar(100) NOT NULL,
    [Order] int NOT NULL DEFAULT 1,
    [MaxScore] float NOT NULL DEFAULT 0.0E0,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_DegreeComponentTypes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DegreeComponentTypes_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Academics].[Subjects] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Academics].[Exams] (
    [Id] nvarchar(450) NOT NULL,
    [ExamName] nvarchar(max) NOT NULL,
    [SubjectId] nvarchar(450) NOT NULL,
    [ClassId] nvarchar(450) NOT NULL,
    [TeacherId] nvarchar(36) NOT NULL,
    [Start] datetime2 NOT NULL,
    [End] datetime2 NOT NULL,
    [Status] nvarchar(20) NOT NULL DEFAULT N'Upcoming',
    [MinMark] decimal(5,2) NOT NULL,
    [FullMark] decimal(5,2) NOT NULL,
    CONSTRAINT [PK_Exams] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Exams_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Academics].[Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Exams_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Academics].[Subjects] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Exams_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Academics].[Teachers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Academics].[TeacherSubjects] (
    [Id] nvarchar(450) NOT NULL,
    [TeacherId] nvarchar(36) NOT NULL,
    [SubjectId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_TeacherSubjects] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeacherSubjects_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Academics].[Subjects] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeacherSubjects_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Academics].[Teachers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Academics].[ChoiceAnswers] (
    [Id] nvarchar(450) NOT NULL,
    [QuestionId] nvarchar(450) NOT NULL,
    [ChoiceId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_ChoiceAnswers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChoiceAnswers_Choices_ChoiceId] FOREIGN KEY ([ChoiceId]) REFERENCES [Academics].[Choices] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ChoiceAnswers_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Academics].[Questions] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Academics].[Certificates] (
    [Id] nvarchar(450) NOT NULL,
    [StudentId] nvarchar(36) NOT NULL,
    [GPA] float(4) NOT NULL,
    [IssuedDate] datetime2 NOT NULL,
    [CurriculumId] nvarchar(450) NOT NULL,
    [TemplateName] nvarchar(100) NOT NULL,
    [DegreeType] int NOT NULL,
    [GradeId] nvarchar(450) NULL,
    [ClassId] nvarchar(450) NULL,
    [AcademicYear] nvarchar(20) NULL,
    [Semester] nvarchar(20) NULL,
    [IsArchived] bit NOT NULL,
    [IsVerified] bit NOT NULL,
    [VerifiedDate] datetime2 NULL,
    [VerifiedBy] nvarchar(255) NULL,
    [PdfData] varbinary(max) NOT NULL,
    [FileName] nvarchar(255) NOT NULL,
    [ContentType] nvarchar(100) NOT NULL DEFAULT N'application/pdf',
    [FileSize] bigint NOT NULL,
    [CertificateNumber] nvarchar(50) NULL,
    CONSTRAINT [PK_Certificates] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Certificates_GPA] CHECK ([GPA] >= 0 AND [GPA] <= 4.0),
    CONSTRAINT [FK_Certificates_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Academics].[Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Certificates_Curricula_CurriculumId] FOREIGN KEY ([CurriculumId]) REFERENCES [Academics].[Curricula] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Certificates_Grades_GradeId] FOREIGN KEY ([GradeId]) REFERENCES [Academics].[Grades] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Certificates_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Academics].[Students] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Academics].[Degrees] (
    [Id] nvarchar(450) NOT NULL,
    [StudentId] nvarchar(36) NOT NULL,
    [SubjectId] nvarchar(450) NOT NULL,
    [DegreeType] int NOT NULL,
    [Score] float NOT NULL,
    [MaxScore] float NOT NULL,
    [SubjectName] nvarchar(150) NOT NULL,
    CONSTRAINT [PK_Degrees] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Degrees_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Academics].[Students] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Degrees_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Academics].[Subjects] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Academics].[ParentStudent] (
    [ParentId] nvarchar(36) NOT NULL,
    [StudentId] nvarchar(36) NOT NULL,
    [Relation] nvarchar(max) NULL,
    CONSTRAINT [PK_ParentStudent] PRIMARY KEY ([ParentId], [StudentId]),
    CONSTRAINT [FK_ParentStudent_Parents_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [Academics].[Parents] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ParentStudent_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Academics].[Students] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Academics].[ExamQuestions] (
    [Id] nvarchar(450) NOT NULL,
    [ExamId] nvarchar(450) NOT NULL,
    [QuestionId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_ExamQuestions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamQuestions_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Academics].[Exams] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ExamQuestions_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Academics].[Questions] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Academics].[StudentExamAnswers] (
    [Id] nvarchar(450) NOT NULL,
    [StudentId] nvarchar(36) NOT NULL,
    [ExamId] nvarchar(450) NOT NULL,
    [QuestionId] nvarchar(450) NOT NULL,
    [ChoiceId] nvarchar(max) NULL,
    [TrueAndFalseAnswer] bit NULL,
    [CorrectTextAnswer] nvarchar(max) NULL,
    [ConnectionLeftId] nvarchar(max) NULL,
    [ConnectionRightId] nvarchar(max) NULL,
    [Mark] decimal(18,2) NULL,
    CONSTRAINT [PK_StudentExamAnswers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentExamAnswers_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Academics].[Exams] ([Id]),
    CONSTRAINT [FK_StudentExamAnswers_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Academics].[Questions] ([Id]),
    CONSTRAINT [FK_StudentExamAnswers_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Academics].[Students] ([Id])
);
GO

CREATE TABLE [Academics].[DegreeComponents] (
    [Id] nvarchar(450) NOT NULL,
    [DegreeId] nvarchar(450) NOT NULL,
    [ComponentTypeId] nvarchar(450) NOT NULL,
    [Score] float NOT NULL DEFAULT 0.0E0,
    [MaxScore] float NOT NULL DEFAULT 0.0E0,
    [ComponentName] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_DegreeComponents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DegreeComponents_DegreeComponentTypes_ComponentTypeId] FOREIGN KEY ([ComponentTypeId]) REFERENCES [Academics].[DegreeComponentTypes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DegreeComponents_Degrees_DegreeId] FOREIGN KEY ([DegreeId]) REFERENCES [Academics].[Degrees] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Admins_AppUserId] ON [Identity].[Admins] ([AppUserId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [Identity].[AppRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AppUserRoles_RoleId] ON [Identity].[AppUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [Identity].[AppUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [Identity].[AppUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_Certificates_CertificateNumber] ON [Academics].[Certificates] ([CertificateNumber]) WHERE [CertificateNumber] IS NOT NULL;
GO

CREATE INDEX [IX_Certificates_ClassId] ON [Academics].[Certificates] ([ClassId]);
GO

CREATE INDEX [IX_Certificates_CurriculumId] ON [Academics].[Certificates] ([CurriculumId]);
GO

CREATE INDEX [IX_Certificates_GradeId] ON [Academics].[Certificates] ([GradeId]);
GO

CREATE INDEX [IX_Certificates_IssuedDate] ON [Academics].[Certificates] ([IssuedDate]);
GO

CREATE INDEX [IX_Certificates_StudentId] ON [Academics].[Certificates] ([StudentId]);
GO

CREATE INDEX [IX_ChoiceAnswers_ChoiceId] ON [Academics].[ChoiceAnswers] ([ChoiceId]);
GO

CREATE UNIQUE INDEX [IX_ChoiceAnswers_QuestionId] ON [Academics].[ChoiceAnswers] ([QuestionId]);
GO

CREATE INDEX [IX_Choices_QuestionId] ON [Academics].[Choices] ([QuestionId]);
GO

CREATE INDEX [IX_ClassAppointments_ClassId] ON [Academics].[ClassAppointments] ([ClassId]);
GO

CREATE INDEX [IX_ClassAppointments_SubjectId] ON [Academics].[ClassAppointments] ([SubjectId]);
GO

CREATE INDEX [IX_ClassAppointments_TeacherId] ON [Academics].[ClassAppointments] ([TeacherId]);
GO

CREATE INDEX [IX_ClassAssets_ClassId] ON [Academics].[ClassAssets] ([ClassId]);
GO

CREATE INDEX [IX_Classes_GradeId] ON [Academics].[Classes] ([GradeId]);
GO

CREATE INDEX [IX_DegreeComponents_ComponentTypeId] ON [Academics].[DegreeComponents] ([ComponentTypeId]);
GO

CREATE UNIQUE INDEX [IX_DegreeComponents_DegreeId_ComponentTypeId] ON [Academics].[DegreeComponents] ([DegreeId], [ComponentTypeId]);
GO

CREATE UNIQUE INDEX [IX_DegreeComponentTypes_SubjectId_ComponentName] ON [Academics].[DegreeComponentTypes] ([SubjectId], [ComponentName]);
GO

CREATE INDEX [IX_Degrees_StudentId] ON [Academics].[Degrees] ([StudentId]);
GO

CREATE INDEX [IX_Degrees_SubjectId] ON [Academics].[Degrees] ([SubjectId]);
GO

CREATE UNIQUE INDEX [IX_ExamQuestions_ExamId_QuestionId] ON [Academics].[ExamQuestions] ([ExamId], [QuestionId]);
GO

CREATE INDEX [IX_ExamQuestions_QuestionId] ON [Academics].[ExamQuestions] ([QuestionId]);
GO

CREATE INDEX [IX_Exams_ClassId] ON [Academics].[Exams] ([ClassId]);
GO

CREATE INDEX [IX_Exams_SubjectId] ON [Academics].[Exams] ([SubjectId]);
GO

CREATE INDEX [IX_Exams_TeacherId] ON [Academics].[Exams] ([TeacherId]);
GO

CREATE INDEX [IX_FileRecords_UserId] ON [Files].[FileRecords] ([UserId]);
GO

CREATE INDEX [IX_Grades_CurriculumId] ON [Academics].[Grades] ([CurriculumId]);
GO

CREATE INDEX [IX_Notifications_CreatorUserId] ON [Notifications].[Notifications] ([CreatorUserId]);
GO

CREATE INDEX [IX_Parents_AppUserId] ON [Academics].[Parents] ([AppUserId]);
GO

CREATE INDEX [IX_ParentStudent_StudentId] ON [Academics].[ParentStudent] ([StudentId]);
GO

CREATE INDEX [IX_Questions_TeacherId] ON [Academics].[Questions] ([TeacherId]);
GO

CREATE INDEX [IX_RefreshTokens_AppUserId] ON [Identity].[RefreshTokens] ([AppUserId]);
GO

CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [Identity].[RefreshTokens] ([Token]);
GO

CREATE INDEX [IX_RoleClaims_RoleId] ON [Identity].[RoleClaims] ([RoleId]);
GO

CREATE INDEX [IX_StudentExamAnswers_ExamId] ON [Academics].[StudentExamAnswers] ([ExamId]);
GO

CREATE INDEX [IX_StudentExamAnswers_QuestionId] ON [Academics].[StudentExamAnswers] ([QuestionId]);
GO

CREATE UNIQUE INDEX [IX_StudentExamAnswers_StudentId_ExamId_QuestionId] ON [Academics].[StudentExamAnswers] ([StudentId], [ExamId], [QuestionId]);
GO

CREATE UNIQUE INDEX [IX_StudentExamResults_StudentId_ExamId] ON [Academics].[StudentExamResults] ([StudentId], [ExamId]);
GO

CREATE INDEX [IX_Students_AppUserId] ON [Academics].[Students] ([AppUserId]);
GO

CREATE INDEX [IX_Students_ClassId] ON [Academics].[Students] ([ClassId]);
GO

CREATE INDEX [IX_Students_CurriculumId] ON [Academics].[Students] ([CurriculumId]);
GO

CREATE INDEX [IX_Subjects_GradeId] ON [Academics].[Subjects] ([GradeId]);
GO

CREATE INDEX [IX_TeacherClasses_ClassId] ON [Academics].[TeacherClasses] ([ClassId]);
GO

CREATE UNIQUE INDEX [IX_TeacherClasses_TeacherId_ClassId] ON [Academics].[TeacherClasses] ([TeacherId], [ClassId]);
GO

CREATE INDEX [IX_TeacherCurricula_TeachersId] ON [Academics].[TeacherCurricula] ([TeachersId]);
GO

CREATE INDEX [IX_Teachers_AppUserId] ON [Academics].[Teachers] ([AppUserId]);
GO

CREATE INDEX [IX_TeacherSubjects_SubjectId] ON [Academics].[TeacherSubjects] ([SubjectId]);
GO

CREATE UNIQUE INDEX [IX_TeacherSubjects_TeacherId_SubjectId] ON [Academics].[TeacherSubjects] ([TeacherId], [SubjectId]);
GO

CREATE INDEX [IX_UserClaims_UserId] ON [Identity].[UserClaims] ([UserId]);
GO

CREATE INDEX [IX_UserLogins_UserId] ON [Identity].[UserLogins] ([UserId]);
GO

CREATE INDEX [IX_UserNotifications_NotificationId] ON [Notifications].[UserNotifications] ([NotificationId]);
GO

CREATE INDEX [IX_UserNotifications_UserId] ON [Notifications].[UserNotifications] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260101170519_Init', N'8.0.0');
GO

COMMIT;
GO

