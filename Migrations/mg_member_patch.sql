-- Step 1: Add as nullable (so SQL Server accepts it)
ALTER TABLE mg_member
ADD member_email VARCHAR(255) NULL,
    member_email_status CHAR NULL,
    member_password VARCHAR(255) NULL,
    member_dob DATE NULL;

-- Step 2: Backfill values for existing rows
-- (replace with logic that assigns real emails/password hashes)
UPDATE mg_member
SET member_email = CONCAT('user', member_id, '@example.com'), 
    member_email_status = 'n',
    member_password = 'changeme', 
    member_dob = GETDATE()
WHERE member_email IS NULL;

-- Step 3: Alter to NOT NULL
ALTER TABLE mg_member
ALTER COLUMN member_email VARCHAR(255) NOT NULL;

ALTER TABLE mg_member
ALTER COLUMN member_email_status CHAR NOT NULL;

ALTER TABLE mg_member
ALTER COLUMN member_password VARCHAR(255) NOT NULL;

ALTER TABLE mg_member
ALTER COLUMN member_dob DATE NOT NULL;

ALTER TABLE mg_member
ALTER COLUMN member_password VARCHAR(255) NOT NULL;

-- Step 4: Add unique constraint on email
ALTER TABLE mg_member