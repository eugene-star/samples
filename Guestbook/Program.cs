using System;

namespace Guestbook
{
    class Program
    {
		static GuestbookProxyServer server;

        static void Main(string[] args)
        {
            Console.WriteLine("Guestbook Proxy Server. Press Ctrl-C to shutdown.");
			Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            try
            {
                server = new GuestbookProxyServer(Console.WriteLine);
                server.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Exception ix = ex.InnerException;
                while (ix != null)
                {
                    Console.WriteLine(ix.Message);
                    ix = ix.InnerException;
                }
                Console.WriteLine("Press any key...");
                Console.Read();
            }
        }

		/// <summary>
		/// Обработчик нажатия Ctrl-C
		/// </summary>
		static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			if (server != null)
			{
                e.Cancel = true;    // пусть консоль не убивает наше приложение
                server.Stop();      // вместо этого остановим сервер
			}
		}
    }
}