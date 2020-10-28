drop proc if exists dbo.CrmDb_Restore
go

create proc dbo.CrmDb_Restore
as
begin

ALTER DATABASE CrmDb
SET SINGLE_USER WITH
ROLLBACK AFTER 60 --this will give your current connections 60 seconds to complete

BACKUP LOG [CrmDb] 
	TO  DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\Backup\CrmDb_LogBackup_2020-04-10_09-42-23.bak' 
	WITH NOFORMAT, 
	NOINIT,  
	NAME = N'CrmDb_LogBackup_2020-04-10_09-42-23', 
	NOSKIP, 
	NOREWIND, 
	NOUNLOAD,  
	NORECOVERY,  
	STATS = 5

RESTORE DATABASE CrmDb WITH RECOVERY
ALTER DATABASE CrmDb SET MULTI_USER

if exists(select * from sys.databases where name = 'CrmDb_Restored')
Drop database CrmDb_Restored

--Do Actual Restore
RESTORE DATABASE [CrmDb_Restored]
FROM  DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\Backup\CrmDb_backup.BAK'
WITH  FILE = 2,  MOVE N'CrmDb' TO N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\CrmDb_Restored.mdf',  
MOVE N'CrmDb_log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\CrmDb_Restored_log.ldf'

end