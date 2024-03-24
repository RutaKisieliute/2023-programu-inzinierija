using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AALKisAPI.Models;

public partial class Folder
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Title { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
}
