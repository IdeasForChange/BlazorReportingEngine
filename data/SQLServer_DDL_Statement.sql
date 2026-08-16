-- Create Schema
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Reporting')
BEGIN
    EXEC('CREATE SCHEMA [Reporting]');
END
GO

CREATE TABLE [Reporting].[ReportTemplate] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [EntityVersion] INT NOT NULL DEFAULT 1,
    [EntityWrittenAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(256) NULL,
    [CreatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedBy] NVARCHAR(256) NULL,
    [UpdatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [FilePath] NVARCHAR(1000) NOT NULL,
    [Version] INT NOT NULL DEFAULT 1,
    [OutputDirectory] NVARCHAR(1000) NOT NULL,
    [FileNamePattern] NVARCHAR(255) NOT NULL,
    CONSTRAINT [PK_ReportTemplate] PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [Reporting].[ReportMetric] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [EntityVersion] INT NOT NULL DEFAULT 1,
    [EntityWrittenAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(256) NULL,
    [CreatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedBy] NVARCHAR(256) NULL,
    [UpdatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ReportTemplateId] BIGINT NOT NULL,
    [NamedRange] NVARCHAR(255) NOT NULL,
    [SqlQuery] NVARCHAR(MAX) NOT NULL,
    [DatabaseType] INT NOT NULL DEFAULT 1,
    [MaxRows] INT NULL,
    CONSTRAINT [PK_ReportMetric] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReportMetric_ReportTemplate] FOREIGN KEY ([ReportTemplateId]) 
        REFERENCES [Reporting].[ReportTemplate] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Reporting].[ReportParameter] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [EntityVersion] INT NOT NULL DEFAULT 1,
    [EntityWrittenAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(256) NULL,
    [CreatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedBy] NVARCHAR(256) NULL,
    [UpdatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ReportTemplateId] BIGINT NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Type] INT NOT NULL DEFAULT 1,
    [IsRequired] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_ReportParameter] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReportParameter_ReportTemplate] FOREIGN KEY ([ReportTemplateId]) 
        REFERENCES [Reporting].[ReportTemplate] ([Id]) ON DELETE CASCADE
);

-- Indexing foreign keys for query performance
CREATE INDEX [IX_ReportMetric_ReportTemplateId] ON [Reporting].[ReportMetric] ([ReportTemplateId]);
CREATE INDEX [IX_ReportParameter_ReportTemplateId] ON [Reporting].[ReportParameter] ([ReportTemplateId]);

CREATE TABLE [Reporting].[ReportRunnerQueue] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [EntityVersion] INT NOT NULL DEFAULT 1,
    [EntityWrittenAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(256) NULL,
    [CreatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedBy] NVARCHAR(256) NULL,
    [UpdatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ReportTemplateId] BIGINT NOT NULL,
    [Status] INT NOT NULL DEFAULT 1, -- Maps to QueueStatus enum (1 = Pending)
    [ParameterValuesJson] NVARCHAR(MAX) NULL,
    [OutputFilePath] NVARCHAR(1000) NULL,
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [StartedAtUtc] DATETIME2 NULL,
    [CompletedAtUtc] DATETIME2 NULL,
    CONSTRAINT [PK_ReportRunnerQueue] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReportRunnerQueue_ReportTemplate] FOREIGN KEY ([ReportTemplateId]) 
        REFERENCES [Reporting].[ReportTemplate] ([Id]) ON DELETE CASCADE
);

-- Indexing foreign key and status for worker polling queries
CREATE INDEX [IX_ReportRunnerQueue_ReportTemplateId] ON [Reporting].[ReportRunnerQueue] ([ReportTemplateId]);
CREATE INDEX [IX_ReportRunnerQueue_Status] ON [Reporting].[ReportRunnerQueue] ([Status]);