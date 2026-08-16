CREATE TABLE "ReportTemplate" (
    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "EntityVersion" INTEGER NOT NULL DEFAULT 1,
    "EntityWrittenAt" TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%S', 'now')),
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedBy" TEXT NULL,
    "CreatedAtUtc" TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%S', 'now')),
    "UpdatedBy" TEXT NULL,
    "UpdatedAtUtc" TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%S', 'now')),
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
    "FilePath" TEXT NOT NULL,
    "Version" INTEGER NOT NULL DEFAULT 1,
    "OutputDirectory" TEXT NOT NULL,
    "FileNamePattern" TEXT NOT NULL
);

CREATE TABLE "ReportMetric" (
    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "EntityVersion" INTEGER NOT NULL DEFAULT 1,
    "EntityWrittenAt" TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%S', 'now')),
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedBy" TEXT NULL,
    "CreatedAtUtc" TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%S', 'now')),
    "UpdatedBy" TEXT NULL,
    "UpdatedAtUtc" TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%S', 'now')),
    "ReportTemplateId" INTEGER NOT NULL,
    "NamedRange" TEXT NOT NULL,
    "SqlQuery" TEXT NOT NULL,
    "DatabaseType" INTEGER NOT NULL DEFAULT 1,
    "MaxRows" INTEGER NULL,
    CONSTRAINT "FK_ReportMetric_ReportTemplate" FOREIGN KEY ("ReportTemplateId") 
        REFERENCES "ReportTemplate" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ReportParameter" (
    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "EntityVersion" INTEGER NOT NULL DEFAULT 1,
    "EntityWrittenAt" TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%S', 'now')),
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedBy" TEXT NULL,
    "CreatedAtUtc" TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%S', 'now')),
    "UpdatedBy" TEXT NULL,
    "UpdatedAtUtc" TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%S', 'now')),
    "ReportTemplateId" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "Type" INTEGER NOT NULL DEFAULT 1,
    "IsRequired" INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT "FK_ReportParameter_ReportTemplate" FOREIGN KEY ("ReportTemplateId") 
        REFERENCES "ReportTemplate" ("Id") ON DELETE CASCADE
);

-- Indexing foreign keys for query performance
CREATE INDEX "IX_ReportMetric_ReportTemplateId" ON "ReportMetric" ("ReportTemplateId");
CREATE INDEX "IX_ReportParameter_ReportTemplateId" ON "ReportParameter" ("ReportTemplateId");

CREATE TABLE "ReportRunnerQueue" (
    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "EntityVersion" INTEGER NOT NULL DEFAULT 1,
    "EntityWrittenAt" TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%S', 'now')),
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedBy" TEXT NULL,
    "CreatedAtUtc" TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%S', 'now')),
    "UpdatedBy" TEXT NULL,
    "UpdatedAtUtc" TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%S', 'now')),
    "ReportTemplateId" INTEGER NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 1,
    "ParameterValuesJson" TEXT NULL,
    "OutputFilePath" TEXT NULL,
    "ErrorMessage" TEXT NULL,
    "StartedAtUtc" TEXT NULL,
    "CompletedAtUtc" TEXT NULL,
    CONSTRAINT "FK_ReportRunnerQueue_ReportTemplate" FOREIGN KEY ("ReportTemplateId") 
        REFERENCES "ReportTemplate" ("Id") ON DELETE CASCADE
);

-- Indexing foreign key and status for worker polling queries
CREATE INDEX "IX_ReportRunnerQueue_ReportTemplateId" ON "ReportRunnerQueue" ("ReportTemplateId");
CREATE INDEX "IX_ReportRunnerQueue_Status" ON "ReportRunnerQueue" ("Status");