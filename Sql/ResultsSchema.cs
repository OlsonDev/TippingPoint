namespace TippingPoint.Sql {
  public static class ResultsSchema {
    public static readonly string DropAndCreateBase = $@"
IF SCHEMA_ID('Benchmark') IS NULL EXEC('CREATE SCHEMA Benchmark AUTHORIZATION dbo;');

DROP TYPE IF EXISTS Benchmark.TableIterationTvp;
DROP TYPE IF EXISTS Benchmark.FlatResultTvp;
DROP TYPE IF EXISTS Benchmark.TableIoTvp;

CREATE TYPE Benchmark.TableIterationTvp AS TABLE (
    IterationNumber              INT            NOT NULL
  , TableName                    VARCHAR(50)    NOT NULL
  , [RowCount]                   BIGINT         NOT NULL
);

CREATE TYPE Benchmark.FlatResultTvp AS TABLE (
    IndexClassName               VARCHAR(100)   NOT NULL
  , IndexClassCommand            NVARCHAR(MAX)  NOT NULL
  , QueryClassName               VARCHAR(100)   NOT NULL
  , QueryClassCommand            NVARCHAR(MAX)  NOT NULL
  , IterationNumber              INT            NOT NULL
  , SampleNumber                 INT            NOT NULL
  , HitOrMiss                    VARCHAR(4)     NOT NULL
  , [RowCount]                   BIGINT         NOT NULL
  , StopwatchTicks               BIGINT         NOT NULL
  , ExecutionCpuMs               BIGINT         NOT NULL
  , ExecutionElapsedMs           BIGINT         NOT NULL
  , ParseAndCompileCpuMs         BIGINT         NOT NULL
  , ParseAndCompileElapsedMs     BIGINT         NOT NULL
);

CREATE TYPE Benchmark.TableIoTvp AS TABLE (
    IndexClassName               VARCHAR(100)   NOT NULL
  , QueryClassName               VARCHAR(100)   NOT NULL
  , IterationNumber              INT            NOT NULL
  , SampleNumber                 INT            NOT NULL
  , HitOrMiss                    VARCHAR(4)     NOT NULL
  , TableName                    VARCHAR(50)    NOT NULL
  , ScanCount                    INT                NULL
  , LogicalReads                 INT                NULL
  , PhysicalReads                INT                NULL
  , PageServerReads              INT                NULL
  , ReadAheadReads               INT                NULL
  , PageServerReadAheadReads     INT                NULL
  , LobLogicalReads              INT                NULL
  , LobPhysicalReads             INT                NULL
  , LobPageServerReads           INT                NULL
  , LobReadAheadReads            INT                NULL
  , LobPageServerReadAheadReads  INT                NULL
);

DROP TABLE IF EXISTS Benchmark.SampleTableIo;
DROP TABLE IF EXISTS Benchmark.Sample;
DROP TABLE IF EXISTS Benchmark.QueryClass;
DROP TABLE IF EXISTS Benchmark.IndexClass;
DROP TABLE IF EXISTS Benchmark.TableIteration;

CREATE TABLE Benchmark.TableIteration (
    TableIterationID             INT IDENTITY(1, 1)   NOT NULL
  , IterationNumber              INT                  NOT NULL
  , TableName                    VARCHAR(50)          NOT NULL
  , [RowCount]                   BIGINT               NOT NULL
  , CONSTRAINT PK_Benchmark_TableIteration                  PRIMARY KEY (TableIterationID)
);

CREATE TABLE Benchmark.IndexClass (
    IndexClassID                 INT IDENTITY(1, 1)  NOT NULL
  , Name                         VARCHAR(100)        NOT NULL
  , Command                      NVARCHAR(MAX)       NOT NULL
  , CONSTRAINT PK_Benchmark_IndexClass                      PRIMARY KEY (IndexClassID)
);

CREATE TABLE Benchmark.QueryClass (
    QueryClassID                 INT IDENTITY(1, 1)  NOT NULL
  , Name                         VARCHAR(100)        NOT NULL
  , Command                      NVARCHAR(MAX)       NOT NULL
  , CONSTRAINT PK_Benchmark_QueryClass                      PRIMARY KEY (QueryClassID)
);

CREATE TABLE Benchmark.Sample (
    SampleID                     INT IDENTITY(1, 1)   NOT NULL
  , IndexClassID                 INT                  NOT NULL
  , QueryClassID                 INT                  NOT NULL
  , IterationNumber              INT                  NOT NULL
  , SampleNumber                 INT                  NOT NULL
  , HitOrMiss                    VARCHAR(4)           NOT NULL
  , [RowCount]                   BIGINT               NOT NULL
  , StopwatchTicks               BIGINT               NOT NULL
  , ExecutionCpuMs               BIGINT               NOT NULL
  , ExecutionElapsedMs           BIGINT               NOT NULL
  , ParseAndCompileCpuMs         BIGINT               NOT NULL
  , ParseAndCompileElapsedMs     BIGINT               NOT NULL
  , CONSTRAINT PK_Benchmark_Sample                          PRIMARY KEY (SampleID)
  , CONSTRAINT FK_Benchmark_Sample_Benchmark_IndexClass     FOREIGN KEY (IndexClassID)  REFERENCES Benchmark.IndexClass (IndexClassID)
  , CONSTRAINT FK_Benchmark_Sample_Benchmark_QueryClass     FOREIGN KEY (QueryClassID)  REFERENCES Benchmark.QueryClass (QueryClassID)
);

CREATE TABLE Benchmark.SampleTableIo (
    SampleTableIoID              INT IDENTITY(1, 1)   NOT NULL
  , SampleID                     INT                  NOT NULL
  , TableName                    VARCHAR(50)          NOT NULL
  , ScanCount                    INT                      NULL
  , LogicalReads                 INT                      NULL
  , PhysicalReads                INT                      NULL
  , PageServerReads              INT                      NULL
  , ReadAheadReads               INT                      NULL
  , PageServerReadAheadReads     INT                      NULL
  , LobLogicalReads              INT                      NULL
  , LobPhysicalReads             INT                      NULL
  , LobPageServerReads           INT                      NULL
  , LobReadAheadReads            INT                      NULL
  , LobPageServerReadAheadReads  INT                      NULL
  , CONSTRAINT PK_Benchmark_SampleTableIo                   PRIMARY KEY (SampleTableIoID)
  , CONSTRAINT FK_Benchmark_SampleTableIo_Benchmark_Sample  FOREIGN KEY (SampleID)  REFERENCES Benchmark.Sample (SampleID)
);

CREATE UNIQUE INDEX UQ_Benchmark_IndexClass_Name
  ON Benchmark.IndexClass (Name);

CREATE UNIQUE INDEX UQ_Benchmark_QueryClass_Name
  ON Benchmark.QueryClass (Name);

CREATE UNIQUE INDEX UQ_Benchmark_TableIteration_IterationNumber_TableName
  ON Benchmark.TableIteration (IterationNumber, TableName);

CREATE UNIQUE INDEX UQ_Benchmark_Sample_IndexClassID_QueryClassID_IterationNumber_SampleNumber_HitOrMiss
  ON Benchmark.Sample (IndexClassID, QueryClassID, IterationNumber, SampleNumber, HitOrMiss);

CREATE UNIQUE INDEX UQ_Benchmark_SampleTableIo_SampleID_TableName
  ON Benchmark.SampleTableIo (SampleID, TableName);
    ";
  }
}