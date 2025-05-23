using System;
using System.IO;
using Singleton.Interfaces;

namespace Singleton.IO
{
    public class FileManager : IFileManager
    {
        private static FileManager instance;
        private static readonly object _lock = new object();

        private StreamWriter _Writer;
        private StreamReader _Reader;
        private string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.txt");

        private FileManager()
        {
            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath));

            // Open file once
            FileStream stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            _Writer = new StreamWriter(stream);
            _Reader = new StreamReader(stream);
            _Writer.AutoFlush = true;
        }

        public static FileManager Instance()
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = new FileManager(); // Constructor initializes _Writer and _Reader
                }
                return instance;
            }
        }

        public void WriteLine(string line)
        {
            _Writer.WriteLine(line);
        }

        public string ReadAll()
        {
            _Reader.BaseStream.Seek(0, SeekOrigin.Begin);
            _Reader.DiscardBufferedData();
            return _Reader.ReadToEnd();
        }

        public void Close()
        {
            _Writer?.Close();
            _Reader?.Close();
        }
    }
}
