namespace FFCEI.Microservices.EntityFrameworkCore
{
    public static class MySqlConnectionStringBuilder
    {
        public static string Build(string database, string username, string password, string server, ushort port)
            => $"Server={server};Port={port};User={username};Password={password};Database={database};OldGuids=true;Pooling=true;ConnectionLifeTime=15;ConnectionIdlePingTime=5;MaximumPoolsize=512;DefaultCommandTimeout=60";
    }
}
