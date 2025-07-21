namespace Singleton.Interfaces;

public interface IFileManager
{
    void WriteLine(string line);
    string ReadAll();
    void Close();
}