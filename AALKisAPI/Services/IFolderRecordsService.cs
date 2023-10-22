using AALKisShared;

namespace AALKisAPI.Services;

public interface IFolderRecordsService
{
    public List<FolderRecord<NoteRecord>> GetAllFolders(bool previewOnly);

    public FolderRecord<NoteRecord> GetFolder(int id, bool previewOnly);

    public bool CheckIfFolderExists(int id);

    public void CreateFolder(string folderName);

    public void DeleteFolder(int id, bool force);

    public void RenameFolder(string oldFolderName, string newFolderName);
}
