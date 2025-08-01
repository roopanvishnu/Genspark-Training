using System.Threading.Tasks;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services;

public class ColorService
{
    private ColorRepo _colorRepo;
    public ColorService(ColorRepo colorRepo)
    {
        _colorRepo = colorRepo;
    }

    public async Task<List<Color>> GetAll()
    {
        return (await _colorRepo.GetAll()).ToList();
    }
    public async Task<Color> Get(int id)
    {
        return (await _colorRepo.Get(id));
    }
    public async Task<Color> Create(string name)
    {
        return (await _colorRepo.Add(new Color {Color1 = name}));
    }
    public async Task<Color> Edit(int id, string name)
    {
        Color color = await _colorRepo.Get(id);
        color.Color1 = name;
        color = await _colorRepo.Update(id, color);
        return color;
    }
    public async Task<Color> Delete(int id)
    {
        Color color = await _colorRepo.Delete(id);
        return color;
    }
}