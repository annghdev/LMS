namespace LMS.Vocabulary.Entities;

public class Word
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Mean { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Example1 { get; set; } = string.Empty;
    public string Example2 { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public WordStatus Status { get; set; }

    public override string ToString()
    {
        return $"{Value} - {Mean}";
    }
}

public enum WordStatus
{
    Unread,
    NotRemember,
    UnsureRememember,
    Remembered
}
public enum Difficulty
{
    Easy,
    Medium,
    Hard,
}