namespace LMS.Quiz.Models;

public class Question
{
    public string QuestionText { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new List<string>();
    public string CorrectAnswer { get; set; } = string.Empty;
    public bool IsAskingForMeaning { get; set; } // true: hỏi nghĩa từ từ, false: hỏi từ từ nghĩa
}
