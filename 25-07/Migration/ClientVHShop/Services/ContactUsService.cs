using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Services;

public class ContactUsService
{
    private ContactURepo _contactURepo;
    public ContactUsService(ContactURepo contactURepo)
    {
        _contactURepo = contactURepo;
    }

    public async Task<List<ContactU>> GetAll()
    {
        return (await _contactURepo.GetAll()).ToList();
    }
    public async Task<List<ContactU>> GetPage(int? page)
    {
        var pageNumber = page ?? 1;
        var pageSize = 2;
        var ContactU = (await _contactURepo.GetAll())
                            .OrderByDescending(x => x.id)
                            .Skip(pageSize * (pageNumber - 1))
                            .Take(pageSize)
                            .ToList();
        return ContactU;
    }

    public async Task<ContactU> Get(int id)
    {
        var ContactU = (await _contactURepo.Get(id));
        return ContactU;
    }
    public async Task<ContactU> Create([FromBody] ContactUsAddDTO contactUDTO)
    {
        var ContactU = new ContactU
        {
            content = contactUDTO.content,
            email = contactUDTO.email,
            name = contactUDTO.name,
            phone = contactUDTO.phone
        };
        ContactU = await _contactURepo.Add(ContactU);
        return ContactU;
    }
    public async Task<ContactU> Edit(int id, [FromBody] ContactUsAddDTO contactUDTO)
    {
        var ContactU = await _contactURepo.Get(id);
        
        ContactU.content = contactUDTO.content;
        ContactU.email = contactUDTO.email;
        ContactU.name = contactUDTO.name;
        ContactU.phone = contactUDTO.phone;

        ContactU = await _contactURepo.Update(id,ContactU);
        return ContactU;
    }

    public async Task<ContactU> Delete(int id)
    {
        var ContactU = await _contactURepo.Delete(id);
        return ContactU;
    }

}