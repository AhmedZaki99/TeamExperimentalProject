BEGIN TRANSACTION;
GO

DROP TABLE [Comments];
GO

DROP TABLE [RoleUser];
GO

DROP TABLE [Posts];
GO

DROP TABLE [Roles];
GO

DROP TABLE [Users];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220303132021_DropOldTables', N'6.0.2');
GO

COMMIT;
GO

