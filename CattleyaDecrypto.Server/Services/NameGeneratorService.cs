﻿using CattleyaDecrypto.Server.Services.Interfaces;

namespace CattleyaDecrypto.Server.Services;

public class NameGeneratorService : INameGeneratorService
{
    public NameGeneratorService()
    {
        
    }

    public string GenerateName()
    {
        return $"Foo Bar#{Random.Shared.Next(0, 9999):0000}";
    }
}