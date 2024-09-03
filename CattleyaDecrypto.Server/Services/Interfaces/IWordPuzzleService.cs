namespace CattleyaDecrypto.Server.Services.Interfaces;

public interface IWordPuzzleService
{
    IEnumerable<string> PuzzleWords(int count);
}