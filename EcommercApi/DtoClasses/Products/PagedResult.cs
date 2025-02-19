using EcommercApi.DtoClasses;
using EcommercApi.Models;


public class PagedResult
{
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public List<Product> Items { get; set; }
}
