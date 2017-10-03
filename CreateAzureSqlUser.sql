USE LeaderboardNETCore
GO

CREATE LOGIN techdays2017 WITH PASSWORD='abc123!@'
CREATE USER techdays2017
	FOR LOGIN techdays2017
	WITH DEFAULT_SCHEMA = dbo
GO
-- Add user to the database owner role
EXEC sp_addrolemember N'db_owner', N'techdays2017'
GO
