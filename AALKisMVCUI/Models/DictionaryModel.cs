namespace AALKisMVCUI.Models;


public class DictionaryModel
{
    public int Id { get; set; }
public string Word { get; set; }
public string Meaning { get; set; }
    public DictionaryModel(int id, string word, string meaning)
    {
        Id = id;
        Word = word;
        Meaning = meaning;
    }
}
