SELECT member_id,
		member_agent_id,
        member_gamemoney, 
        member_charge_money, 
        member_exchange_money, 
        member_betting_money, 
        member_betting_benefit_money 
from mg_member where member_gamemoney != member_charge_money- member_exchange_money- member_betting_money + member_betting_benefit_money;
SELECT M.member_id, B.member_all_betting, B.member_all_benefit, A.agent_wallet FROM mg_member M LEFT JOIN (SELECT betting_member_id, SUM(betting_money) AS member_all_betting, SUM(betting_benefit) AS member_all_benefit FROM mg_betting GROUP BY betting_member_id) B ON (M.member_id = B.betting_member_id) LEFT JOIN mg_agent A ON (M.member_agent_id = A.agent_id) WHERE M.member_betting_money != B.member_all_betting;
SELECT M.member_id, B.member_all_betting, B.member_all_benefit, A.agent_wallet FROM mg_member M LEFT JOIN (SELECT betting_member_id, SUM(betting_money) AS member_all_betting, SUM(betting_benefit) AS member_all_benefit FROM mg_betting GROUP BY betting_member_id) B ON (M.member_id = B.betting_member_id) LEFT JOIN mg_agent A ON (M.member_agent_id = A.agent_id) WHERE M.member_betting_benefit_money != B.member_all_benefit;
SELECT *
FROM (
    SELECT 
        member_id,
		member_agent_id,
        member_gamemoney, 
        member_charge_money, 
        member_exchange_money, 
        member_betting_money, 
        member_betting_benefit_money,
        (member_charge_money - member_exchange_money - member_betting_money + member_betting_benefit_money) as current_money
    FROM mg_member 
) AS subquery
WHERE member_gamemoney != current_money