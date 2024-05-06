using Dapper;
using MyTodoList.Data.Models;

namespace MyTodoList.Data.Repository;

public class CategoryRepository(DatabaseService databaseService)
{
    private readonly DatabaseService _databaseService = databaseService;

    public async Task<IEnumerable<Category>> GetCategories()
    {
        using var db = _databaseService.OpenConnection();
        return await db.QueryAsync<Category>("SELECT * FROM Categories");
    }

    public async Task<int> AddCategory(Category category)
    {
        using var db = _databaseService.OpenConnection();
        return await db.ExecuteAsync("INSERT INTO Categories (Name) VALUES (@Name)", category);
    }
}