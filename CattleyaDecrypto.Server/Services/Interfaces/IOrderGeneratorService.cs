namespace CattleyaDecrypto.Server.Services.Interfaces;

public interface IOrderGeneratorService
{
    int[] GetRandomOrder(ref int number);
}