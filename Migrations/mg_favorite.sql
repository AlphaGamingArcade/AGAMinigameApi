CREATE TABLE mg_favorite (
    favorite_id          BIGINT IDENTITY(1,1) PRIMARY KEY,     -- internal ID
    favorite_member_id   INT          NOT NULL,                -- who favorited
    favorite_item_type   VARCHAR(32)  NOT NULL,                -- 'game', 'post', 'video', etc.
    favorite_item_id     INT          NOT NULL,                -- ID of the item being favorited
    favorite_created_at  DATETIME2(3) NOT NULL,
    favorite_updated_at  DATETIME2(3) NULL,
    -- Ensure a member can favorite an item only once
    CONSTRAINT uq_favorite UNIQUE (favorite_member_id, favorite_item_type, favorite_item_id)
);

-- Indexes for fast lookups
CREATE INDEX ix_favorite_member  ON mg_favorite(favorite_member_id, favorite_item_type);
CREATE INDEX ix_favorite_item    ON mg_favorite(favorite_item_type, favorite_item_id);
CREATE INDEX ix_favorite_created ON mg_favorite(favorite_created_at DESC);
CREATE INDEX ix_favorite_updated ON mg_favorite(favorite_updated_at DESC);