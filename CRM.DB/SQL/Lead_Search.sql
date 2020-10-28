drop proc if exists dbo.Lead_Search
go 
 
create proc dbo.Lead_Search 
		@Id bigint = null,
		@IdOperator int = null,
		@IdEnd int = null,
		@FirstName nvarchar(100) = null,
		@FirstNameOperator int = null,
		@LastName nvarchar(100) = null,
		@LastNameOperator int = null,
		@Patronymic nvarchar(100) = null,
		@PatronymicOperator int = null,
		@BirthDate datetime2(7) = null,
		@BirthDateOperator int = null,		
		@BirthDateDateEnd datetime2(7) = null,
		@Phone nvarchar(30) = null,
		@PhoneOperator int = null,
		@Email nvarchar(100) = null,
		@EmailOperator int = null,
		@Login nvarchar(100) = null,
		@LoginOperator int = null,
		@CityId int = null,
		@CityIdOperator int = null,
		@CitiesValues nvarchar(100) = null,
		@RegistrationDate datetime2(7) = null,
		@RegistrationDateOperator int = null,
		@RegistrationDateEnd datetime2(7) = null,
		@IsDeletedInclude bit = 0,
		@AccountId bigint = null
as 
begin 
declare @sql nvarchar(max),
		@OperatorId nvarchar(30),
		@OperatorFirstName nvarchar(30),
		@OperatorLastName nvarchar(30),
		@OperatorPatronymic nvarchar(30),
		@OperatorBirthDate nvarchar(30),
		@OperatorPhone nvarchar(30),
		@OperatorEmail nvarchar(30),
		@OperatorLogin nvarchar(30),
		@OperatorRegistrationDate nvarchar(30),
		@OperatorCityId nvarchar(30) = ' = '

set @OperatorId	= case @IdOperator
					when 1 then ' < '
					when 2 then ' > '
					else ' = '
					end					
set @OperatorFirstName	= case @FirstNameOperator
					when 3 then ' like '''
					else ' = '''
					end
set @OperatorLastName	= case @LastNameOperator
					when 3 then ' like '''
					else ' = '''
					end
set @OperatorPatronymic	= case @PatronymicOperator
					when 3 then ' like '''
					else ' = '''
					end
set @OperatorBirthDate = case @BirthDateOperator
					when 1 then ' < '''
					when 2 then ' > '''
					else ' = '''
					end
set @OperatorPhone	= case @PhoneOperator
					when 3 then ' like '''
					else ' = '''
					end
set @OperatorEmail	= case @EmailOperator
					when 3 then ' like '''
					else ' = '''
					end
set @OperatorRegistrationDate = case @RegistrationDateOperator
					when 1 then ' < '''
					when 2 then ' > '''
					else ' = '''
					end
set @OperatorLogin	= case @LoginOperator
					when 3 then ' like '''
					else ' = '''
					end

set @sql = N'select	
		l.Id,
		l.FirstName,
		l.LastName,
		l.Patronymic,
		l.BirthDate,
		l.Phone,
		l.Email,
		l.Login,
		l.RegistrationDate,
		l.LastUpdateDate,
		l.IsDeleted,
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
		inner join dbo.City c on c.Id = l.CityId
		left join dbo.Account a on a.LeadId = l.Id
		left join dbo.Currency cur on a.CurrencyId = cur.Id
		where '

	if @Id is not null and @IdEnd is null 
	set @sql=@sql + N'l.Id' + @OperatorId + CAST (@Id as nvarchar )+ ' and '  

	if @Id is not null and @IdEnd is not null 
	set @sql=@sql + N'l.Id between ' + CAST (@Id as nvarchar) + ' and ' + CAST (@IdEnd as nvarchar) + ' and '
	
	if @FirstName is not null
	set @sql=@sql + N'l.FirstName' + @OperatorFirstName + @FirstName + ''' and ' 

	if @LastName is not null
	set @sql=@sql + N'l.LastName'+ @OperatorLastName + @LastName + ''' and ' 

	if @Patronymic is not null
	set @sql=@sql + N'l.Patronymic'+ @OperatorPatronymic + @Patronymic + ''' and ' 

	if @Phone is not null 
	set @sql=@sql + N'l.Phone'+ @OperatorPhone + @Phone + ''' and ' 

	if @Email is not null 
	set @sql=@sql + N'l.Email' + @OperatorEmail + @Email + ''' and ' 
	 
	if @Login is not null 
	set @sql=@sql + N'l.Login'+ @OperatorLogin + @Login + ''' and ' 

	if @BirthDate is not null and @BirthDateDateEnd is null
	set @sql=@sql + N'l.BirthDate' + @OperatorBirthDate + CAST (@BirthDate as nvarchar) + ''' and '
	
	if @BirthDate is not null and @BirthDateDateEnd is not null 
	set @sql=@sql + N'l.BirthDate between ''' + CAST (@BirthDate as nvarchar) + ''' and ''' + CAST (@BirthDateDateEnd as nvarchar) + ''' and '
	
	if @RegistrationDate is not null and @RegistrationDateEnd is null 
	set @sql=@sql + N'l.RegistrationDate' + @OperatorRegistrationDate + CAST (@RegistrationDate as nvarchar) + ''' and '
	
	if @RegistrationDate is not null and @RegistrationDateEnd is not null 
	set @sql=@sql + N'l.RegistrationDate between ''' + CAST (@RegistrationDate as nvarchar) + ''' and ''' + CAST (@RegistrationDateEnd as nvarchar) + ''' and '	

	if @IsDeletedInclude = 0
	set @sql=@sql + N'l.IsDeleted = 0 and '

	if @IsDeletedInclude = 1
	set @sql=@sql + N' l.IsDeleted = 1 and '

	if @CityId is not null and @CitiesValues is null
	set @sql=@sql + N'c.Id'+ @OperatorCityId + CAST (@CityId as nvarchar) + ' and '
	
	if @CitiesValues is not null
	set @sql=@sql + N'c.Id in ('+ @CitiesValues +') and '
	
	if @AccountId is not null and @AccountId != 0
	set @sql=@sql + N'a.Id = ' + CAST (@AccountId as nvarchar) + ' and '

	set @sql=@sql + N'1 = 1'

	print @sql
	EXECUTE sp_executesql @SQL
end 

-- @Id
-- exec [dbo].[Lead_Search11] 1254280, null, 1254283    
-- exec [dbo].[Lead_Search11] 100, 1
-- exec [dbo].[Lead_Search11] 32
-- @FirstName
-- exec [dbo].[Lead_Search11] null, null, null, 'Бела'
-- exec [dbo].[Lead_Search11] null, null, null, '%на', 3 
-- @LastName
-- exec [dbo].[Lead_Search11] null, null, null, null, null, 'Файнс'
-- exec [dbo].[Lead_Search11] null, null, null, null, null, '%ов', 3, 
-- exec [dbo].[Lead_Search11] 1254200, 2, null, null, null, '%мова', 3 
-- @Patronymic
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, 'Алексеевна'
-- exec [dbo].[Lead_Search11] null, null, null, null,null, '%нн%', 3, 'Петрович', '1997-05-17', 2
-- exec [dbo].[Lead_Search11] 1254200, 2, null, null, null, '%мова', 3 , 'Михайловна'
--@BirthDate
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, '1956-03-10'
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, '1960-01-01', 1
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, '2001-01-01', 2
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, '1997-05-01', null, '1997-05-02'
--@Phone
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, null, null, null, '+7-923-2606848'
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, null, null, null, '%606848', 3
--@Email
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, null, null, null, null, null, 'blpgctV@gmail.com'
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, null, null, null, null, null, 'blpgct%', 3
--@Login
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 'Tcgplbyl25'
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 'Tcga%', 3
--@CityId
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 3
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, '1, 2'
--@RegistrationDate
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, '2020-04-20 00:52:15.0700000'
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, '2020-01-01', 2
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, '2018-12-20', null, '2020-04-19'

--@IsDeletedInclude
-- exec [dbo].[Lead_Search11] 32, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 1
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 1

--@AccountId
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,null, 48
-- exec [dbo].[Lead_Search11] null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 1, 787

-- set statistics time on exec [dbo].[Lead_Search11] 1200000, 2, null, null, null, '%ва', 3 , '%вна', 3, null, null, null, null, null, '%fgdgdgmail.com', 3, null, null, null, null, null, '2020-01-01', 2 set statistics time off

