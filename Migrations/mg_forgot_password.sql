CREATE TABLE mg_forgot_password (
    forgot_password_id INT IDENTITY(1,1) PRIMARY KEY,
    forgot_password_member_id INT NOT NULL,
    forgot_password_email NVARCHAR(255) NOT NULL,
    forgot_password_app_key NVARCHAR(255) NOT NULL,
    forgot_password_token_hash NVARCHAR(255) NOT NULL UNIQUE,
    forgot_password_created_at DATETIME2 NOT NULL,
    forgot_password_expires_at DATETIME2 NOT NULL,
    forgot_password_consumed_at DATETIME2 NULL
);