using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUI_MHike.Models;

namespace MAUI_MHike.Services;

public interface IObservationRepository
{
    Task<List<Observation>> ListByHikeAsync(string hikeId);
    Task<Observation?> GetByIdAsync(string id);
    Task<int> InsertAsync(Observation o);
    Task<int> UpdateAsync(Observation o);
    Task<int> DeleteAsync(string id);
}

public class ObservationRepository : IObservationRepository
{
    private readonly IDatabaseService _db;
    public ObservationRepository(IDatabaseService db) => _db = db;

    public Task<List<Observation>> ListByHikeAsync(string hikeId) =>
        _db.Connection.Table<Observation>()
            .Where(x => x.HikeId == hikeId)
            .OrderByDescending(x => x.TimeSec)
            .ToListAsync();

    public async Task<Observation?> GetByIdAsync(string id) =>
        await _db.Connection.Table<Observation>().Where(x => x.Id == id).FirstOrDefaultAsync();

    public Task<int> InsertAsync(Observation o) => _db.Connection.InsertAsync(o);
    public Task<int> UpdateAsync(Observation o) => _db.Connection.UpdateAsync(o);
    public Task<int> DeleteAsync(string id) =>
        _db.Connection.Table<Observation>().DeleteAsync(x => x.Id == id);
}
