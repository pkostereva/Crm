drop proc if exists dbo.Lead_SoftDeleteById
go

create proc dbo.Lead_SoftDeleteById
        @Id bigint
as
begin
	update dbo.Lead
	set IsDeleted = 1
	where Id = @Id
end

	