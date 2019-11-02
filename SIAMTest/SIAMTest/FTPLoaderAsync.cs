using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SIAMTest
{
    /// <summary>
    /// Состояние загрузки по FTP
    /// </summary>
    class FtpState
    {
        public FtpWebRequest Request { get; set; }
        public Exception OperationException { get; set; }
    }

    /// <summary>
    /// Класс загрузчика файлов с FTP
    /// </summary>
    class FTPLoader: FileLoader
    {
        FtpState _state = new FtpState();

        public override void LoadFile()
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri((string)_params["URL"]));
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                _state.Request = request;

                request.BeginGetRequestStream(new AsyncCallback(EndGetStreamCallback), null);

                if (_state.OperationException != null)
                {
                    throw _state.OperationException;
                }
            }
            catch (KeyNotFoundException ex)
            {
                throw new FileLoaderException("Не задан параметр URL для загрузки по FTP.", ex);
            }
            catch (Exception ex)
            {
                throw new FileLoaderException("Ошибка загрузки по FTP.", ex);
            }
        }

        private void EndGetStreamCallback(IAsyncResult ar)
        {
            try
            {
                Stream requestStream = _state.Request.EndGetRequestStream(ar);

                const int bufferLength = 2048;
                byte[] buffer = new byte[bufferLength];
                int count = 0, readBytes = 0;
                var stream = new MemoryStream();
                do
                {
                    readBytes = requestStream.Read(buffer, 0, bufferLength);
                    stream.Write(buffer, 0, readBytes);
                    count += readBytes;
                }
                while (readBytes != 0);

                requestStream.Close();

                _state.Request.BeginGetResponse(new AsyncCallback(EndGetResponseCallback), null);
            }
            catch (Exception e)
            {
                _state.OperationException = e;
                return;
            }
        }

        private void EndGetResponseCallback(IAsyncResult ar)
        {
            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse)_state.Request.EndGetResponse(ar);
                response.Close();
                _state.StatusDescription = response.StatusDescription;
            }
            catch (Exception e)
            {
                _state.OperationException = e;
            }
        }
    }
}
