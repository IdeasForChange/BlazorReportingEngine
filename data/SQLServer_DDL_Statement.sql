-- Create Schema
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Reporting')
BEGIN
    EXEC('CREATE SCHEMA [Reporting]');
END
GO

DROP TABLE IF EXISTS [Reporting].[ReportRunnerQueue];
DROP TABLE IF EXISTS [Reporting].[ReportParameter];
DROP TABLE IF EXISTS [Reporting].[ReportMetric];
DROP TABLE IF EXISTS [Reporting].[ReportTemplate];
DROP TABLE IF EXISTS [Reporting].[ReportMaster];
DROP TABLE IF EXISTS [Reporting].[DatabaseConnection];

CREATE TABLE [Reporting].[DatabaseConnection] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [EntityVersion] INT NOT NULL DEFAULT 1,
    [EntityWrittenAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),	
    [ConnectionName] NVARCHAR(100) NOT NULL,
    [DatabaseType] INT NOT NULL,
    [Environment] INT NOT NULL,
    [ServerHost] NVARCHAR(255) NOT NULL,
    [Port] INT NOT NULL DEFAULT 1433,
    [DatabaseName] NVARCHAR(128) NOT NULL,
    [AuthenticationMethod] INT NOT NULL,
    [UserId] NVARCHAR(100) NULL,
    [Password] NVARCHAR(255) NULL,
    [TimeoutSeconds] INT NOT NULL DEFAULT 30,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(256) NULL,
    [CreatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedBy] NVARCHAR(256) NULL,
    [UpdatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_DatabaseConnection] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_DatabaseConnection_Name_Environment] UNIQUE ([ConnectionName], [Environment])
);

-- Index for querying connections by Environment
CREATE NONCLUSTERED INDEX [IX_DatabaseConnection_Environment] ON [Reporting].[DatabaseConnection] ([Environment]);

CREATE TABLE [Reporting].[ReportMaster] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [EntityVersion] INT NOT NULL DEFAULT 1,
    [EntityWrittenAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [ReportNamePattern] NVARCHAR(255) NOT NULL,
    [ReportDirectory] NVARCHAR(1000) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(256) NULL,
    [CreatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedBy] NVARCHAR(256) NULL,
    [UpdatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_ReportMaster] PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [Reporting].[ReportParameter] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [EntityVersion] INT NOT NULL DEFAULT 1,
    [EntityWrittenAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ReportMasterId] BIGINT NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [ParameterType] INT NOT NULL DEFAULT 1,
    [IsRequired] BIT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(256) NULL,
    [CreatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedBy] NVARCHAR(256) NULL,
    [UpdatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_ReportParameter] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReportParameter_ReportMaster] FOREIGN KEY ([ReportMasterId]) 
        REFERENCES [Reporting].[ReportMaster] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Reporting].[ReportTemplate] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [EntityVersion] INT NOT NULL DEFAULT 1,
    [EntityWrittenAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ReportMasterId] BIGINT NOT NULL,
    [TemplateFileName] NVARCHAR(1000) NOT NULL,
    [TemplatePath] NVARCHAR(1000) NOT NULL,
    [TemplateVersion] INT NOT NULL DEFAULT 1,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(256) NULL,
    [CreatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedBy] NVARCHAR(256) NULL,
    [UpdatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_ReportTemplate] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReportTemplate_ReportMaster] FOREIGN KEY ([ReportMasterId]) 
        REFERENCES [Reporting].[ReportMaster] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Reporting].[ReportMetric] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [EntityVersion] INT NOT NULL DEFAULT 1,
    [EntityWrittenAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ReportTemplateId] BIGINT NOT NULL,
    [NamedRange] NVARCHAR(255) NOT NULL,
    [SqlQuery] NVARCHAR(MAX) NOT NULL,
	[DatabaseConnectionId] BIGINT NOT NULL,
    [MaxRows] INT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(256) NULL,
    [CreatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedBy] NVARCHAR(256) NULL,
    [UpdatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_ReportMetric] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReportMetric_ReportTemplate] FOREIGN KEY ([ReportTemplateId]) 
        REFERENCES [Reporting].[ReportTemplate] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ReportMetric_DatabaseConnection] FOREIGN KEY ([DatabaseConnectionId]) 
        REFERENCES [Reporting].[DatabaseConnection] ([Id]) ON DELETE CASCADE
);

-- Indexing foreign keys for query performance
CREATE INDEX [IX_ReportParameter_ReportMasterId] ON [Reporting].[ReportParameter] ([ReportMasterId]);
CREATE INDEX [IX_ReportTemplate_ReportMasterId] ON [Reporting].[ReportTemplate] ([ReportMasterId]);
CREATE INDEX [IX_ReportMetric_ReportTemplateId] ON [Reporting].[ReportMetric] ([ReportTemplateId]);
CREATE INDEX [IX_ReportMetric_DatabaseConnectionId] ON [Reporting].[ReportMetric] ([DatabaseConnectionId]);

CREATE TABLE [Reporting].[ReportRunnerQueue] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [EntityVersion] INT NOT NULL DEFAULT 1,
    [EntityWrittenAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ReportMasterId] BIGINT NOT NULL,
    [Status] INT NOT NULL DEFAULT 1, -- Maps to QueueStatus enum (1 = Pending)
    [ParameterValuesJson] NVARCHAR(MAX) NULL,
    [OutputFilePath] NVARCHAR(1000) NULL,
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [StartedAtUtc] DATETIME2 NULL,
    [CompletedAtUtc] DATETIME2 NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(256) NULL,
    [CreatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedBy] NVARCHAR(256) NULL,
    [UpdatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_ReportRunnerQueue] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReportRunnerQueue_ReportMaster] FOREIGN KEY ([ReportMasterId]) 
        REFERENCES [Reporting].[ReportMaster] ([Id]) ON DELETE CASCADE
);

-- Indexing foreign key and status for worker polling queries
CREATE INDEX [IX_ReportRunnerQueue_ReportMasterId] ON [Reporting].[ReportRunnerQueue] ([ReportMasterId]);
CREATE INDEX [IX_ReportRunnerQueue_Status] ON [Reporting].[ReportRunnerQueue] ([Status]);