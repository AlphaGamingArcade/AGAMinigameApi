CREATE TABLE mg_banner(
    banner_id INT IDENTITY(1, 1) NOT NULL,
    banner_title VARCHAR(100) NOT NULL,
    banner_description VARCHAR(250) NOT NULL,
    banner_image NVARCHAR(MAX) NOT NULL,
    banner_url NVARCHAR(MAX) NOT NULL,
    banner_order INT NOT NULL,
    banner_datetime datetime2 NOT NULL
);