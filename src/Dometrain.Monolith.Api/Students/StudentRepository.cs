using Dapper;
using Dometrain.Monolith.Api.Database;

namespace Dometrain.Monolith.Api.Students;

public interface IStudentRepository
{
    Task<string?> GetPasswordHashAsync(string email);
    
    Task<Student?> CreateAsync(Student student, string hash);

    Task<bool> EmailExistsAsync(string email);
    
    Task<IEnumerable<Student?>> GetAllAsync(int pageNumber, int pageSize);
    
    Task<Student?> GetByEmailAsync(string email);
    
    Task<Student?> GetByIdAsync(Guid id);
    
    Task<bool> DeleteByIdAsync(Guid id);
}

public class StudentRepository : IStudentRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public StudentRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<string?> GetPasswordHashAsync(string email)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<string>(
            "select password_hash from students where email = @email", new { email });
    }

    public async Task<Student?> CreateAsync(Student student, string hash)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            """
            insert into students (id, email, fullname, password_hash) 
            values (@id, @email, @fullname, @password_hash)
            """, new { id = student.Id, email = student.Email, fullname = student.FullName, password_hash = hash });

        if (result > 0)
        {
            return student;
        }

        return null;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.QuerySingleOrDefaultAsync<int>(
            "select 1 from students where email = @email", new { email });

        return result > 0;
    }

    public async Task<IEnumerable<Student?>> GetAllAsync(int pageNumber, int pageSize)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Student>(
            "select * from students limit @pageSize offset @pageOffset", 
            new { pageSize, pageOffset = (pageNumber - 1) * pageSize });
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<Student>(
            "select * from students where email = @email", new { email });
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<Student>(
            "select * from students where id = @id", new { id });
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            "delete from students where id = @id", new { id });
        return result > 0;
    }
}
