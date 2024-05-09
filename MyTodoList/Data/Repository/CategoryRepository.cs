using Dapper;
using MyTodoList.Data.Models;

namespace MyTodoList.Data.Repository;

public class CategoryRepository(DatabaseService databaseService)
{
    private readonly DatabaseService _databaseService = databaseService;

    public async Task<IEnumerable<Category>> GetCategories()
    {
        using var db = _databaseService.OpenConnection();
        return await db.QueryAsync<Category>("SELECT Id, Name FROM Categories");
    }

    public async Task<int> AddCategory(Category category)
    {
        using var db = _databaseService.OpenConnection();
        return await db.ExecuteAsync("INSERT INTO Categories (Name) VALUES (@Name)", category);
    }
    
    public async Task<Category> GetCategory(int id)
    {
        using var db = _databaseService.OpenConnection();
        return await db.QueryFirstOrDefaultAsync<Category>(
                   "SELECT Id, Name FROM Categories WHERE Id = @Id", new { Id = id }) 
               ?? throw new InvalidOperationException($"Category with id {id} not found");
    }
    
    public async Task<int> UpdateCategory(Category category)
    {
        using var db = _databaseService.OpenConnection();
        return await db.ExecuteAsync("UPDATE Categories SET Name = @Name WHERE Id = @Id", category);
    }
    
    public async Task<int> DeleteCategory(int id)
    {
        using var db = _databaseService.OpenConnection();
        return await db.ExecuteAsync("DELETE FROM Categories WHERE Id = @Id", new { Id = id });
    }
}