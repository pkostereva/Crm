drop proc if exists dbo.TransactionStoreDbBackup_Drop
go

create proc dbo.TransactionStoreDbBackup_Drop
as
begin
	drop database if exists TransactionStoreDb_Restored
end
go