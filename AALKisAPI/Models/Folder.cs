using System;
using System.Collections.Generic;

namespace AALKisAPI.Models;

public partial class Folder
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
}
