using System.Threading.Tasks;
using ChienVHShopOnline.Contexts;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services;

public class CategoryService
{
    private CategoryRepo _categoryRepo;
    public CategoryService(CategoryRepo categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    public async Task<List<Category>> GetPage(int? page)
    {
        int pageNumber = page ?? 1;
        int pageSize = 5;
        var catList = (await _categoryRepo.GetAll()).OrderBy(x => x.Name);
        var list = catList.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
        return list;
    }
    public async Task<List<Category>> GetAll()
    {
        var catList = (await _categoryRepo.GetAll()).OrderBy(x => x.Name).ToList();
        return catList;
    }
    public async Task<Category> Create(string name)
    {
        Category category = new Category { Name = name };
        category = await _categoryRepo.Add(category);
        return category;
    }
    public async Task<Category> Edit(int id, string name)
    {
        Category category = await _categoryRepo.Get(id);
        category.Name = name;
        category = await _categoryRepo.Update(id, category);
        return category;
    }

    public async Task<Category> Get(int id)
    {
        Category category = await _categoryRepo.Get(id);
        return category;
    }
    public async Task<Category> Delete(int id)
    {
        Category category = await _categoryRepo.Delete(id);
        return category;
    }
}