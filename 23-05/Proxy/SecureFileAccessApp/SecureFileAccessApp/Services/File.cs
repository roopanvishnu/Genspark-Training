using SecureFileAccessApp.Interfaces;
using System;

namespace SecureFileAccessApp.Services;

public class File : IFile
{
    public void Read()
    {
        Console.WriteLine("Access granted");
    }
}