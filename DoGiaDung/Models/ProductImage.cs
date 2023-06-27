namespace DoGiaDung.Models
{
    public class ProductImage
    {
        public string Name { get; set; } = null!;

        public int CateId { get; set; }

        public string? NewPromotion { get; set; }

        public int Installment { get; set; }

        public int Discount { get; set; }

        public string Detail { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Specification { get; set; } = null!;

        public double Price { get; set; }

        public int Quantity { get; set; }

        public double ProPrice { get; set; }

        public string? MetaKey { get; set; }

        public string? MetaDesc { get; set; }

        public int Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public IFormFile? FileImage { get; set; }
    }
}
