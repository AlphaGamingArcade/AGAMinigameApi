CREATE TABLE mg_recharge(
    recharge_id BIGINT IDENTITY(1, 1) NOT NULL,
    recharge_member_id INT NOT NULL,
    recharge_agent_id INT NOT NULL,
    recharge_gamemoney INT NOT NULL,
    recharge_currency INT NOT NULL,
    recharge_date  DATE NOT NULL,
    recharge_datetime DATETIME2 NOT NULL,
);