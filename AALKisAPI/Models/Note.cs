using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AALKisAPI.Models;

public partial class Note
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Title { get; set; }

    public sbyte? Flags { get; set; }

    public string? Content { get; set; }

    public int? FolderId { get; set; }

    public DateTime Modified { get; set; }

    public virtual Folder? Folder { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public virtual ICollection<Keyword> Keywords { get; set; } = new List<Keyword>();
}
