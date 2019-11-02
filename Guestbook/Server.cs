/* Сервер HTTP с функциями гостевой книги и прокси
 *	GET /Guestbook/	- показ сообщений
 *	POST /Guestbook/ - добавление сообщения
 *	GET /Proxy/	- выдача запрашиваемого документа
 *	в остальных случаях выдаётся приветственное сообщение
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace Guestbook
{
    class GuestbookProxyServer
	{
		private const string PORT = "port";
		private const string PATH_GUESTBOOK = "/Guestbook/";
		private const string PATH_PROXY = "/Proxy/";

        private Guestbook _guestbook;
        
        private HttpListener _listener;
        private bool _stop = false;

		/// <summary>
		/// делегат для отправки сообщений вызывающему коду
		/// </summary>
		/// <param name="msg"></param>
		public delegate void Log(string msg);
		private Log _log;

        public GuestbookProxyServer(Log log)
        {
            IMessagePersistor persister;

            switch (ConfigurationManager.AppSettings["storage"])
            {
                case "xml":
                    persister = new XmlFile();
                    break;

                case "db":
                    persister = new SQLite();
                    break;

                default:
                    throw new ApplicationException("Invalid 'storage' configuration parameter value.");
            }

            _guestbook = new Guestbook(persister);

            int port;
            if (!int.TryParse(ConfigurationManager.AppSettings[PORT], out port))
                throw new ApplicationException("Invalid '" + PORT + "' configuration parameter value.");

            if (!HttpListener.IsSupported)
				throw new ApplicationException("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");

            _listener = new HttpListener();
            _listener.Prefixes.Add("http://+:" + port + "/");

        	_log = log;
         }

        /// <summary>
        /// Основной цикл получения и обработки запросов.
        /// </summary>
        public void Run()
        {
            if (_listener != null)
            {
                try
                {
                    _listener.Start();
                }
                catch (HttpListenerException ex)
                {
                    throw new Exception("Can't start HTTP listener", ex);
                }

                while (!_stop)
                    Process(_listener.GetContext());
            }
        }

        public void Stop()
		{
            _stop = true;
			_listener.Stop();
		}

        private void Process(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;

            _log(request.HttpMethod + " " + request.RawUrl);

             // содержимое ответа
            byte[] buffer = new byte[0];
            switch (request.Url.AbsolutePath)
            {
				case PATH_GUESTBOOK:
					if (request.HttpMethod.Equals("POST") && request.HasEntityBody)
                        buffer = HandlePOST(request);
                    else if (request.HttpMethod.Equals("GET"))
                    {
                        request.InputStream.Dispose();
                        buffer = HandleGET();
                    }
                    break;

                case PATH_PROXY:
                    {
                        string url = request.QueryString["url"];
                        request.InputStream.Dispose();
                        buffer = HandleProxy(url);
                    }
                    break;

                default:
                    request.InputStream.Dispose();
                    buffer = Encoding.Default.GetBytes("Helloworld!");
                    break;
            }

            using (HttpListenerResponse response = context.Response)
            {
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
        }

        private byte[] HandlePOST(HttpListenerRequest request)
        {
            byte[] buffer = new byte[0];

            using(request.InputStream)
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                string user = "", message = "",
                    requestBody = HttpUtility.UrlDecode(reader.ReadToEnd());

                foreach (string parameter in requestBody.Split('&'))
                {
                    string[] expr = parameter.Split('=');
                    if (expr[0].Equals("user") && expr.Length == 2)
                        user = expr[1];
                    if (expr[0].Equals("message") && expr.Length == 2)
                        message = expr[1];
                }

                if (user.Length > 0 && message.Length > 0)
                {
                    _guestbook.SaveMessage(new Message() { User = user, Text = message });
                    buffer = Encoding.Default.GetBytes("Message added.");
                }
            }

            return buffer;
        }

        private byte[] HandleGET()
        {
			var sb = new StringBuilder(@"<html>
<header>
    <meta http-equiv = ""content-type"" content = text/html; charset = Windows-1251> 
</header>
<body>
<table border=""1"">
    <tr>
        <td><h3>User</h3></td>
        <td><h3>Message</h3></td>
    </tr>");
            foreach (Message message in _guestbook.GetMessages())
				sb.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", message.User, message.Text);
			sb.Append("</table></body>");

            return Encoding.Default.GetBytes(sb.ToString());
        }

        private byte[] HandleProxy(string url)
		{
			if (url == null)
				return Encoding.Default.GetBytes("Missing url parameter.");

			if (!url.ToLower().StartsWith("http"))
				url = "http://" + url;

            string remoteContent;
            Encoding encoding = Encoding.Default;
			try
			{
				var requestRemote = (HttpWebRequest)WebRequest.Create(url);
                var responseRemote = requestRemote.GetResponse();

                if (responseRemote.ContentType.Contains("UTF-8"))
                    encoding = Encoding.GetEncoding("UTF-8");

                using (Stream dataStream = responseRemote.GetResponseStream())
				using (var reader = new StreamReader(dataStream))
					remoteContent = reader.ReadToEnd();
			}
			catch (Exception ex)
			{
                remoteContent = @"<html>
<header>
    <meta http-equiv = ""content-type"" content = text/html; charset = Windows-1251> 
</header>
<body>
    <h1>Proxy error</h1><p/>
    <i>" + url + "</i><p/>" 
    + ex.Message +
"</body>";
			}

            return encoding.GetBytes(remoteContent);
		}
    }
}
