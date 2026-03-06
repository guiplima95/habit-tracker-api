using HabitTracker.API.Application.Common.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HabitTracker.API.Infrastructure.Data;

public sealed class DapperContext(IConfiguration configuration) : IDapperContext
{
    private readonly string _connectionString =
        configuration.GetConnectionString("SqlServer")!;

    // Always returns a fresh open connection
    // Dapper manages the connection lifecycle
    public IDbConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}