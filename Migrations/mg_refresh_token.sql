USE [slot]
GO

/****** Object:  Table [dbo].[mg_refresh_token]    Script Date: 05/09/2025 4:08:19 pm ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[mg_refresh_token](
	[refresh_token_id] [int] IDENTITY(1,1) NOT NULL,
	[refresh_token_member_id] [int] NOT NULL,
	[refresh_token_app_key] [varchar](50) NOT NULL,
	[refresh_token_token] [nvarchar](512) NOT NULL,
	[refresh_token_expires_at] [datetime] NOT NULL,
	[refresh_token_created_at] [datetime] NOT NULL,
	[refresh_token_revoked_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[refresh_token_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[mg_refresh_token] ADD  DEFAULT (getdate()) FOR [refresh_token_created_at]
GO