namespace ProductCatalog.Models
{
    public class ProductLog
    {
        public int Id { get; set; }

        //product Foreign Key 
        public int ProductId { get; set; }
        public string UpdatedByUserId { get; set; }

        public DateTime UpdateDateTime { get; set; } = DateTime.UtcNow;

        //Action ==> Addedd, Updated, or Deleted actions
        public string Action { get; set; }
    }
}
