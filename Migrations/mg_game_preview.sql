CREATE TABLE mg_game_preview(
    game_preview_id INT IDENTITY(1, 1) NOT NULL,
    game_preview_game_id int NOT NULL,
    game_preview_image NVARCHAR(MAX) NOT NULL,
    game_preview_title VARCHAR(100) NOT NULL,
    game_preview_description VARCHAR(250) NOT NULL,
    game_preview_order int NOT NULL,
    game_preview_datetime datetime2 NOT NULL,
);