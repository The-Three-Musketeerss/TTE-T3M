namespace TTE.Commons
{
    public static class EnvVariables
    {
        public static string AUTH_TOKEN_URL = Environment.GetEnvironmentVariable("AUTH_TOKEN_URL") ?? string.Empty;
        public static string DB_CONNECTION_STRING = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? string.Empty;
        public static string JWT_SECRET = Environment.GetEnvironmentVariable("JWT_SECRET") ?? string.Empty;
    }
}
