using MAUI_MHike.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUI_MHike.Services;


public interface IHikeRepository
{
    Task<List<Hike>> GetAllAsync(string? q = null);
    Task<Hike?> GetByIdAsync(string id);
    Task<int> InsertAsync(Hike item);
    Task<int> UpdateAsync(Hike item);
    Task<int> DeleteAsync(string id);
}

public class HikeRepository : IHikeRepository
{
    private readonly IDatabaseService _db;

    public HikeRepository(IDatabaseService db) => _db = db;

    public async Task<List<Hike>> GetAllAsync(string? q = null)
    {
        var conn = _db.Connection;
        if (string.IsNullOrWhiteSpace(q))
        {
            return await conn.Table<Hike>()
                .OrderByDescending(h => h.DateIso)
                .ToListAsync();
        }
        q = q.Trim();
        return await conn.Table<Hike>()
            .Where(h => h.Name.Contains(q) || h.Location.Contains(q))
            .OrderByDescending(h => h.DateIso)
            .ToListAsync();
    }

    public async Task<Hike?> GetByIdAsync(string id) =>
        await _db.Connection.Table<Hike>().Where(h => h.Id == id).FirstOrDefaultAsync();

    public Task<int> InsertAsync(Hike item) => _db.Connection.InsertAsync(item);

    public Task<int> UpdateAsync(Hike item) => _db.Connection.UpdateAsync(item);

    public Task<int> DeleteAsync(string id) =>
        _db.Connection.Table<Hike>().DeleteAsync(h => h.Id == id);
}
