using TayaAPI.Models;

namespace TayaAPI.DTO.Response_Models
{
    public class PaginatedMovementsResponse
    {
        public List<Movement> Movements { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
