CREATE TABLE mg_email_verify (
    email_verify_id           BIGINT IDENTITY(1,1) PRIMARY KEY,
    email_verify_member_id    INT            NOT NULL,
    email_verify_email        VARCHAR(255)   NOT NULL,
    email_verify_app_key       VARCHAR(64)   NULL,
    email_verify_token_hash   VARCHAR(64)  NOT NULL UNIQUE,
    email_verify_purpose      VARCHAR(32)    NOT NULL,
    email_verify_created_at   DATETIME2   NOT NULL,
    email_verify_expires_at   DATETIME2   NOT NULL,  -- set from app
    email_verify_consumed_at  DATETIME2   NULL
);