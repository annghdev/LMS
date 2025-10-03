using LMS.Vocabulary.Entities;

namespace LMS.Vocabulary.Interfaces;

public interface IWordRepository
{
    Task StartInTopicAsync(string topic);
    Task<IEnumerable<Word>> GetAllAsync();
    Task<Word?> GetByIdAsync(int id);
    Task<IEnumerable<Word>> FindByValueAsync(string value);
    Task<IEnumerable<Word>> FindByMeanAsync(string meaning);

    Task<int> AddAsync(Word word);
    Task<bool> UpdateAsync(Word word);
    Task<bool> RemoveAsync(int id);

    Task SaveChangesAsync();
}
