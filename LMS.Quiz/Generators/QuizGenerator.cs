using LMS.Vocabulary.Interfaces;
using LMS.Quiz.Models;

namespace LMS.Quiz.Generators;

public class QuizGenerator
{
    private readonly IWordRepository _repository;

    public QuizGenerator(IWordRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Tạo danh sách câu hỏi trắc nghiệm từ topic cụ thể.
    /// </summary>
    /// <param name="topic">Tên topic để load từ vựng.</param>
    /// <param name="numQuestions">Số lượng câu hỏi cần tạo (tối đa bằng số từ vựng có sẵn).</param>
    /// <param name="askForMeaning">true: hỏi nghĩa từ từ; false: hỏi từ từ nghĩa.</param>
    /// <param name="numOptions">Số lượng lựa chọn mỗi câu (mặc định 4, bao gồm 1 đúng).</param>
    /// <returns>Danh sách câu hỏi.</returns>
    public async Task<List<Question>> GenerateQuizAsync(string topic, int numQuestions = -1, bool askForMeaning = true, int numOptions = 4)
    {
        if (numOptions < 2)
            throw new ArgumentException("Number of options must be at least 2.", nameof(numOptions));

        // Load từ vựng từ repository
        await _repository.StartInTopicAsync(topic);
        var allWords = (await _repository.GetAllAsync()).ToList();

        if (numQuestions < 0)
            numQuestions = allWords.Count;

        if (allWords.Count < numQuestions)
            throw new InvalidOperationException($"Not enough words in topic '{topic}'. Available: {allWords.Count}, Requested: {numQuestions}");

        // Chọn ngẫu nhiên numQuestions từ vựng để tạo câu hỏi
        var random = new Random();
        var selectedWords = allWords.OrderBy(x => random.Next()).Take(numQuestions).ToList();

        var questions = new List<Question>();

        foreach (var word in selectedWords)
        {
            var question = new Question
            {
                IsAskingForMeaning = askForMeaning
            };

            string correctAnswer;
            if (askForMeaning)
            {
                // Hỏi nghĩa từ từ
                question.QuestionText = $"What is the meaning of '{word.Value}'?";
                correctAnswer = word.Mean;
            }
            else
            {
                // Hỏi từ từ nghĩa
                question.QuestionText = $"What word means '{word.Mean}'?";
                correctAnswer = word.Value;
            }

            // Lấy đáp án sai ngẫu nhiên (không trùng với đúng và không trùng nhau)
            var otherCandidates = allWords.Where(w => askForMeaning ? w.Mean != correctAnswer : w.Value != correctAnswer).ToList();
            var wrongAnswers = otherCandidates.OrderBy(x => random.Next()).Take(numOptions - 1)
                .Select(w => askForMeaning ? w.Mean : w.Value)
                .ToList();

            // Kết hợp đúng + sai, xáo trộn
            question.Options = wrongAnswers;
            question.Options.Add(correctAnswer);
            question.Options = question.Options.OrderBy(x => random.Next()).ToList();

            question.CorrectAnswer = correctAnswer;

            questions.Add(question);
        }

        return questions;
    }
}