using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace MyTodoList.Data;

public class DatabaseService(string serverConnectionString, string dbConnectionString)
{
    private readonly string _serverConnectionString = serverConnectionString;
    private readonly string _dBConnectionString = dbConnectionString;

    public void CreateDatabase()
    {
        using var connection = new SqlConnection(_serverConnectionString);
        connection.Open();

        const string query = """
                             
                                             IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'MyTodoListDB')
                                             CREATE DATABASE MyTodoListDB
                                         
                             """;

        connection.Execute(query);
    }

    public IDbConnection OpenConnection() => new SqlConnection(_dBConnectionString);

    public void CreateTables()
    {
        using var connection = OpenConnection();

        const string query = """
                             
                                             USE MyTodoListDB;
                             
                                             IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'Categories')
                                             CREATE TABLE Categories
                                             (
                                                 Id INT PRIMARY KEY IDENTITY,
                                                 Name NVARCHAR(MAX) NOT NULL
                                             );
                             
                                             IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'Jobs')
                                             CREATE TABLE Jobs
                                             (
                                                 Id INT PRIMARY KEY IDENTITY,
                                                 Name NVARCHAR(MAX) NOT NULL,
                                                 CategoryId INT NOT NULL,
                                                 IsDone BIT NOT NULL,
                                                 CONSTRAINT FK_Jobs_Categories FOREIGN KEY (CategoryId)
                                                 REFERENCES Categories(Id)
                                             );
                                         
                             """;

        connection.Execute(query);
    }
}