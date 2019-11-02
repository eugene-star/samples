using System;
using System.Collections.Generic;
using System.IO;

namespace SIAMTest.Loaders
{
    /// <summary>
    /// Исключение, возникающее при ошибке загрузки файла
    /// </summary>
    public class FileLoaderException : Exception
    {
        public FileLoaderException(string msg) : base(msg) { }
        public FileLoaderException(string msg, Exception innerException) : base(msg, innerException) { }
    }

    /// <summary>
    /// Абстрактный класс загрузчика файлов различными способами
    /// </summary>
    abstract public class FileLoader
    {
        protected readonly Uri _uri;
        
        public FileLoader(Uri uri)
        {
            _uri = uri;
        }

        public abstract Stream LoadFile();
    }
}
