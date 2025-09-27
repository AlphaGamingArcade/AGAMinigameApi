CREATE TABLE dbo.mg_app_play (
    app_play_member_id      INT           NOT NULL,
    app_play_game_code      VARCHAR(100)  NOT NULL,
    app_play_created_at     DATETIME2(3)  NOT NULL,
    app_play_updated_at     DATETIME2(3)  NULL
);