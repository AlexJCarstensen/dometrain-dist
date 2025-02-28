using Dapper;
using Dometrain.Monolith.Api.Database;

namespace Dometrain.Monolith.Api.Enrollments;

public interface IEnrollmentRepository
{
    Task<IEnumerable<Guid>> GetEnrolledCoursesAsync(Guid studentId);
    
    Task<bool> EnrollToCourseAsync(Guid studentId, Guid courseId);
    
    Task<bool> UnEnrollFromCourseAsync(Guid studentId, Guid courseId);
}

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public EnrollmentRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<Guid>> GetEnrolledCoursesAsync(Guid studentId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Guid>(
            "select course_id from enrollments where student_id = @studentId",
            new { studentId });
    }

    public async Task<bool> EnrollToCourseAsync(Guid studentId, Guid courseId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            """
            insert into enrollments (student_id, course_id)
            values (@studentId, @courseId) on conflict do nothing
            """, new { studentId, courseId });

        return result > 0;
    }

    public async Task<bool> UnEnrollFromCourseAsync(Guid studentId, Guid courseId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            """
            delete from enrollments
            where student_id = @studentId and course_id = @courseId
            """, new { studentId, courseId });

        return result > 0;
    }
}
