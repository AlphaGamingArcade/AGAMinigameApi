CREATE TABLE mg_app_game(
    game_id INT IDENTITY(1, 1) NOT NULL,
    game_code VARCHAR(100) NOT NULL,
    game_name VARCHAR(100) NOT NULL,
    game_description VARCHAR(250) NOT NULL,
    game_image NVARCHAR(MAX) NOT NULL,
    game_url NVARCHAR(MAX) NOT NULL,
    game_status CHAR(1) NOT NULL,
    game_category VARCHAR(100) NOT NULL,
    game_top CHAR(1) NOT NULL,
    game_latest CHAR(1) NOT NULL,
    game_trending CHAR(1) NOT NULL,
    game_datetime datetime2 NOT NULL,
);