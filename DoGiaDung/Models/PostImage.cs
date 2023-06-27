namespace DoGiaDung.Models
{
    public class PostImage
    {
        public int TopicId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Detail { get; set; }

        public string? Position { get; set; }

        public string? MetaKey { get; set; }

        public string? MetaDesc { get; set; }

        public int Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public IFormFile? FileImage { get; set; }
    }
}
