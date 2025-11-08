using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUI_MHike.Services;

public interface IDatabaseService
{
    SQLiteAsyncConnection Connection { get; }
    Task InitAsync();
}

public class DatabaseService : IDatabaseService
{
    private readonly string _dbPath;
    private SQLiteAsyncConnection? _conn;

    public DatabaseService()
    {
        var basePath = FileSystem.AppDataDirectory;
        _dbPath = Path.Combine(basePath, "mhike.db3");
    }

    public SQLiteAsyncConnection Connection =>
        _conn ??= new SQLiteAsyncConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

    public async Task InitAsync()
    {
        // Create tables if not exist
        await Connection.CreateTableAsync<Models.Hike>();
        await Connection.CreateTableAsync<Models.Observation>();
    }
}


