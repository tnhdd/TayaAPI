namespace TayaAPI.Models
{
    public class Movement
    {
        public Guid Id { get; set; }
        public DateTime OperationDate { get; set; }
        public DateTime ValueDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; } 
        public Category Category { get; set; } =null!;
    }
}
