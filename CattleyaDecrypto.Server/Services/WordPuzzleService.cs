using System.Reflection;
using CattleyaDecrypto.Server.Services.Interfaces;

namespace CattleyaDecrypto.Server.Services;

public class WordPuzzleService : IWordPuzzleService
{
    private readonly List<string> Words;
    
    public WordPuzzleService()
    {
        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("CattleyaDecrypto.Server.Resources.short-wordlist.txt");
        using var reader = new StreamReader(stream!);
        Words = reader.ReadToEnd().Split("\r\n").Where(w => w.Length > 2).ToList();
    }
    
    public IEnumerable<string> PuzzleWords(int count)
    {
        return Enumerable.Range(0, count)
            .Select(i => Words[Random.Shared.Next(0, Words.Count)]);
    }
}