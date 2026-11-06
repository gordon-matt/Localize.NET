namespace Localize;

/// <summary>
/// Provides constants.
/// </summary>
public static class Constants
{
    /// <summary>Database schema for providers that support one (SQL Server and PostgreSQL).</summary>
    public const string Schema = "localize";

    /// <summary>
    /// Prepended to every table name on providers without schemas. In MySQL a schema is a database,
    /// and SQLite has none, so the tables stay unqualified and are prefixed instead.
    /// </summary>
    public const string TablePrefix = "Localize_";

    /// <summary>Kept apart from the host's migrations history so both can share a database.</summary>
    public const string MigrationsHistoryTable = "__LocalizeMigrationsHistory";

    /// <summary>
    /// Provides tables.
    /// </summary>
    public static class Tables
    {
        /// <summary>
        /// The languages constant.
        /// </summary>
        public const string Languages = "Languages";
        /// <summary>
        /// The localizable strings constant.
        /// </summary>
        public const string LocalizableStrings = "LocalizableStrings";
        /// <summary>
        /// The localizable properties constant.
        /// </summary>
        public const string LocalizableProperties = "LocalizableProperties";
    }

    /// <summary>
    /// Provides max lengths.
    /// </summary>
    public static class MaxLengths
    {
        /// <summary>
        /// The scope identifier constant.
        /// </summary>
        public const int ScopeId = 64;
        /// <summary>
        /// The culture code constant.
        /// </summary>
        public const int CultureCode = 32;
        /// <summary>
        /// The name constant.
        /// </summary>
        public const int Name = 255;

        /// <summary>
        /// Stays inside SQL Server's index key limit once the scope and culture columns are included.
        /// </summary>
        public const int TextKey = 400;

        /// <summary>
        /// The entity type constant.
        /// </summary>
        public const int EntityType = 256;
        /// <summary>
        /// The entity identifier constant.
        /// </summary>
        public const int EntityId = 64;
        /// <summary>
        /// The property constant.
        /// </summary>
        public const int Property = 128;
    }

    /// <summary>
    /// Provides database providers.
    /// </summary>
    public static class DatabaseProviders
    {
        /// <summary>
        /// The my SQL constant.
        /// </summary>
        public const string MySql = "MySql";
        /// <summary>
        /// The npgsql constant.
        /// </summary>
        public const string Npgsql = "Npgsql";
        /// <summary>
        /// The sqlite constant.
        /// </summary>
        public const string Sqlite = "Sqlite";
        /// <summary>
        /// The SQL server constant.
        /// </summary>
        public const string SqlServer = "SqlServer";
    }
}
