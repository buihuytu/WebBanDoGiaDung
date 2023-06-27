using Microsoft.AspNetCore.Http;

namespace DoGiaDung.Models
{
    public class SliderImage
    {
        public string? Name { get; set; }

        public string? Position { get; set; }

        public int Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public IFormFile? FileImage { get; set; }
    }
}
