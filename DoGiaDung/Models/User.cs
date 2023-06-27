using System;
using System.Collections.Generic;

namespace DoGiaDung.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Fullname { get; set; }

    public string? Name { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public int Gender { get; set; }

    public int Phone { get; set; }

    public string? Address { get; set; }

    public string? Image { get; set; }

    public int Access { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int UpdatedBy { get; set; }
}
