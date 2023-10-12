using AALKisShared;

namespace AALKisAPI.Services;

public class FileSystemRecordsService : IRecordsService
{
    private static readonly string baseDirectory = "./DataBase/Folders";

    public FileSystemRecordsService()
    {
        return;
    }

    public List<FolderRecord<NoteRecord>> GetAllFolders(bool previewOnly)
    {
            return Directory.GetDirectories(baseDirectory)
                .Select(directoryPath => {
                            var records = new FolderRecord<NoteRecord>();
                            records.SetFromDirectory(
                                    previewOnly: previewOnly,
                                    path: directoryPath);
                            return records;
                        })
                .ToList();
    }

    public FolderRecord<NoteRecord> GetFolder(string folderName, bool previewOnly)
    {
        var records = new FolderRecord<NoteRecord>();
        records.SetFromDirectory(
                previewOnly: previewOnly,
                path: $"{baseDirectory}/{folderName}");
        return records;
    }

    public NoteRecord GetNote(string folderName, string noteTitle, bool previewOnly)
    {
            var record = new NoteRecord();
            record.SetFromJsonFile($"{baseDirectory}/{folderName}/{noteTitle}.json");
            return record;
    }

    public bool CheckIfFolderExists(string folderName)
    {
        return Directory.Exists($"{baseDirectory}/{folderName}");
    }

    public bool CheckIfNoteExists(string folderName, string noteTitle)
    {
        return File.Exists($"{baseDirectory}/{folderName}/{noteTitle}.json");
    }

    public void CreateFolder(string folderName)
    {
        if(CheckIfFolderExists(folderName))
        {
            throw new DirectoryNotFoundException($"Folder {folderName} "
                    + "already exists.");
        }
        Directory.CreateDirectory($"{baseDirectory}/{folderName}");
        return;
    }

    public void CreateNote(string folderName, string noteTitle)
    {
        if(CheckIfNoteExists(folderName, noteTitle))
        {
            throw new IOException($"Note {noteTitle} in folder {folderName}"
                    + "already exists.");
        }
        new NoteRecord().ToJsonFile($"{baseDirectory}/{folderName}");
        return;
    }

    public void DeleteFolder(string folderName, bool recursive)
    {
        Directory.Delete(
                recursive: recursive,
                path: $"{baseDirectory}/{folderName}");
        return;
    }

    public void DeleteNote(string folderName, string noteTitle)
    {
        if(!CheckIfNoteExists(folderName, noteTitle))
        {
            throw new FileNotFoundException($"No note {noteTitle} in {folderName} "
                    + $"to delete.");
        }

        NoteRecord record = GetNote(folderName, noteTitle, previewOnly: true);
        if((record.Flags & NoteRecord.NoteFlags.MarkedForDeletion)
                == NoteRecord.NoteFlags.MarkedForDeletion)
        {
            System.IO.File.Delete($"{baseDirectory}/{folderName}/{noteTitle}.json");
            return;
        }
        record.Flags |= NoteRecord.NoteFlags.MarkedForDeletion;

        UpdateNote(folderName, record);
        return;
    }

    public void UpdateNote(string folderName, NoteRecord record)
    {
        if(!CheckIfNoteExists(folderName, record.Title))
        {
            throw new FileNotFoundException($"No note {record.Title} in {folderName} "
                    + $"to update.");
        }
        record.ToJsonFile($"{baseDirectory}/{folderName}");
        return;
    }
}
