using AALKisShared;

namespace AALKisAPI.Services;

public interface IFolderRecordsService
{
    public List<FolderRecord<NoteRecord>> GetAllFolders(bool previewOnly);

    public FolderRecord<NoteRecord> GetFolder(string folderName, bool previewOnly);

    public bool CheckIfFolderExists(string folderName);

    public void CreateFolder(string folderName);

    public void DeleteFolder(string folderName, bool force);

    public void RenameFolder(string oldFolderName, string newFolderName);
}
