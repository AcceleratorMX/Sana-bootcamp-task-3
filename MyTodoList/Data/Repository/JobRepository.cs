using Dapper;
using MyTodoList.Data.Models;

namespace MyTodoList.Data.Repository;

public class JobRepository(DatabaseService databaseService)
{
    private readonly DatabaseService _databaseService = databaseService;

    public async Task<int> AddJob(Job job)
    {
        using var db = _databaseService.OpenConnection();
        return await db.ExecuteAsync(
            "INSERT INTO Jobs (Name, CategoryId, IsDone) VALUES (@Name, @CategoryId, @IsDone)",
            job);
    }

    public async Task<Job> GetJob(int id)
    {
        using var db = _databaseService.OpenConnection();
        return await db.QueryFirstOrDefaultAsync<Job>("SELECT * FROM Jobs WHERE Id = @Id", new { Id = id }) ??
               throw new InvalidOperationException();
    }

    public async Task<IEnumerable<Job>> GetJobs()
    {
        using var connection = _databaseService.OpenConnection();

        var jobs = await connection.QueryAsync<Job>("SELECT * FROM Jobs");
        var categories = await connection.QueryAsync<Category>("SELECT * FROM Categories");

        foreach (var job in jobs)
        {
            job.Category = categories.FirstOrDefault(c => c.Id == job.CategoryId);
        }

        return jobs;
    }


    public async Task<int> UpdateJob(Job job)
    {
        using var db = _databaseService.OpenConnection();
        return await db.ExecuteAsync(
            "UPDATE Jobs SET Name = @Name, CategoryId = @CategoryId, IsDone = @IsDone WHERE Id = @Id", job);
    }


    public async Task<int> DeleteJob(int id)
    {
        using var db = _databaseService.OpenConnection();
        return await db.ExecuteAsync("DELETE FROM Jobs WHERE Id = @Id", new { Id = id });
    }
}