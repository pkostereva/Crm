drop proc if exists dbo.Account_DeleteById
go

create proc  dbo.Account_DeleteById
        @Id bigint
		
as
begin
	delete 
	from dbo.[Account]
	where Id = @Id 
end


	