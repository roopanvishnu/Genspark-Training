using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FirstAPI.Interfaces;
using FirstAPI.Models;

namespace FirstAPI.Services
{
    public class EncryptionService : IEncryptionService
    {
        public virtual EncryptModel EncryptData(EncryptModel data)
        {
            if (data.Data == null) throw new Exception("No data is provided for encrytion.");
            HMACSHA256 hMACSHA256;
            if (data.HashKey != null)
            {
                hMACSHA256 = new HMACSHA256(data.HashKey);
            }
            else
            {
                hMACSHA256 = new HMACSHA256();
            }
            data.EncryptedData = hMACSHA256.ComputeHash(Encoding.UTF8.GetBytes(data.Data));
            data.HashKey = hMACSHA256.Key;
            return data;
        }

    }
}