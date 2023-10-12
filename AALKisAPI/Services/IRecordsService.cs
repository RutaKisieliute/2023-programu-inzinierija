using AALKisShared;

namespace AALKisAPI.Services;

public interface IRecordsService
{
    public List<FolderRecord<NoteRecord>> GetAllFolders(bool previewOnly);

    public FolderRecord<NoteRecord> GetFolder(string folderName, bool previewOnly);

    public NoteRecord GetNote(string folderName, string noteTitle, bool previewOnly);

    public bool CheckIfFolderExists(string folderName);

    public bool CheckIfNoteExists(string folderName, string noteTitle);

    public void CreateFolder(string folderName);

    public void CreateNote(string folderName, string noteTitle);

    public void DeleteFolder(string folderName, bool recursive);

    public void DeleteNote(string folderName, string noteTitle);

    public void UpdateNote(string folderName, NoteRecord record);
}
