insert Task (Name, Dt, Status) values ('T1', SYSDATETIME(), 'Active')
insert Task (Name, Dt, Status) values ('T2', SYSDATETIME(), 'New')

insert [File] (TaskId, Name) values (6, '1.bin')
insert [File] (TaskId, Name) values (6, '2.bin')

select * from Task t join [File] f on f.TaskId=t.Id