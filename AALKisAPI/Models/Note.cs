using System;
using System.Collections.Generic;

namespace AALKisAPI.Models;

public partial class Note
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public sbyte? Flags { get; set; }

    public string? Content { get; set; }

    public int? FolderId { get; set; }

    public DateTime Modified { get; set; }

    public virtual Folder? Folder { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
