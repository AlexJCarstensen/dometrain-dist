using Dapper;
using Dometrain.Monolith.Api.Database;

namespace Dometrain.Monolith.Api.Courses;

public interface ICourseRepository
{
    Task<Course?> CreateAsync(Course course);
    
    Task<Course?> GetByIdAsync(Guid id);
    
    Task<Course?> GetBySlugAsync(string slug);
    
    Task<IEnumerable<Course>> GetAllAsync(string nameFilter, int pageNumber, int pageSize);
    
    Task<Course?> UpdateAsync(Course course);
    
    Task<bool> DeleteAsync(Guid id);
}

public class CourseRepository : ICourseRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public CourseRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Course?> CreateAsync(Course course)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            """
            insert into courses (id, name, description, slug, author)
            values (@id, @name, @description, @slug, @author)
            """, course);
        
        return result > 0 ? course : null;
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<Course>(
            "select * from courses where id = @id", new { id });
    }

    public async Task<Course?> GetBySlugAsync(string slug)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<Course>(
            "select * from courses where slug = @slug", new { slug });
    }

    public async Task<IEnumerable<Course>> GetAllAsync(string nameFilter, int pageNumber, int pageSize)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Course>(
            """
            select * from courses
                     where (@nameFilter is null or name ilike ('%' || @nameFilter || '%'))
                     limit @pageSize offset @pageOffset
            """, 
            new { nameFilter, pageSize, pageOffset = (pageNumber - 1) * pageSize });
    }

    public async Task<Course?> UpdateAsync(Course course)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            """
            update courses set name = @Name, description = @Description, slug = @Slug, author = @Author
            where id = @Id
            """, course);
        return result > 0 ? course : null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            "delete from courses where id = @id", new { id });
        return result > 0;
    }
}
