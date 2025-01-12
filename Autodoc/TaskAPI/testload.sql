declare @idTable table (id int);
insert Task (Name, Dt, Status)
	output inserted.id into @idTable
	values ('T1', sysdatetime(), 'Active');
declare @taskId int;
select @taskId = id from @idTable;

insert [File] (TaskId, Name) values (@taskId, '1.bin');
insert [File] (TaskId, Name) values (@taskId, '2.bin');

insert Task (Name, Dt, Status) values ('T2', sysdatetime(), 'New');

select * from Task t left join [File] f on f.TaskId=t.Id;