namespace Pronia.ViewModels;

public class HeaderVM
{
    public Dictionary<string,string> Settings { get; set;  } = new Dictionary<string, string>();
    
    public List<BasketItemVM> BasketItems { get; set;  } = new List<BasketItemVM>();
}