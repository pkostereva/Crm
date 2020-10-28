drop proc if exists dbo.Lead_Merge
go

create proc dbo.Lead_Merge
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
merge 
	dbo.[Lead] l
using 
	(values (@Id)) n(Id) on l.Id = n.Id
when matched then 
   update set FirstName = @firstName,
	 			LastName = @lastName,
	 			Patronymic = @patronymic,
	 			BirthDate = @birthDate,
	 			Phone = @phone,
	 			Email = @email,
	 			[Password] = @password,
	 			[Login] = @login,
	 			CityID = @cityId,
	 			LastUpdateDate = getdate()
when not matched then 
	insert (FirstName, 
			LastName, 
			Patronymic, 
			BirthDate, 
			Phone, 
			Email, 
			[Password], 
			[Login], 
			CityID, 
			RegistrationDate, 
			LastUpdateDate)

	values (@firstName,
			@lastName, 
			@patronymic, 
			@birthDate, 
			@phone, 
			@email, 
			@password, 
			@login, 
			@cityId, 
			getdate(), 
			getdate());
			select scope_identity();
end


--exec Lead_Merge 32, 'Тина', 'Федорова', 'Анатольевна', '1976-10-09', '+7-905-876-5487', 'tina@gmail.com', null, 'tina999', 2

--exec Lead_Merge -1, 'Тина', 'Федорова', 'Анатольевна', '1976-10-09', '+7-909-876-5487', 'tin5a@gmail.com', null, 'ti6na999', 2