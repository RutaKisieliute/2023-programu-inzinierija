namespace AALKisMVCUI.Models;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class SearchViewModel
{
    public List<AALKisShared.Records.Note> Notes { get; set; }
    public string SearchQuery { get; set; }
}
