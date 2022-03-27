BEGIN TRANSACTION;
GO

DROP INDEX [IX_Users_Email] ON [Users];
GO

DROP INDEX [IX_Users_UserName] ON [Users];
GO

ALTER TABLE [Users] ADD [NormalizedEmail] AS UPPER([Email]) PERSISTED;
GO

ALTER TABLE [Users] ADD [NormalizedUserName] AS UPPER([UserName]) PERSISTED;
GO

CREATE UNIQUE INDEX [IX_Users_NormalizedEmail] ON [Users] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [IX_Users_NormalizedUserName] ON [Users] ([NormalizedUserName]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220327122545_AddNormalizedUserColumns', N'6.0.3');
GO

COMMIT;
GO

