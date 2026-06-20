using SQLite;
using System.Diagnostics;
using tracktask.Models;

namespace tracktask.Data
{
    public static class AppDatabase
    {
        private static SQLiteConnection _db;
        private static readonly object _lock = new();
        private static string _dbPath;

        public static SQLiteConnection GetDb()
        {
            lock (_lock)
            {
                if (_db == null)
                {
                    _dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");

                    _db = new SQLiteConnection(
                        _dbPath,
                        SQLiteOpenFlags.ReadWrite |
                        SQLiteOpenFlags.Create |
                        SQLiteOpenFlags.FullMutex
                    );

                    // ❌ REMOVED WAL (was causing crash)
                }

                return _db;
            }
        }

        public static string GetDbPath()
        {
            return _dbPath ?? Path.Combine(FileSystem.AppDataDirectory, "app.db");
        }

        public static void CloseDb()
        {
            lock (_lock)
            {
                if (_db != null)
                {
                    try
                    {
                        _db.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"SQLite: failed to close connection: {ex.Message}");
                    }
                    finally
                    {
                        _db = null;
                    }
                }
            }
        }

        public static void InitializeDatabase()
        {
            lock (_lock)
            {
                try
                {
                    CloseDb();

                    using var db = new SQLiteConnection(
                        GetDbPath(),
                        SQLiteOpenFlags.ReadWrite |
                        SQLiteOpenFlags.Create |
                        SQLiteOpenFlags.FullMutex
                    );

                   

                    db.CreateTable<User>();
                    db.CreateTable<Track>();
                    db.CreateTable<TripTask>();
                    db.CreateTable<TrackDriver>();
                    db.CreateTable<Request>();

                    Debug.WriteLine("SQLite: database initialized successfully.");
                }
                catch (SQLiteException ex)
                {
                    var msg = ex?.Message ?? string.Empty;
                    Debug.WriteLine($"SQLite.InitializeDatabase: migration/create failed: {msg}");

                    if (msg.Contains("PRIMARY KEY", StringComparison.OrdinalIgnoreCase) ||
                        msg.Contains("Cannot add", StringComparison.OrdinalIgnoreCase) ||
                        msg.Contains("duplicate column name", StringComparison.OrdinalIgnoreCase) ||
                        msg.Contains("database is locked", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            CloseDb();

                            var path = GetDbPath();
                            var backupPath = path + ".bak_" + DateTime.Now.ToString("yyyyMMddHHmmss");

                            if (File.Exists(path))
                            {
                                try
                                {
                                    File.Move(path, backupPath);
                                    Debug.WriteLine($"SQLite: backed up database to '{backupPath}'.");
                                }
                                catch (Exception)
                                {
                                    if (File.Exists(path))
                                    {
                                        File.Delete(path);
                                        Debug.WriteLine("SQLite: deleted original DB file.");
                                    }
                                }
                            }

                            using var newDb = new SQLiteConnection(
                                GetDbPath(),
                                SQLiteOpenFlags.ReadWrite |
                                SQLiteOpenFlags.Create |
                                SQLiteOpenFlags.FullMutex
                            );

                            // ❌ REMOVED WAL AGAIN

                            newDb.CreateTable<User>();
                            newDb.CreateTable<Track>();
                            newDb.CreateTable<TripTask>();
                            newDb.CreateTable<TrackDriver>();
                            newDb.CreateTable<Request>();

                            Debug.WriteLine("SQLite: recreated database and tables after recovery.");
                        }
                        catch (Exception recoveryEx)
                        {
                            Debug.WriteLine($"SQLite: recovery failed: {recoveryEx.Message}");
                            throw;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}