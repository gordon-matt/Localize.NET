namespace Localize;

public static class Constants
{
    /// <summary>Database schema for every provider that supports one. SQLite stores the same tables without a schema.</summary>
    public const string Schema = "localize";

    /// <summary>Kept apart from the host's migrations history so both can share a database.</summary>
    public const string MigrationsHistoryTable = "__LocalizeMigrationsHistory";

    public static class Tables
    {
        public const string Languages = "Languages";
        public const string LocalizableStrings = "LocalizableStrings";
        public const string LocalizableProperties = "LocalizableProperties";
    }

    public static class MaxLengths
    {
        public const int ScopeId = 64;
        public const int CultureCode = 32;
        public const int Name = 255;

        /// <summary>
        /// Stays inside SQL Server's index key limit once the scope and culture columns are included.
        /// </summary>
        public const int TextKey = 400;

        public const int EntityType = 256;
        public const int EntityId = 64;
        public const int Property = 128;
    }

    public static class DatabaseProviders
    {
        public const string MySql = "MySql";
        public const string Npgsql = "Npgsql";
        public const string Sqlite = "Sqlite";
        public const string SqlServer = "SqlServer";
    }
}
