using System.Threading.Tasks;
using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Services;

public class NewsService
{
    private NewsRepo _newsRepo;
    public NewsService(NewsRepo newsRepo)
    {
        _newsRepo = newsRepo;
    }

    public async Task<List<News>> GetAll()
    {
        return (await _newsRepo.GetAll()).ToList();
    }
    public async Task<List<News>> GetPage(int? page)
    {
        var pageNumber = page ?? 1;
        var pageSize = 2;
        var news = (await _newsRepo.GetAll())
                            .OrderByDescending(x => x.NewsId)
                            .Skip(pageSize * (pageNumber - 1))
                            .Take(pageSize)
                            .ToList();
        return news;
    }

    public async Task<News> Get(int id)
    {
        var news = (await _newsRepo.Get(id));
        return news;
    }
    public async Task<News> Create([FromBody] NewsAddDTO newsDTO)
    {
        var news = new News
        {
            Content = newsDTO.Content,
            Image = newsDTO.Image,
            ShortDescription = newsDTO.ShortDescription,
            CreatedDate = newsDTO.CreatedDate,
            Status = newsDTO.Status,
            Title = newsDTO.Title,
            UserId = newsDTO.UserId
        };
        news = await _newsRepo.Add(news);
        return news;
    }
    public async Task<News> Edit(int id, [FromBody] NewsAddDTO newsDTO)
    {
        var news = await _newsRepo.Get(id);

        news.Content = newsDTO.Content;
        news.Image = newsDTO.Image;
        news.ShortDescription = newsDTO.ShortDescription;
        news.CreatedDate = newsDTO.CreatedDate;
        news.Status = newsDTO.Status;
        news.Title = newsDTO.Title;

        news = await _newsRepo.Update(id, news);
        return news;
    }

    public async Task<News> Delete(int id)
    {
        var news = await _newsRepo.Delete(id);
        return news;
    }
public async Task<News> Add(News news)
{
    return await _newsRepo.Add(news);
}



    public async Task<string> ExportContentToCSV()
    {
        var strw = new StringWriter();
        var listNews = (await _newsRepo.GetAll()).OrderBy(x => x.NewsId).ToList();
        foreach (var news in listNews)
        {
            strw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                          news.NewsId, news.Title, news.ShortDescription, news.CreatedDate, news.Status));
        }
        return strw.ToString();
    }
    
}