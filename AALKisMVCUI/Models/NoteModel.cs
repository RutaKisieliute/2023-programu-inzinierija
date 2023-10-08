namespace AALKisMVCUI.Models;

public class NoteModel {
    public string Content { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }

    public NoteModel()
    {
        Name = "Error";
        Author = "Error";
        Content = "Error";
    }
}