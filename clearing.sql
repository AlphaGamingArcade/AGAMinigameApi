UPDATE [slot].[dbo].[mg_member] set member_gamemoney = 1000000, member_charge_money = 1000000, member_exchange_money = 0, member_betting_money=0, member_betting_benefit_money = 0 where agent_id = 1;
UPDATE [slot].[dbo].[mg_agent] set agent_money = 0 where agent_id = ;
TRUNCATE TABLE mg_betting where agent_id = ;