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

CREATE TABLE [Roles] (
    [RoleId] int NOT NULL IDENTITY,
    [RoleName] nvarchar(256) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([RoleId])
);
GO

CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [UserName] nvarchar(256) NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [Email] nvarchar(256) NOT NULL,
    [EmailConfirmed] bit NOT NULL,
    [FirstName] nvarchar(256) NULL,
    [LastName] nvarchar(256) NULL,
    [BirthDate] datetime2(0) NOT NULL,
    [LastSignedIn] datetime2(0) NULL,
    [DateCreated] datetime2(3) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);
GO

CREATE TABLE [Posts] (
    [PostId] int NOT NULL IDENTITY,
    [Caption] nvarchar(256) NULL,
    [Content] nvarchar(max) NOT NULL,
    [DatePosted] datetime2(0) NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY ([PostId]),
    CONSTRAINT [FK_Posts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [RoleUser] (
    [RolesRoleId] int NOT NULL,
    [UsersUserId] int NOT NULL,
    CONSTRAINT [PK_RoleUser] PRIMARY KEY ([RolesRoleId], [UsersUserId]),
    CONSTRAINT [FK_RoleUser_Roles_RolesRoleId] FOREIGN KEY ([RolesRoleId]) REFERENCES [Roles] ([RoleId]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleUser_Users_UsersUserId] FOREIGN KEY ([UsersUserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Comments] (
    [CommentId] int NOT NULL IDENTITY,
    [Content] nvarchar(max) NOT NULL,
    [DatePosted] datetime2(0) NOT NULL,
    [UserId] int NOT NULL,
    [PostId] int NOT NULL,
    CONSTRAINT [PK_Comments] PRIMARY KEY ([CommentId]),
    CONSTRAINT [FK_Comments_Posts_PostId] FOREIGN KEY ([PostId]) REFERENCES [Posts] ([PostId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Comments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId])
);
GO

CREATE INDEX [IX_Comments_PostId] ON [Comments] ([PostId]);
GO

CREATE INDEX [IX_Comments_UserId] ON [Comments] ([UserId]);
GO

CREATE INDEX [IX_Posts_UserId] ON [Posts] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Roles_RoleName] ON [Roles] ([RoleName]);
GO

CREATE INDEX [IX_RoleUser_UsersUserId] ON [RoleUser] ([UsersUserId]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

CREATE UNIQUE INDEX [IX_Users_UserName] ON [Users] ([UserName]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220306172449_InitialCreate', N'6.0.2');
GO

COMMIT;
GO

