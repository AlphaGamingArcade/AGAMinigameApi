-- Table with constraint
CREATE TABLE dbo.mg_app_play (
    app_play_id           BIGINT        IDENTITY(1,1) PRIMARY KEY,
    app_play_member_id    INT           NOT NULL,
    app_play_game_id    VARCHAR(100)  NOT NULL,
    app_play_created_at   DATETIME2(3)  NOT NULL,
    app_play_updated_at   DATETIME2(3)  NULL

    CONSTRAINT UQ_mg_app_play_member_game UNIQUE (app_play_member_id, app_play_game_id)
);

-- Index as separate statement
CREATE INDEX IX_mg_app_play_member_recent
    ON dbo.mg_app_play (app_play_member_id, app_play_created_at DESC)
    INCLUDE (app_play_game_id); 