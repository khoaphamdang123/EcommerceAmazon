namespace Ecommerce_Product.Models;

public class PageList<T>
{
    public List<T> item;
    public int totalCount;
    public int pageSize;
    public int currentPage;
    public int totalPage;

    public PageList(List<T> _item,int total_count,int page_size,int pageNumber)
    {
        this.item=_item;
        this.totalCount=total_count;
        this.pageSize=page_size;
        this.currentPage=pageNumber;
        this.totalPage=(int)Math.Ceiling(total_count/(double)page_size);
    }

    public bool hasPreviousPage=>currentPage>1;

    public bool hasNextPage=>currentPage<totalPage;

    public static PageList<T> CreateItem(IQueryable<T> source,int pageNumber,int pageSize)
    {
        var count = source.Count();
        var items=source.Skip((pageNumber-1)*pageSize).Take(pageSize).ToList();
        return new PageList<T>(items,count,pageSize,pageNumber);
    }
}