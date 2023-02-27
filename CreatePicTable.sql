use Library01
create table Pictures
(
	Id int not null identity(1,1) primary key,
	BookId int not null,
	foreign key (BookId) references Books (Id),
	SmallPicture varbinary (Max),
	Picture varbinary (Max)
)
	