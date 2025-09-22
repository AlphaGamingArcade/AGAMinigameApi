CREATE TABLE mg_forgot_password (
    forgot_password_id INT IDENTITY(1,1) PRIMARY KEY,
    forgot_password_member_id INT NOT NULL,
    forgot_password_email NVARCHAR(255) NOT NULL,
    forgot_password_token NVARCHAR(255) NOT NULL UNIQUE,
    forgot_password_created_at DATETIME2 NOT NULL,
    forgot_password_expires_at DATETIME2 NOT NULL,
    forgot_password_is_used CHAR(1) NOT NULL,
    forgot_password_used_at DATETIME2 NULL
);