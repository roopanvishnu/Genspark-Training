using System;

namespace Notify.Models.DTOs;

public class HashDTO
{
    public string? Data { get; set; }
    public byte[]? HashedData { get; set; }
    public byte[]? HashKey { get; set; }
}
