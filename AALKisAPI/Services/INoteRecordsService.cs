using AALKisShared;

namespace AALKisAPI.Services;

public interface INoteRecordsService
{
    public NoteRecord GetNote(string folderName, string noteTitle, bool previewOnly);

    public bool CheckIfNoteExists(string folderName, string noteTitle);

    public void CreateNote(string folderName, string noteTitle);

    public void DeleteNote(string folderName, string noteTitle);

    public void UpdateNote(string folderName, NoteRecord record);

    public List<NoteRecord> SearchByTitle(string searchQuery);
}
