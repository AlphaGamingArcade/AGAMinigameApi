CREATE TABLE dbo.mg_user (
    user_member_id      INT           NOT NULL PRIMARY KEY,
    user_email          NVARCHAR(255) NOT NULL,
    user_password       VARCHAR(255)  NOT NULL, 
    user_email_status   CHAR(1)       NOT NULL DEFAULT ('n'),
    user_dob            DATE          NULL,
    user_created_at     DATETIME2(3)  NOT NULL,
    user_updated_at     DATETIME2(3)  NULL
);