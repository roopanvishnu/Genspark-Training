using System;
using FirstAPI.Models;

namespace FirstAPI.Interfaces;

public interface IEncryptionService
{
    EncryptModel EncryptData(EncryptModel data);
}
