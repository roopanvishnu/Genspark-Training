using Singleton.Interfaces;
using Singleton.IO;

namespace SingletonFileApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IFileManager fileManager = FileManager.Instance();

            fileManager.WriteLine("This is a test log at " + DateTime.Now);
            fileManager.WriteLine("Second entry.");

            Console.WriteLine("\nFile content:\n" + fileManager.ReadAll());

            fileManager.Close();
        }
    }
}
