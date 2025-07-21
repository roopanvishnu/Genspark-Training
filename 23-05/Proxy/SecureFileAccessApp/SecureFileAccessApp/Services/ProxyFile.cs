using SecureFileAccessApp.Interfaces;
using SecureFileAccessApp.Models;
using System;

namespace SecureFileAccessApp.Services;

public class ProxyFile : IFile
{
    private readonly User _user;
    private readonly File _realFile;

    public ProxyFile(User user)
    {
        _user = user;
        _realFile = new File();
    }
    public void Read()
    {
        switch (_user.Role.ToLower())
        {
            case "admin":
                _realFile.Read();
                break;
            case "user":
                Console.WriteLine("[Limited Access] Showing metadata only...");
                break;
            case "guest":
                Console.WriteLine("[Access Denied] You do not have permission to read this file.");
                break;
            default:
                Console.WriteLine("[Access Denied] Invalid role.");
                break;
        }
    }
}