-- Drop if exists (for iterative dev)
-- DROP TABLE IF EXISTS mg_favorite;

CREATE TABLE mg_favorite (
    favorite_id         BIGINT IDENTITY(1,1) PRIMARY KEY,
    favorite_member_id  INT          NOT NULL,
    favorite_game_id    INT          NOT NULL,
    favorite_created_at DATETIME2(3) NOT NULL,
    favorite_updated_at DATETIME2(3) NULL
);

CREATE UNIQUE INDEX UX_mg_favorite_member_game
ON mg_favorite (favorite_member_id, favorite_game_id);

-- Includes columns to make it covering for your SELECT.
CREATE INDEX IX_mg_favorite_member_created
ON mg_favorite (favorite_member_id, favorite_created_at DESC)
INCLUDE (favorite_id, favorite_game_id, favorite_updated_at);
