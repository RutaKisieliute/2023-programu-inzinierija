using System;
using System.Collections.Generic;

namespace AALKisAPI.Models;

public partial class Keyword
{

    public string Name { get; set; } = null!;

    public int NoteId { get; set; }

    public virtual Note Note { get; set; } = null!;
}
