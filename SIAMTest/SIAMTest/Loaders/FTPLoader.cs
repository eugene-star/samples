using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SIAMTest.Loaders
{
    /// <summary>
    /// Загрузчик файлов по FTP
    /// </summary>
    class FTPLoader: FileLoader
    {
        public FTPLoader(Uri uri) : base(uri) { }

        public override Stream LoadFile()
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_uri);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    if (response.ContentLength > Int32.MaxValue)
                        throw new FileLoaderException("Файл слишком велик.");

                    using (Stream responseStream = response.GetResponseStream())
                    using (var sr = new StreamReader(responseStream))
                    {
                        int contentLength = (int)response.ContentLength;
                        var resultStream = new MemoryStream(contentLength > 0 ? contentLength : 4096);
                        var sw = new StreamWriter(resultStream) { AutoFlush = true };
                        sw.Write(sr.ReadToEnd());
                        return resultStream;
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                throw new FileLoaderException("Не задан параметр URL для загрузки по FTP.", ex);
            }
            catch(FileLoaderException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FileLoaderException("Ошибка загрузки по FTP.", ex);
            }
        }

    }
}
