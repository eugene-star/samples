using System;
using System.IO;

namespace SIAMTest.Loaders
{
    /// <summary>
    /// Загрузчик файлов с сетевого ресурса
    /// </summary>
    public class UNCLoader : FileLoader
    {
        public UNCLoader(Uri uri) : base(uri) { }

        public override System.IO.Stream LoadFile()
        {
            try
            {
                using (var fileStream = new FileStream(_uri.OriginalString, FileMode.Open))
                using (var sr = new StreamReader(fileStream))
                {
                    if (fileStream.Length > Int32.MaxValue)
                        throw new FileLoaderException("Файл слишком велик.");

                    var resultStream = new MemoryStream((int)fileStream.Length);
                    var sw = new StreamWriter(resultStream) { AutoFlush = true };
                    sw.Write(sr.ReadToEnd());
                    return resultStream;
                }
            }
            catch (FileLoaderException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FileLoaderException("Ошибка загрузки файла с сетевого ресурса.", ex);
            }
        }
    }
}
