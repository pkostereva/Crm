drop proc if exists dbo.Lead_SelectByID
go

create proc dbo.Lead_SelectByID
		@Id bigint
as
begin
	select	
		l.Id,
		l.FirstName,
		l.LastName,
		l.Patronymic,
		l.BirthDate,
		l.Phone,
		l.Email,
		l.[Password],
		l.[Login],
		l.RegistrationDate,
		l.LastUpdateDate,
		c.Id,
		c.[Name],
		a.Id,
		a.LeadId,
		a.Balance,
		a.LastUpdateDate,
		a.IsDeleted,
		cur.Id, 
		cur.Code
	from dbo.[Lead] l
	left join dbo.[City] c on l.CityId = c.Id
	left join dbo.Account a on l.Id = a.LeadId
	left join dbo.Currency cur on a.CurrencyId = cur.Id
	where l.Id = @Id and l.IsDeleted = 0
end

-- exec Lead_SelectByID 32