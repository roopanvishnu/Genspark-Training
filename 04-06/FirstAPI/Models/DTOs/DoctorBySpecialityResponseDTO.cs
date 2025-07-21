using System;

namespace FirstAPI.Models.DTOs;

public class DoctorBySpecialityResponseDTO
{
    public int Id { get; set; }
    public string Dname { get; set; } = string.Empty;
    public float Yoe { get; set; }
}
