using System.Text.RegularExpressions;
using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Models.Models;
using CattleyaDecrypto.Server.Services.Interfaces;
using MediatR;

namespace CattleyaDecrypto.Server.Features.Decrypto.CreateNewMatch;

public class CreateNewMatchHandler : IRequestHandler<CreateNewMatchCommand, DecryptoMatchResponse>
{
    private readonly IWordPuzzleService _wordPuzzleService;
    private readonly ICacheService _cacheService;

    public CreateNewMatchHandler(IWordPuzzleService wordPuzzleService, ICacheService cacheService)
    {
        _wordPuzzleService = wordPuzzleService;
        _cacheService = cacheService;
    }
    
    public async Task<DecryptoMatchResponse> Handle(CreateNewMatchCommand request, CancellationToken cancellationToken)
    {
        var words = _wordPuzzleService.PuzzleWords(8).ToArray();
        
        var match = new DecryptoMatch()
        {
            Id = Guid.NewGuid(),
            Round = 1,
            State = DecryptMatchState.WaitingForPlayers,
            Teams = new Dictionary<DecryptoTeamEnum, TeamInfoModel>
            {
                {
                    DecryptoTeamEnum.Blue,
                    new TeamInfoModel()
                    {
                        Words = words.Take(4).ToArray()
                    }
                },
                {
                    DecryptoTeamEnum.Red,
                    new TeamInfoModel()
                    {
                        Words = words.Skip(4).Take(4).ToArray()
                    }
                }
            }
        };

        await _cacheService.SetRecordAsync(_cacheService.GetKey<Match>(match.Id), match);

        var response = new DecryptoMatchResponse()
        {
            Id = match.Id,
            Round = match.Round,
            State = match.State,
            Teams = new Dictionary<int, CreateNewMatchTeamResponse>()
        };
        foreach (var (key, value) in match.Teams)
        {
            response.Teams.Add((int)key, new CreateNewMatchTeamResponse()
            {
                InterceptionCount = value.InterceptionCount,
                MiscommunicationCount = value.MiscommunicationCount,
                Players = value.Players
            });
        }
        
        return response;
    }
}