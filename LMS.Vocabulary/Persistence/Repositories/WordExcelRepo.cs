using ClosedXML.Excel;
using LMS.Vocabulary.Entities;
using LMS.Vocabulary.Interfaces;
using System.Reflection;

namespace LMS.Vocabulary.Persistence.Repositories;

public class WordExcelRepo : IWordRepository, IDisposable
{
    private readonly string _filePath;
    private readonly XLWorkbook _workbook;
    private IXLWorksheet _currentWorksheet;

    public WordExcelRepo()
    {
        // Lấy thư mục chứa mã nguồn của project Vocabulary
        var codeBase = Assembly.GetExecutingAssembly().CodeBase;
        var uri = new UriBuilder(codeBase);
        var path = Uri.UnescapeDataString(uri.Path);
        var assemblyDirectory = Path.GetDirectoryName(path);
        // Giả định project Vocabulary nằm cùng cấp với project Console trong thư mục LMS
        var projectRoot = Directory.GetParent(assemblyDirectory).Parent.Parent.Parent.FullName;
        var vocabularyProjectRoot = Path.Combine(projectRoot, "LMS.Vocabulary");
        _filePath = Path.Combine(vocabularyProjectRoot, "Persistence", "Data", "Vocabulary.xlsx");
        _filePath = Path.GetFullPath(_filePath); // Chuẩn hóa đường dẫn

        if (!File.Exists(_filePath))
        {
            throw new ArgumentException("File not found.");
        }
        else
        {
            _workbook = new XLWorkbook(_filePath);
        }
    }

    public async Task StartInTopicAsync(string topic)
    {
        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentException("Topic cannot be empty.", nameof(topic));

        _currentWorksheet = _workbook.Worksheets.FirstOrDefault(ws => ws.Name == topic);

        if (_currentWorksheet == null)
        {
            throw new ArgumentException("Topic sheet not found.", nameof(topic));
        }
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<Word>> GetAllAsync()
    {
        if (_currentWorksheet == null)
            throw new InvalidOperationException("No topic selected. Call StartInTopicAsync first.");

        var words = new List<Word>();
        var lastRow = _currentWorksheet.LastRowUsed()?.RowNumber() ?? 2;

        for (int row = 3; row <= lastRow; row++)
        {
            if (_currentWorksheet.Cell(row, 1).IsEmpty())
                continue;

            var word = new Word
            {
                Id = _currentWorksheet.Cell(row, 1).GetValue<int>(),
                Value = _currentWorksheet.Cell(row, 2).GetString(),
                Type = _currentWorksheet.Cell(row, 3).GetString(),
                Mean = _currentWorksheet.Cell(row, 4).GetString(),
                Notes = _currentWorksheet.Cell(row, 5).GetString(),
                Example1 = _currentWorksheet.Cell(row, 6).GetString(),
                Example2 = _currentWorksheet.Cell(row, 7).GetString(),
                Difficulty = Enum.Parse<Difficulty>(_currentWorksheet.Cell(row, 8).GetString()),
                Status = Enum.Parse<WordStatus>(_currentWorksheet.Cell(row, 9).GetString())
            };
            words.Add(word);
        }

        return words;
    }

    public async Task<Word?> GetByIdAsync(int id)
    {
        if (_currentWorksheet == null)
            throw new InvalidOperationException("No topic selected. Call StartInTopicAsync first.");

        var row = id + 2;
        if (_currentWorksheet.Cell(row, 1).IsEmpty())
            return null;

        var word = new Word
        {
            Id = _currentWorksheet.Cell(row, 1).GetValue<int>(),
            Value = _currentWorksheet.Cell(row, 2).GetString(),
            Type = _currentWorksheet.Cell(row, 3).GetString(),
            Mean = _currentWorksheet.Cell(row, 4).GetString(),
            Notes = _currentWorksheet.Cell(row, 5).GetString(),
            Example1 = _currentWorksheet.Cell(row, 6).GetString(),
            Example2 = _currentWorksheet.Cell(row, 7).GetString(),
            Difficulty = Enum.Parse<Difficulty>(_currentWorksheet.Cell(row, 8).GetString()),
            Status = Enum.Parse<WordStatus>(_currentWorksheet.Cell(row, 9).GetString())
        };

        return await Task.FromResult(word);
    }

    public async Task<IEnumerable<Word>> FindByValueAsync(string value)
    {
        if (_currentWorksheet == null)
            throw new InvalidOperationException("No topic selected. Call StartInTopicAsync first.");

        var words = new List<Word>();
        var lastRow = _currentWorksheet.LastRowUsed()?.RowNumber() ?? 2;

        for (int row = 3; row <= lastRow; row++)
        {
            if (_currentWorksheet.Cell(row, 2).GetString().Contains(value, StringComparison.OrdinalIgnoreCase))
            {
                var word = new Word
                {
                    Id = _currentWorksheet.Cell(row, 1).GetValue<int>(),
                    Value = _currentWorksheet.Cell(row, 2).GetString(),
                    Type = _currentWorksheet.Cell(row, 3).GetString(),
                    Mean = _currentWorksheet.Cell(row, 4).GetString(),
                    Notes = _currentWorksheet.Cell(row, 5).GetString(),
                    Example1 = _currentWorksheet.Cell(row, 6).GetString(),
                    Example2 = _currentWorksheet.Cell(row, 7).GetString(),
                    Difficulty = Enum.Parse<Difficulty>(_currentWorksheet.Cell(row, 8).GetString()),
                    Status = Enum.Parse<WordStatus>(_currentWorksheet.Cell(row, 9).GetString())
                };
                words.Add(word);
            }
        }

        return await Task.FromResult(words);
    }

    public async Task<IEnumerable<Word>> FindByMeanAsync(string meaning)
    {
        if (_currentWorksheet == null)
            throw new InvalidOperationException("No topic selected. Call StartInTopicAsync first.");

        var words = new List<Word>();
        var lastRow = _currentWorksheet.LastRowUsed()?.RowNumber() ?? 2;

        for (int row = 3; row <= lastRow; row++)
        {
            if (_currentWorksheet.Cell(row, 4).GetString().Contains(meaning, StringComparison.OrdinalIgnoreCase))
            {
                var word = new Word
                {
                    Id = _currentWorksheet.Cell(row, 1).GetValue<int>(),
                    Value = _currentWorksheet.Cell(row, 2).GetString(),
                    Type = _currentWorksheet.Cell(row, 3).GetString(),
                    Mean = _currentWorksheet.Cell(row, 4).GetString(),
                    Notes = _currentWorksheet.Cell(row, 5).GetString(),
                    Example1 = _currentWorksheet.Cell(row, 6).GetString(),
                    Example2 = _currentWorksheet.Cell(row, 7).GetString(),
                    Difficulty = Enum.Parse<Difficulty>(_currentWorksheet.Cell(row, 8).GetString()),
                    Status = Enum.Parse<WordStatus>(_currentWorksheet.Cell(row, 9).GetString())
                };
                words.Add(word);
            }
        }

        return await Task.FromResult(words);
    }

    public async Task<int> AddAsync(Word word)
    {
        if (_currentWorksheet == null)
            throw new InvalidOperationException("No topic selected. Call StartInTopicAsync first.");

        var lastRow = _currentWorksheet.LastRowUsed()?.RowNumber() ?? 2;
        int rowToUse = lastRow + 1;
        int newId = rowToUse - 2;

        _currentWorksheet.Cell(rowToUse, 1).Value = newId;
        _currentWorksheet.Cell(rowToUse, 2).Value = word.Value ?? string.Empty;
        _currentWorksheet.Cell(rowToUse, 3).Value = word.Type ?? string.Empty;
        _currentWorksheet.Cell(rowToUse, 4).Value = word.Mean ?? string.Empty;
        _currentWorksheet.Cell(rowToUse, 5).Value = word.Notes ?? string.Empty;
        _currentWorksheet.Cell(rowToUse, 6).Value = word.Example1 ?? string.Empty;
        _currentWorksheet.Cell(rowToUse, 7).Value = word.Example2 ?? string.Empty;
        _currentWorksheet.Cell(rowToUse, 8).Value = word.Difficulty.ToString();
        _currentWorksheet.Cell(rowToUse, 9).Value = word.Status.ToString();

        await SaveChangesAsync();
        return newId;
    }

    public async Task<bool> UpdateAsync(Word word)
    {
        if (_currentWorksheet == null)
            throw new InvalidOperationException("No topic selected. Call StartInTopicAsync first.");

        int rowToUse = word.Id + 2;
        if (_currentWorksheet.Cell(rowToUse, 1).IsEmpty())
            return false;

        _currentWorksheet.Cell(rowToUse, 2).Value = word.Value ?? string.Empty;
        _currentWorksheet.Cell(rowToUse, 3).Value = word.Type ?? string.Empty;
        _currentWorksheet.Cell(rowToUse, 4).Value = word.Mean ?? string.Empty;
        _currentWorksheet.Cell(rowToUse, 5).Value = word.Notes ?? string.Empty;
        _currentWorksheet.Cell(rowToUse, 6).Value = word.Example1 ?? string.Empty;
        _currentWorksheet.Cell(rowToUse, 7).Value = word.Example2 ?? string.Empty;
        _currentWorksheet.Cell(rowToUse, 8).Value = word.Difficulty.ToString();
        _currentWorksheet.Cell(rowToUse, 9).Value = word.Status.ToString();

        await SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveAsync(int id)
    {
        if (_currentWorksheet == null)
            throw new InvalidOperationException("No topic selected. Call StartInTopicAsync first.");

        int rowToRemove = id + 2;
        if (_currentWorksheet.Cell(rowToRemove, 1).IsEmpty())
            return false;

        _currentWorksheet.Row(rowToRemove).Clear();

        await SaveChangesAsync();
        return true;
    }

    public async Task SaveChangesAsync()
    {
        await Task.Run(() => _workbook.SaveAs(_filePath));
    }

    public void Dispose()
    {
        _workbook?.Dispose();
    }
}