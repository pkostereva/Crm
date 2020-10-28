drop proc if exists dbo.AddManyLeads
go

create proc dbo.AddManyLeads
as
begin

--создаем временную таблицу с данными
create table #RandomFirstNames
(id int,
 [name] varchar(100),
 gender char(1))

 create table #RandomSecondNames
(id int,
 [name] varchar(100),
 gender char(1))

 create table #RandomPatronymic
(id int,
 [name] nvarchar(100),
 gender char(1))

 insert into #RandomFirstNames
(id, [name], gender)
select 1,'Петя','М' union
select 2,'Василий','М' union
select 3,'Иван','М' union
select 4,'Григорий','М' union
select 5,'Захар','М' union
select 6,'Аркадий','М' union
select 7,'Мухамед','М' union
select 8,'Алексей','М' union
select 9,'Минчжу','М' union
select 10,'Эдуард','М' union
select 11,'Анна','Ж' union
select 12,'Анастасия','Ж' union
select 13,'Клавдия','Ж' union
select 14,'Виктория','Ж' union
select 15,'Зинаида','Ж' union
select 16,'Алиса','Ж' union
select 17,'Варвара','Ж' union
select 18,'Бела','Ж' union
select 19,'Лара','Ж' union
select 20,'Оксана','Ж'

 insert into #RandomSecondNames
(id, [name], gender)
select 1,'Абрамович','М' union
select 2,'Песков','М' union
select 3,'Адамс','М' union
select 4,'Джонсон','М' union
select 5,'Леонов','М' union
select 6,'Фадеев','М' union
select 7,'Ли','М' union
select 8,'Глазунов','М' union
select 9,'Немцов','М' union
select 10,'Дарвин','М' union
select 11,'Аверьянова','Ж' union
select 12,'Васильева','Ж' union
select 13,'Любимова','Ж' union
select 14,'Файнс','Ж' union
select 15,'Образцова','Ж' union
select 16,'Грин','Ж' union
select 17,'Гумилёва','Ж' union
select 18,'Белова','Ж' union
select 19,'Смышляева','Ж' union
select 20,'Собчак','Ж'

 insert into #RandomPatronymic
(id, [name], gender)
select 1,'Иванович','М' union
select 2,'Макарович','М' union
select 3,'Львович','М' union
select 4,'Кириллович','М' union
select 5,'Викторович','М' union
select 6,'Антонович','М' union
select 7,'Борисович','М' union
select 8,'Петрович','М' union
select 9,'Яковлевич','М' union
select 10,'Егорович','М' union
select 11,'Олеговна','Ж' union
select 12,'Степановна','Ж' union
select 13,'Виталиевна','Ж' union
select 14,'Артемовна','Ж' union
select 15,'Алексеевна','Ж' union
select 16,'Богдановна','Ж' union
select 17,'Михайловна','Ж' union
select 18,'Николаевна','Ж' union
select 19,'Ярославовна','Ж' union
select 20,'Александровна','Ж'

--тут генерируются данные и добавляются в базу
declare @counter int,
		@createData datetime,
		@symbols nvarchar(100),
		@gender char(1),
		@numbersInTable int,
		@firstName nvarchar(100),
		@secondName nvarchar(100),
		@patronymic nvarchar(100),
		@randomLogin nvarchar(10),
		@randomLoginLength int,
		@randomLoginCounter int;

set @counter = 0;
set @gender = 'Ж';

while @counter < 10000
begin

set @firstName = (select top 1 [Name]
from #RandomFirstNames
where gender = @gender
order by newid()
);

set @secondName = (select top 1 [Name]
from #RandomSecondNames
where gender = @gender
order by newid()
);

set @patronymic = (select top 1 [Name]
from #RandomPatronymic
where gender = @gender
order by newid()
);

set @randomLogin = char(rand()*(90-65)+65)
set @randomLoginLength = rand()*(9-4)+4
set @randomLoginCounter = 0
while @randomLoginCounter < @randomLoginLength
begin
	set @randomLogin = @randomLogin + char(rand()*(122-97)+97)
	set @randomLoginCounter = @randomLoginCounter + 1;
end;
set @randomLogin = @randomLogin + char(rand()*(57-48)+48) + char(rand()*(57-48)+48)

set @createData = dateadd(day, rand()*(2650-1)+1, '2010-01-01');

insert into [dbo].[Lead]
           ([FirstName]
           ,[LastName]
           ,[Patronymic]
           ,[BirthDate]
           ,[Phone]
           ,[Email]
           ,[Login]
           ,[CityId]
           ,[RegistrationDate]
           ,[LastUpdateDate])
     values
           (@firstName,
		   @secondName,
		   @patronymic,
		   dateadd(day, rand()*(10000-1)+1, '1970-01-01'),
		   '+7-'+ cast(floor(rand()*(999-900)+900) as nvarchar(15))+'-' + cast(floor(rand()*(999999-100000)+100000) as nvarchar(15)) + cast(floor(rand()*9) as nvarchar(2)),
		   reverse(lower(substring(@randomLogin,0,@randomLoginLength)))+'@yahoo.com',
		   @randomLogin,
		   rand()*(4-1)+1,
		   @createData,
		   dateadd(day, rand()*(365-1)+1, @createData))
	 set @counter = @counter + 1
	 end;
end

--exec [dbo].[AddManyLeads]