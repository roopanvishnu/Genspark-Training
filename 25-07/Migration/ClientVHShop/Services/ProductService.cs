using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Services;

public class ProductService
{
    private ProductRepo _productRepo;
    public ProductService(ProductRepo productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<List<Product>> GetAll()
    {
        return (await _productRepo.GetAll()).ToList();
    }
    public async Task<List<Product>> GetByFilter(int? page, int? category)
    {
        var pageNumber = page ?? 1;
        var pageSize = 2;
        var products = (await _productRepo.GetAll()).ToList();
        if (category != null)
        {
            products = products.Where(x => x.CategoryId == category).ToList();
        }
        products = products
                    .OrderByDescending(x => x.ProductId)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToList();
        return products;
    }

    public async Task<Product> Get(int id)
    {
        return (await _productRepo.Get(id));
    }

    public async Task<Product> Create([FromBody] ProductAddDTO productDTO)
    {
        var product = new Product
        {
            ProductName = productDTO.ProductName,
            Image = productDTO.Image,
            Price = productDTO.Price,
            UserId = productDTO.UserId,
            CategoryId = productDTO.CategoryId,
            ColorId = productDTO.ColorId,
            ModelId = productDTO.ModelId,
            StorageId = productDTO.StorageId,
            SellStartDate = productDTO.SellStartDate,
            SellEndDate = productDTO.SellEndDate,
            IsNew = productDTO.IsNew
        };
        product = await _productRepo.Add(product);
        return product;
    }
    public async Task<Product> Edit(int id, [FromBody] ProductAddDTO productDTO)
    {
        Product product = await _productRepo.Get(id);
        product.ProductName = productDTO.ProductName;
        product.Image = productDTO.Image;
        product.Price = productDTO.Price;
        product.UserId = productDTO.UserId;
        product.CategoryId = productDTO.CategoryId;
        product.ColorId = productDTO.ColorId;
        product.ModelId = productDTO.ModelId;
        product.StorageId = productDTO.StorageId;
        product.SellStartDate = productDTO.SellStartDate;
        product.SellEndDate = productDTO.SellEndDate;
        product.IsNew = productDTO.IsNew;


        product = await _productRepo.Update(id, product);
        return product;
    }
    public async Task<Product> Delete(int id)
    {
        Product product = await _productRepo.Delete(id);
        return product;
    }
    
}