CREATE TABLE dbo.mg_app_user (
    app_user_member_id      INT           NOT NULL PRIMARY KEY,
    app_user_email          NVARCHAR(255) NOT NULL,
    app_user_password       VARCHAR(255)  NOT NULL, 
    app_user_email_status   CHAR(1)       NOT NULL DEFAULT ('n'),
    app_user_dob            DATE          NULL,
    app_user_created_at     DATETIME2(3)  NOT NULL,
    app_user_updated_at     DATETIME2(3)  NULL
);