-- Table with constraint
CREATE TABLE dbo.mg_play (
    play_id           BIGINT        IDENTITY(1,1) PRIMARY KEY,
    play_member_id    INT           NOT NULL,
    play_game_id    VARCHAR(100)  NOT NULL,
    play_created_at   DATETIME2(3)  NOT NULL,
    play_updated_at   DATETIME2(3)  NULL

    CONSTRAINT UQ_mg_play_member_game UNIQUE (play_member_id, play_game_id)
);

-- Index as separate statement
CREATE INDEX IX_mg_play_member_recent
    ON dbo.mg_play (play_member_id, play_created_at DESC)
    INCLUDE (play_game_id); 