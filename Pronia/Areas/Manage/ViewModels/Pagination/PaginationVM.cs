namespace Pronia.Areas.Manage.ViewModels.Pagination;

public class PaginationVM<T>  where T : class
{
    public int CurrentPage { get; set; }
    public int TotalPageSize { get; set; }
    public List<T> Items { get; set; } = new List<T>();
    
}