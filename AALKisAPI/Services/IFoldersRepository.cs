using AALKisShared.Records;

namespace AALKisAPI.Services;

public interface IFoldersRepository
{
    public List<Folder<Note>> GetAllFolders(bool previewOnly);

    public Folder<Note> GetFolder(int id, bool previewOnly);

    public bool CheckIfFolderExists(int id);

    public int CreateFolder(string folderName);

    public void DeleteFolder(int id, bool force);

    public void RenameFolder(int id, string newFolderName);
}
