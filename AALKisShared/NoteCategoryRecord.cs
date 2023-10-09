using System;
using System.Collections;
using System.Collections.Generic;

namespace AALKisShared;

public record class NoteCategoryRecord
{
    public string Name { get; set; }

    public List<NoteRecord> Notes { get; set; }

    public static NoteCategoryRecord FromDirectory(string path,
            bool readNoteContents = true)
    {
        string[] splitPath = path.Split(new char[] {'\\', '/'});
        return new NoteCategoryRecord {
            Name = splitPath[splitPath.Length - 1],
            Notes = (from filePath in Directory.GetFiles(path)
                 select NoteRecord.FromJsonFile(filePath, readNoteContents))
                .ToList()};
    }
}

