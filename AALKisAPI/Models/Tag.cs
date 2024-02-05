using System;
using System.Collections.Generic;

namespace AALKisAPI.Models;

public partial class Tag
{
    public int NoteId { get; set; }

    public string Tag1 { get; set; } = null!;

    public virtual Note Note { get; set; } = null!;
}
