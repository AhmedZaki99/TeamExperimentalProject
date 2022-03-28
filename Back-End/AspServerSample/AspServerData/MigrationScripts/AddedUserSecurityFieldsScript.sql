BEGIN TRANSACTION;
GO

ALTER TABLE [Users] ADD [Email] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Users] ADD [EmailConfirmed] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Users] ADD [PasswordHash] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Users] ADD [SecurityStamp] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220227080231_AddedUserSecurityFields', N'6.0.2');
GO

COMMIT;
GO

