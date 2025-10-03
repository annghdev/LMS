using LMS.Quiz.Generators;
using LMS.Vocabulary.Constants;
using LMS.Vocabulary.Persistence.Repositories;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

try
{
    // Khởi tạo repository và quiz generator
    using var repo = new WordExcelRepo();
    var generator = new QuizGenerator(repo);

    // Cấu hình bài test
    string topic = Topic.Actions;
    //int numQuestions = 140; // Số câu hỏi
    bool askForMeaning = true; // true: hỏi nghĩa từ từ; false: hỏi từ từ nghĩa

    // Tạo danh sách câu hỏi
    //var questions = await generator.GenerateQuizAsync(topic, numQuestions, askForMeaning);
    var questions = await generator.GenerateQuizAsync(topic, -1, askForMeaning);

    if (!questions.Any())
    {
        Console.WriteLine("No questions generated. Please check the topic or vocabulary data.");
        return;
    }

    Console.WriteLine($"Starting vocabulary quiz for topic '{topic}' with {questions.Count} questions.\n");

    int totalCorrect = 0;
    int totalAttempts = 0;

    // Duyệt qua từng câu hỏi
    for (int i = 0; i < questions.Count; i++)
    {
        var question = questions[i];
        bool answeredCorrectly = false;

        while (!answeredCorrectly)
        {
            // Hiển thị câu hỏi và lựa chọn
            Console.WriteLine($"Question {i + 1}: {question.QuestionText}");
            for (int j = 0; j < question.Options.Count; j++)
            {
                Console.WriteLine($"  {(char)('A' + j)}. {question.Options[j]}");
            }

            // Nhập đáp án từ người dùng
            Console.Write($"Enter your answer (A-{(char)('A' + question.Options.Count - 1)}): ");
            var userInput = Console.ReadLine()?.Trim().ToUpper();

            // Kiểm tra input hợp lệ
            if (string.IsNullOrEmpty(userInput) || userInput.Length != 1 || userInput[0] < 'A' || userInput[0] >= 'A' + question.Options.Count)
            {
                Console.WriteLine("Invalid input. Please enter a letter (e.g., A, B, C, D).\n");
                totalAttempts++;
                continue;
            }

            // Lấy đáp án người dùng chọn
            int selectedIndex = userInput[0] - 'A';
            string selectedAnswer = question.Options[selectedIndex];

            totalAttempts++;

            // Kiểm tra đáp án
            if (selectedAnswer == question.CorrectAnswer)
            {
                Console.WriteLine("Correct!\n");
                answeredCorrectly = true;
                totalCorrect++;
            }
            else
            {
                Console.WriteLine($"Incorrect. The correct answer is: {question.CorrectAnswer}. Try again.\n");
            }
        }
    }

    // Hiển thị kết quả
    Console.WriteLine("=== Quiz Results ===");
    Console.WriteLine($"Total Questions: {questions.Count}");
    Console.WriteLine($"Correct Answers: {totalCorrect}");
    Console.WriteLine($"Total Attempts: {totalAttempts}");
    Console.WriteLine($"Accuracy: {(double)totalCorrect / questions.Count * 100:F2}%");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}