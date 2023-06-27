namespace DoGiaDung.Models
{
    public class UserImage
    {
        public string? Fullname { get; set; }

        public string? Name { get; set; }

        public string? Password { get; set; }

        public string? Email { get; set; }

        public int Gender { get; set; }

        public int Phone { get; set; }

        public string? Address { get; set; }

        public int Access { get; set; }

        public int Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public IFormFile? FileImage { get; set; }
    }
}
