CREATE TABLE dbo.mg_setting (
    setting_id            TINYINT       NOT NULL
        CONSTRAINT PK_mg_setting PRIMARY KEY
        CONSTRAINT CK_mg_setting_id CHECK (setting_id = 1),
    setting_charge_money  DECIMAL(19,2) NOT NULL
);

-- Seed once (optional)
IF NOT EXISTS (SELECT 1 FROM dbo.mg_setting WHERE setting_id = 1)
    INSERT INTO dbo.mg_setting (setting_id, setting_charge_money)
    VALUES (1, 0.00);