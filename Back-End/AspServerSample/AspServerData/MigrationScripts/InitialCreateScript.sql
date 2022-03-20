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
    [RoleId] nvarchar(450) NOT NULL,
    [RoleName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([RoleId])
);
GO

CREATE TABLE [Users] (
    [UserId] nvarchar(450) NOT NULL,
    [UserName] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NULL,
    [LastName] nvarchar(max) NULL,
    [BirthDate] datetime2 NOT NULL,
    [LastSignedIn] datetime2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);
GO

CREATE TABLE [Posts] (
    [PostId] nvarchar(450) NOT NULL,
    [Caption] nvarchar(max) NULL,
    [Content] nvarchar(max) NOT NULL,
    [DatePosted] datetime2 NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY ([PostId]),
    CONSTRAINT [FK_Posts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [RoleUser] (
    [RolesRoleId] nvarchar(450) NOT NULL,
    [UsersUserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_RoleUser] PRIMARY KEY ([RolesRoleId], [UsersUserId]),
    CONSTRAINT [FK_RoleUser_Roles_RolesRoleId] FOREIGN KEY ([RolesRoleId]) REFERENCES [Roles] ([RoleId]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleUser_Users_UsersUserId] FOREIGN KEY ([UsersUserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Comments] (
    [CommentId] nvarchar(450) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [DatePosted] datetime2 NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [PostId] nvarchar(450) NOT NULL,
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

CREATE INDEX [IX_RoleUser_UsersUserId] ON [RoleUser] ([UsersUserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220226232530_InitialCreate', N'6.0.2');
GO

COMMIT;
GO

