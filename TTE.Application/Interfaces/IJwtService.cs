﻿namespace TTE.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string username, string role);
    }
}
