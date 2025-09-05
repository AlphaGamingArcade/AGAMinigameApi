-- Step 1: Add as NULLable
ALTER TABLE mg_member
ADD member_email         VARCHAR(255) NULL,
    member_email_status  CHAR(1)      NULL,
    member_password      VARCHAR(255) NULL,
    member_dob           DATE         NULL;
    
BEGIN TRAN;
-- Step 2: Backfill existing rows (replace with real data/hashed password)
UPDATE mg_member
SET member_email        = CONCAT('user', member_id, '@agaglobal.com'),
    member_email_status = 'n',
    member_password     = 'changeme',          -- TODO: replace with a hash
    member_dob          = CAST(GETDATE() AS date)
WHERE member_email IS NULL;

-- Step 3: Make NOT NULL
ALTER TABLE dbo.mg_member ALTER COLUMN member_email        VARCHAR(255) NOT NULL;
ALTER TABLE dbo.mg_member ALTER COLUMN member_email_status CHAR(1)      NOT NULL;
ALTER TABLE dbo.mg_member ALTER COLUMN member_password     VARCHAR(255) NOT NULL;
ALTER TABLE dbo.mg_member ALTER COLUMN member_dob          DATE         NOT NULL;

-- Step 4: Add UNIQUE constraint on email
ALTER TABLE dbo.mg_member
  ADD CONSTRAINT UQ_mg_member_member_email UNIQUE (member_email);

-- (Optional) sensible defaults/validation for status
ALTER TABLE dbo.mg_member
  ADD CONSTRAINT DF_mg_member_email_status DEFAULT ('n') FOR member_email_status;

ALTER TABLE dbo.mg_member
  ADD CONSTRAINT CK_mg_member_email_status CHECK (member_email_status IN ('y','n'));

COMMIT;
