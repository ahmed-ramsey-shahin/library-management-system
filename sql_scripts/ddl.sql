USE [master];
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'library_management_system')
BEGIN
    ALTER DATABASE library_management_system 
    SET SINGLE_USER 
    WITH ROLLBACK IMMEDIATE;
END

DROP DATABASE IF EXISTS library_management_system;
CREATE DATABASE library_management_system;
GO

USE library_management_system;
GO

CREATE TABLE [authors] (
    [id] uniqueidentifier PRIMARY KEY,
    [name] nvarchar(255) UNIQUE NOT NULL,
    [created_at] datetime NOT NULL DEFAULT GETUTCDATE(),
    [is_deleted] bit NOT NULL DEFAULT (0)
)
GO

CREATE TABLE [publishers] (
    [id] uniqueidentifier PRIMARY KEY,
    [name] nvarchar(255) UNIQUE NOT NULL,
    [created_at] datetime NOT NULL DEFAULT GETUTCDATE(),
    [is_deleted] bit NOT NULL DEFAULT (0)
)
GO

CREATE TABLE [keywords] (
    [id] uniqueidentifier PRIMARY KEY,
    [name] nvarchar(255) UNIQUE NOT NULL,
    [created_at] datetime NOT NULL DEFAULT GETUTCDATE(),
    [is_deleted] bit NOT NULL DEFAULT (0)
)
GO

CREATE TABLE [genres] (
    [id] uniqueidentifier PRIMARY KEY,
    [name] nvarchar(255) UNIQUE NOT NULL,
    [created_at] datetime NOT NULL DEFAULT GETUTCDATE(),
    [is_deleted] bit NOT NULL DEFAULT (0)
)
GO

CREATE TABLE [themes] (
    [id] uniqueidentifier PRIMARY KEY,
    [name] nvarchar(255) UNIQUE NOT NULL,
    [created_at] datetime NOT NULL DEFAULT GETUTCDATE(),
    [is_deleted] bit NOT NULL DEFAULT (0)
)
GO

CREATE TABLE [audience] (
    [id] uniqueidentifier PRIMARY KEY,
    [name] nvarchar(255) UNIQUE NOT NULL,
    [created_at] datetime NOT NULL DEFAULT GETUTCDATE(),
    [is_deleted] bit NOT NULL DEFAULT (0)
)
GO

CREATE TABLE [categories] (
    [id] uniqueidentifier PRIMARY KEY,
    [name] nvarchar(255) UNIQUE NOT NULL,
    [created_at] datetime NOT NULL DEFAULT GETUTCDATE(),
    [is_deleted] bit NOT NULL DEFAULT (0)
)
GO

CREATE TABLE [users] (
    [id] uniqueidentifier PRIMARY KEY,
    [email] nvarchar(255) UNIQUE NOT NULL,
    [role] nvarchar(50) NOT NULL DEFAULT 'member',
    [status] nvarchar(50) NOT NULL DEFAULT 'active',
    [password] varchar(512) NOT NULL,
    [salt] varchar(64) NOT NULL,
    [created_at] datetime NOT NULL DEFAULT GETUTCDATE(),
    [is_deleted] bit NOT NULL DEFAULT (0)
)
GO

CREATE TABLE [librarian_categories] (
    [user_id] uniqueidentifier,
    [category_id] uniqueidentifier,
    PRIMARY KEY ([user_id], [category_id])
)
GO

CREATE TABLE [books] (
    [id] uniqueidentifier PRIMARY KEY,
    [isbn] varchar(17) UNIQUE NOT NULL,
    [issn] varchar(9) UNIQUE NOT NULL,
    [title] nvarchar(255) NOT NULL,
    [publisher_id] uniqueidentifier NOT NULL,
    [publishing_date] date NOT NULL,
    [language] nvarchar(50) NOT NULL DEFAULT 'english',
    [edition] nvarchar(100) NOT NULL,
    [borrow_price_per_day] money NOT NULL,
    [fine_per_day] money NOT NULL,
    [lost_fee] money NOT NULL,
    [damage_fee] money NOT NULL,
    [created_at] datetime NOT NULL DEFAULT GETUTCDATE(),
    [is_deleted] bit NOT NULL DEFAULT (0)
)
GO

CREATE TABLE [book_categories] (
    [book_id] uniqueidentifier,
    [category_id] uniqueidentifier,
    PRIMARY KEY ([book_id], [category_id])
)
GO

CREATE TABLE [book_authors] (
    [book_id] uniqueidentifier,
    [author_id] uniqueidentifier,
    PRIMARY KEY ([book_id], [author_id])
)
GO

CREATE TABLE [book_keywords] (
    [book_id] uniqueidentifier,
    [keyword_id] uniqueidentifier,
    PRIMARY KEY ([book_id], [keyword_id])
)
GO

CREATE TABLE [book_genres] (
    [book_id] uniqueidentifier,
    [genre_id] uniqueidentifier,
    PRIMARY KEY ([book_id], [genre_id])
)
GO

CREATE TABLE [book_themes] (
    [book_id] uniqueidentifier,
    [theme_id] uniqueidentifier,
    PRIMARY KEY ([book_id], [theme_id])
)
GO

CREATE TABLE [book_audience] (
    [book_id] uniqueidentifier,
    [audience_id] uniqueidentifier,
    PRIMARY KEY ([book_id], [audience_id])
)
GO

CREATE TABLE [book_copies] (
    [id] uniqueidentifier PRIMARY KEY,
    [book_id] uniqueidentifier,
    [barcode] varchar(50) UNIQUE NOT NULL,
    [status] varchar(50) NOT NULL DEFAULT 'good',
    [state] varchar(50) NOT NULL DEFAULT 'available',
    [location] varchar(100) NOT NULL,
    [acquisition_date] date NOT NULL DEFAULT GETUTCDATE(),
    [created_at] datetime NOT NULL DEFAULT GETUTCDATE(),
    [is_deleted] bit NOT NULL DEFAULT (0),
    [version] rowversion NOT NULL
)
GO

CREATE TABLE [borrow_records] (
    [id] uniqueidentifier PRIMARY KEY,
    [member_id] uniqueidentifier NOT NULL,
    [book_copy_id] uniqueidentifier NOT NULL,
    [status] varchar(50) NOT NULL DEFAULT 'waiting',
    [due_date] date NOT NULL,
    [fine_accrued] money NOT NULL DEFAULT (0),
    [renewal_count] int NOT NULL DEFAULT (0),
    [pickup_deadline] datetime,
    [created_at] datetime NOT NULL DEFAULT GETUTCDATE(),
    [is_deleted] bit NOT NULL DEFAULT (0)
)
GO

CREATE TABLE [fines] (
    [id] uniqueidentifier PRIMARY KEY,
    [member_id] uniqueidentifier NOT NULL,
    [borrow_record_id] uniqueidentifier NOT NULL,
    [status] varchar(50) NOT NULL DEFAULT 'unpaid',
    [amount] money NOT NULL DEFAULT (0),
    [description] nvarchar(500),
    [fine_date] datetime NOT NULL DEFAULT GETUTCDATE()
)
GO

CREATE TABLE [refresh_tokens] (
	[id] uniqueidentifier PRIMARY KEY,
	[token] varchar(256) NOT NULL,
	[user_id] uniqueidentifier NOT NULL,
	[expires_one] datetime2 NOT NULL
)
GO

CREATE INDEX [IX_Authors_Name] ON [authors] ([name])
GO

CREATE INDEX [IX_Publishers_Name] ON [publishers] ([name])
GO

CREATE INDEX [IX_Keywords_Name] ON [keywords] ([name])
GO

CREATE INDEX [IX_Genres_Name] ON [genres] ([name])
GO

CREATE INDEX [IX_Themes_Name] ON [themes] ([name])
GO

CREATE INDEX [IX_Audience_Name] ON [audience] ([name])
GO

CREATE INDEX [IX_Categories_Name] ON [categories] ([name])
GO

CREATE INDEX [IX_BookCopies_Barcode] ON [book_copies] ([barcode])
GO

CREATE INDEX [IX_BookCopies_BookId_State] ON [book_copies] ([book_id], [state])
GO

CREATE INDEX [IX_RefreshTokens_Token] ON [refresh_tokens] ([token])
GO

ALTER TABLE [librarian_categories] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [librarian_categories] ADD FOREIGN KEY ([category_id]) REFERENCES [categories] ([id])
GO

ALTER TABLE [books] ADD FOREIGN KEY ([publisher_id]) REFERENCES [publishers] ([id])
GO

ALTER TABLE [book_categories] ADD FOREIGN KEY ([book_id]) REFERENCES [books] ([id])
GO

ALTER TABLE [book_categories] ADD FOREIGN KEY ([category_id]) REFERENCES [categories] ([id])
GO

ALTER TABLE [book_authors] ADD FOREIGN KEY ([book_id]) REFERENCES [books] ([id])
GO

ALTER TABLE [book_authors] ADD FOREIGN KEY ([author_id]) REFERENCES [authors] ([id])
GO

ALTER TABLE [book_keywords] ADD FOREIGN KEY ([book_id]) REFERENCES [books] ([id])
GO

ALTER TABLE [book_keywords] ADD FOREIGN KEY ([keyword_id]) REFERENCES [keywords] ([id])
GO

ALTER TABLE [book_genres] ADD FOREIGN KEY ([book_id]) REFERENCES [books] ([id])
GO

ALTER TABLE [book_genres] ADD FOREIGN KEY ([genre_id]) REFERENCES [genres] ([id])
GO

ALTER TABLE [book_themes] ADD FOREIGN KEY ([book_id]) REFERENCES [books] ([id])
GO

ALTER TABLE [book_themes] ADD FOREIGN KEY ([theme_id]) REFERENCES [themes] ([id])
GO

ALTER TABLE [book_audience] ADD FOREIGN KEY ([book_id]) REFERENCES [books] ([id])
GO

ALTER TABLE [book_audience] ADD FOREIGN KEY ([audience_id]) REFERENCES [audience] ([id])
GO

ALTER TABLE [book_copies] ADD FOREIGN KEY ([book_id]) REFERENCES [books] ([id])
GO

ALTER TABLE [borrow_records] ADD FOREIGN KEY ([member_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [borrow_records] ADD FOREIGN KEY ([book_copy_id]) REFERENCES [book_copies] ([id])
GO

ALTER TABLE [fines] ADD FOREIGN KEY ([member_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [fines] ADD FOREIGN KEY ([borrow_record_id]) REFERENCES [borrow_records] ([id])
GO

ALTER TABLE [refresh_tokens] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO
