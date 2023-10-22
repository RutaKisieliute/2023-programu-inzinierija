using AALKisShared;

namespace AALKisAPI.Services;

public interface INoteRecordsService
{
    public NoteRecord GetNote(int id, bool previewOnly);

    public bool CheckIfNoteExists(int id);

    public int CreateNote(int folderId, string noteTitle);

    public void DeleteNote(int id);

    public void UpdateNote(NoteRecord record, int folderId = -1);

    public List<NoteRecord> SearchByTitle(string searchQuery);
}
