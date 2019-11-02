using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace Guestbook
{
    public class SQLite : IMessagePersistor
    {
        private const string c_dbFileName = "messages.db3";

        private object _sqliteFactory;

        public bool SQLiteOK { get { return _sqliteFactory != null; } }

        public SQLite()
        {
            try
            {
                _sqliteFactory = DbProviderFactories.GetFactory("System.Data.SQLite");
                CreateDb();
            }
            catch (ArgumentException)
            {
                _sqliteFactory = null;
            }
        }

        private void CreateDb()
        {
            if (!File.Exists(c_dbFileName))
            {
                using (var conn = (SQLiteConnection)((SQLiteFactory)_sqliteFactory).CreateConnection())
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.ConnectionString = "Data Source = " + c_dbFileName;

                    SQLiteConnection.CreateFile(c_dbFileName);
                    conn.Open();

                    cmd.CommandText = @"CREATE TABLE [Users] (
						[Id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
						[Name] nvarchar(50) NOT NULL);";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE [Messages] (
						[Id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
						[UserId] integer NOT NULL,
						[Text] nvarcharchar(250) NOT NULL,
						CONSTRAINT [FK_Messages_UserId_Users_Id] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]));";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Message> GetMessages()
        {
            List<Message> messageList = new List<Message>();

            if (SQLiteOK)
            {
                using (var conn = (SQLiteConnection)((SQLiteFactory)_sqliteFactory).CreateConnection())
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.ConnectionString = "Data Source = " + c_dbFileName;

                    if (File.Exists(c_dbFileName))
                    {
                        conn.Open();
                        cmd.CommandText = @"
						SELECT U.[Name], M.[Text]
						FROM [Users] U JOIN [Messages] M ON M.[UserId]=U.[Id]";
                        using (SQLiteDataReader r = cmd.ExecuteReader(CommandBehavior.SingleResult))
                            while (r.Read())
                                messageList.Add(new Message()
                                {
                                    User = r.GetString(r.GetOrdinal("Name")),
                                    Text = r.GetString(r.GetOrdinal("Text"))
                                });
                    }
                }
            }

            return messageList;
        }

        public bool SaveMessage(Message m)
        {
            if (SQLiteOK)
            {
                using (var conn = (SQLiteConnection)((SQLiteFactory)_sqliteFactory).CreateConnection())
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.ConnectionString = "Data Source = " + c_dbFileName;
                    conn.Open();

                    int userId = 0;
                    for (int i = 0; i < 2 && userId == 0; i++)
                    {
                        // попробуем найти id пользователя
                        cmd.CommandText = "SELECT [Id] FROM [Users] WHERE [Name]=@Name";
                        cmd.Parameters.Add("@Name", DbType.String).Value = m.User;
                        using (SQLiteDataReader r = cmd.ExecuteReader(CommandBehavior.SingleRow))
                            if (r.Read())
                                userId = r.GetInt32(r.GetOrdinal("Id"));

                        // не нашли - добавим 
                        if (userId == 0)
                        {
                            cmd.CommandText = "INSERT INTO [Users]([Name]) VALUES (@Name)";
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // если всё в порядке с пользователем, добавим сообщение
                    if (userId > 0)
                    {
                        cmd.CommandText = "INSERT INTO [Messages]([UserId], [Text]) VALUES (@UserId, @Text)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@UserId", DbType.Int32).Value = userId;
                        cmd.Parameters.Add("@Text", DbType.String).Value = m.Text;
                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }

            return false;
        }
    }
}
