namespace EventManagement.Utilites
{
    public class PaginatedList<T>:List<T>
    {
        public int PageNumber { get; private set; }
        public int TotlaPages {  get; private set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotlaPages;
        public PaginatedList(List<T> items,int count,int pageNumber,int totlaPages)
        {
            PageNumber = pageNumber;
            TotlaPages =(int)Math.Ceiling(count/(double)totlaPages);

            AddRange(items);
        }

        public static PaginatedList<T> Create(IList<T> source,int pagenumber,int totalpages)
        {
            var count=source.Count();
            var items=source.Skip((pagenumber-1)*totalpages).Take(totalpages).ToList();

            return new PaginatedList<T>(items,count,pagenumber,totalpages);
        }

    }
}
