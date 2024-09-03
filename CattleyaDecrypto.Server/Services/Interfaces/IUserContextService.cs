namespace CattleyaDecrypto.Server.Services.Interfaces;

public interface IUserContextService
{
    string? GetName();
    Guid? GetId();
}