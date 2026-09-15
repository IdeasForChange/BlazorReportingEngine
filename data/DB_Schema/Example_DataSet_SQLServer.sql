CREATE TABLE Reporting.TradeData
(
    TradeNo          INT            NOT NULL,
    BalanceSheet     VARCHAR(20)    NOT NULL,
    IFRS9            VARCHAR(100)   NOT NULL,
    InstrumentType   VARCHAR(200)   NOT NULL,
    CounterPartyType VARCHAR(100)   NOT NULL,
    SupraFlag        VARCHAR(3)     NOT NULL,
    FundFlag         VARCHAR(3)     NOT NULL,
    Total            DECIMAL(18,2)  NOT NULL,

    CONSTRAINT PK_TradeData
        PRIMARY KEY (TradeNo),

    CONSTRAINT CK_TradeData_SupraFlag
        CHECK (SupraFlag IN ('yes', 'no')),

    CONSTRAINT CK_TradeData_FundFlag
        CHECK (FundFlag IN ('yes', 'no'))
);

INSERT INTO Reporting.TradeData
(
    TradeNo,
    BalanceSheet,
    IFRS9,
    InstrumentType,
    CounterPartyType,
    SupraFlag,
    FundFlag,
    Total
)
VALUES
(
    1001,
    'Asset',
    'Amortised Cost',
    'Debt Instruments (and related hedges)',
    'Central Banks',
    'yes',
    'yes',
    1000.00
),
(
    1002,
    'Asset',
    'Amortised Cost',
    'Debt Instruments (and related hedges)',
    'General Governments',
    'no',
    'yes',
    2000.00
),
(
    1003,
    'Asset',
    'Amortised Cost',
    'Debt Instruments (and related hedges)',
    'Credit Institutions',
    'yes',
    'yes',
    3000.00
),
(
    1004,
    'Asset',
    'Amortised Cost',
    'Debt Instruments (and related hedges)',
    'Other Financial Corporations',
    'no',
    'yes',
    4000.00
),
(
    1005,
    'Asset',
    'Amortised Cost',
    'Debt Instruments (and related hedges)',
    'Non-Financial Corporations',
    'yes',
    'yes',
    5000.00
),
(
    1006,
    'Asset',
    'Amortised Cost',
    'Debt Instruments (and related hedges)',
    'Other',
    'no',
    'no',
    6000.00
),
(
    1007,
    'Asset',
    'Amortised Cost',
    'Debt Instruments (and related hedges)',
    'General Governments',
    'yes',
    'no',
    7000.00
),
(
    1008,
    'Asset',
    'Amortised Cost',
    'Debt Instruments (and related hedges)',
    'Credit Institutions',
    'no',
    'no',
    8000.00
),
(
    1009,
    'Asset',
    'Amortised Cost',
    'Debt Instruments (and related hedges)',
    'Other Financial Corporations',
    'yes',
    'no',
    9000.00
),
(
    1010,
    'Asset',
    'Amortised Cost',
    'Debt Instruments (and related hedges)',
    'Non-Financial Corporations',
    'no',
    'no',
    10000.00
);

SELECT
    TradeNo,
    BalanceSheet,
    IFRS9,
    InstrumentType,
    CounterPartyType,
    SupraFlag,
    FundFlag,
    Total
FROM Reporting.TradeData
ORDER BY TradeNo;