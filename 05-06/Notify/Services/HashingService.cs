using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Notify.Models.DTOs;

namespace Notify.Services;

public class HashingService
{
    public HashDTO HashData(HashDTO dto)
    {
        if (dto.Data == null) throw new Exception("No data to hash");
        HMACSHA256 hMACSHA256;
        if (dto.HashKey != null)
        {
            hMACSHA256 = new HMACSHA256(dto.HashKey);
        }
        else
        {
            hMACSHA256 = new HMACSHA256();
        }
        dto.HashedData = hMACSHA256.ComputeHash(Encoding.UTF8.GetBytes(dto.Data));
        dto.HashKey = hMACSHA256.Key;
        return dto;
    }
}
