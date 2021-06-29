/*
use master
drop database SocialMediaDB
*/

create database SocialMediaDB
go
use SocialMediaDB
go

create table tb_UserInfo(
	Id int identity(1, 1) not null primary key,
	UserId varchar(254) not null,
	Username nvarchar(50),
	Age int,
	Role nvarchar(254)
)