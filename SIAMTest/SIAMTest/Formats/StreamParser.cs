using SIAMTest.Objects;
using System;
using System.Collections.Generic;
using System.IO;

namespace SIAMTest.Formats
{
    /// <summary>
    /// Исключение, возникающее при ошибке извлечения объектов из потока
    /// </summary>
    public class StreamParserException : Exception
    {
        public StreamParserException(string msg) : base(msg) { }
        public StreamParserException(string msg, Exception innerException) : base(msg, innerException) { }
    }

    /// <summary>
    /// Абстрактный класс для извлечения предопределённых объектов из потоков различного формата
    /// </summary>
    abstract public class StreamParser
    {
        // метод, извлекающий из потока объекты типа Order
        public abstract IEnumerable<Order> GetObjects(Stream stream);
    }
}
