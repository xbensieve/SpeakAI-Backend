namespace Common.DTO.Query
{
    public class QueryStringParameters
    {
        const int maxPageSize = 100;
        public int PageNumber { get; set; }

        private int _pageSize;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value > maxPageSize ? maxPageSize : value;
            }
        }
    }
}
