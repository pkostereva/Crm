drop proc if exists dbo.Lead_Update
go

create proc dbo.Lead_Update
@id bigint,
	@firstName nvarchar(100),
	@lastName nvarchar(100),
	@patronymic nvarchar(100) = NULL,
	@birthDate date,
	@phone nvarchar(30),
	@email nvarchar(100),
	@password nvarchar(100) = NULL,
	@login nvarchar(100) = NULL,
	@cityId int

as
begin
	update dbo.[Lead] 
	set FirstName = @firstName,
	 	LastName = @lastName,
	 	Patronymic = @patronymic,
	 	BirthDate = @birthDate,
	 	Phone = @phone,
	 	Email = @email,
	 	[Password] = @password,
	 	[Login] = @login,
	 	CityID = @cityId,
	 	LastUpdateDate = getdate()
	where  Id = @Id
end