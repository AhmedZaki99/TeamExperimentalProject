BEGIN TRANSACTION;
GO

ALTER TABLE [Posts] ADD [LastEdited] datetime2(0) NULL;
GO

ALTER TABLE [Comments] ADD [LastEdited] datetime2(0) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220306184440_AddLastEditedColumn', N'6.0.2');
GO

COMMIT;
GO

