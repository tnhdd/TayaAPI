namespace TayaAPI.DTO
{
    public class CreateMovementDto
    {
        public DateTime OperationDate { get; set; }
        public DateTime ValueDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }
}
