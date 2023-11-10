namespace AALKisShared.Exceptions;

public class NoteException : Exception
{
    private NoteException() : base() { }

    public NoteException(string message) : base(message) { }

    public NoteException(Note record, string message) : base(
            message + "; " + record.ToString()
        ) { }
}
