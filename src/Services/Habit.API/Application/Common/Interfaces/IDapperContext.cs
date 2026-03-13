using System.Data;

namespace HabitTracker.API.Application.Common.Interfaces;

public interface IDapperContext
{
    IDbConnection CreateConnection();
}