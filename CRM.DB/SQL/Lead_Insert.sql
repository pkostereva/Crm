drop proc if exists dbo.Lead_Insert
go

create proc dbo.Lead_Insert
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
	insert into dbo.[Lead] (FirstName, LastName, Patronymic, BirthDate, Phone, Email, [Password], [Login], CityID, RegistrationDate, LastUpdateDate)
	values (@firstName, @lastName, @patronymic, @birthDate, @phone, @email, @password, @login, @cityId, getdate(), getdate())

	select SCOPE_IDENTITY()
end