CREATE TABLE dbo.mg_email_verify (
    email_verify_id           BIGINT IDENTITY(1,1) PRIMARY KEY,
    email_verify_member_id    INT            NOT NULL,
    email_verify_email        VARCHAR(255)   NOT NULL,
    email_verify_issuer       VARCHAR(64)    NULL,
    email_verify_token        VARBINARY(64)  NOT NULL UNIQUE,
    email_verify_purpose      VARCHAR(32)    NOT NULL DEFAULT 'email_verify',
    email_verify_created_at   DATETIME2   NOT NULL,  -- set from app
    email_verify_expires_at   DATETIME2   NOT NULL,  -- set from app
    email_verify_consumed_at  DATETIME2   NULL,
    email_verify_revoked_at   DATETIME2   NULL,
    email_verify_user_agent   NVARCHAR(256)  NULL,
    email_verify_ip_address   VARBINARY(16)  NULL
);