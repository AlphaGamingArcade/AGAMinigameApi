CREATE TABLE mg_charge(
    charge_id BIGINT IDENTITY(1, 1) NOT NULL,
    charge_member_id INT NOT NULL,
    charge_agent_id INT NOT NULL,
    charge_gamemoney INT NOT NULL,
    charge_currency VARCHAR(5) NOT NULL,
    charge_date  DATE NOT NULL,
    charge_datetime DATETIME2 NOT NULL,
);