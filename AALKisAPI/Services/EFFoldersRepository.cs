using AALKisShared;
using AALKisAPI.Data;
using AALKisAPI.Models;
using Note = AALKisShared.Records.Note;
using NoteEntity = AALKisAPI.Models.Note;
using Folder = AALKisShared.Records.Folder<AALKisShared.Records.Note>;
using FolderEntity = AALKisAPI.Models.Folder;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;

namespace AALKisAPI.Services;

public class EFFoldersRepository : IFoldersRepository
{
    private readonly NoteDB _database;
    
    public EFFoldersRepository(NoteDB database)
    {
        _database = database;
    }
    
    public List<Folder> GetAllFolders(int userId, bool previewOnly)
    {
        
        IEnumerable<FolderEntity> list1; 
        if(previewOnly)
        {
            list1 = _database.Folders.AsEnumerable();
        }
        else
        {
            list1 = _database.Folders.Where(x => x.UserId == userId)
                .Include(o => o.Notes).AsEnumerable();
        }
        List<Folder> list2 = new List<Folder>();
        foreach(FolderEntity folder in list1) list2.Add(ToSharedFolder(folder));
        return list2;
    }

    public Folder GetFolder(int id, bool previewOnly)
    {
        if(previewOnly)
        {
            return ToSharedFolder(_database.Folders.Single(b => b.Id == id));
        }
        return ToSharedFolder(_database.Folders.Include(a => a.Notes).Single(b => b.Id == id));
    }

    public bool CheckIfFolderExists(int id)
    {
        return _database.Folders.Find(id) != null;
    }

    public int CreateFolder(string folderName, int userId)
    {
        FolderEntity folder = new FolderEntity(){
            Title = folderName,
            UserId = userId
        };
        _database.Folders.Add(folder);
        _database.SaveChanges();
        return folder.Id;
    }

    public void DeleteFolder(int id, bool force)
    {
        FolderEntity? folder = _database.Folders.Find(id);
        if(folder == null) return;
        if(force)
        {
            _database.Notes.RemoveRange(_database.Notes.Where(x => x.FolderId == id));
        }
        _database.Folders.Remove(folder);
        _database.SaveChanges();
    }

    public void RenameFolder(int id, string newFolderName)
    {
        FolderEntity? folder = _database.Folders.Find(id);
        if(folder == null) return;
        folder.Title = newFolderName;
        _database.Folders.Update(folder);
        _database.SaveChanges();
    }

    public static Folder ToSharedFolder(FolderEntity? entity)
    {
        if(entity == null) return new Folder();
        List<Note> list = new List<Note>();
        foreach(NoteEntity note in entity.Notes)
        {
            list.Add(EFNotesRepository.ToSharedNote(note)); 
        }
        return new Folder(){
            Id = entity.Id,
            Name = entity.Title ?? "",
            Records = list
        };
    }
}
