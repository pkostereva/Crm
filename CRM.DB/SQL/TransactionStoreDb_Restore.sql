drop proc if exists dbo.TransactionStoreDb_Restore
go

create proc dbo.TransactionStoreDb_Restore
as
begin

ALTER DATABASE TransactionStoreDb
SET SINGLE_USER WITH
ROLLBACK AFTER 30

BACKUP DATABASE [TransactionStoreDb] 
	TO  DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\Backup\TransactionStoreDb_backup2.BAK' 
	with INIT


ALTER DATABASE TransactionStoreDb SET MULTI_USER 

if exists(select * from sys.databases where name = 'TransactionStoreDb_Restored')
begin 
	ALTER DATABASE TransactionStoreDb_Restored
	SET SINGLE_USER WITH
	ROLLBACK AFTER 5 

	Drop database TransactionStoreDb_Restored

end

RESTORE DATABASE [TransactionStoreDb_Restored] FROM  DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\Backup\TransactionStoreDb_backup2.BAK'
WITH  FILE = 1,  MOVE N'TransactionStoreDb' TO N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\TransactionStoreDb_Restored.mdf',  
MOVE N'TransactionStoreDb_log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\TransactionStoreDb_Restored_log.ldf',
NOUNLOAD,  
REPLACE,
STATS = 5

ALTER DATABASE TransactionStoreDb_Restored SET MULTI_USER 

end

--exec dbo.TransactionStoreDb_Restore
