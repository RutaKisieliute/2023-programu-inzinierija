using AALKisShared.Records;

namespace AALKisAPI.Services;

public interface INotesRepository
{
    public IEnumerable<Note> SearchNotes(string query, int userId);

    public IEnumerable<Note> GetAllNotes();
    
    public Note GetNote(int id, bool previewOnly);

    public bool CheckIfNoteExists(int id);

    public int? CreateNote(int folderId, string noteTitle, string content, int userId);

    public void DeleteNote(int id);

    public void UpdateNote(Note record, int folderId = -1);

    public event EventHandler<Note> NoteCreated;
}
