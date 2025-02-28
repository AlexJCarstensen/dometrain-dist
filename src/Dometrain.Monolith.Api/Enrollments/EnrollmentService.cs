using Dometrain.Monolith.Api.Courses;
using Dometrain.Monolith.Api.Students;

namespace Dometrain.Monolith.Api.Enrollments;

public interface IEnrollmentService
{
    Task<Enrollments?> GetStudentEnrollmentsAsync(Guid studentId);
    
    Task<bool?> EnrollToCourseAsync(Guid studentId, Guid courseId);
    
    Task<bool?> UnEnrollFromCourseAsync(Guid studentId, Guid courseId);
}

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;

    public EnrollmentService(IEnrollmentRepository enrollmentRepository, IStudentRepository studentRepository, ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
    }

    public async Task<Enrollments?> GetStudentEnrollmentsAsync(Guid studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student is null)
        {
            return null;
        }

        var courseIds = await _enrollmentRepository.GetEnrolledCoursesAsync(studentId);
        return new Enrollments
        {
            StudentId = studentId, CourseIds = courseIds.ToList()
        };
    }

    public async Task<bool?> EnrollToCourseAsync(Guid studentId, Guid courseId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student is null)
        {
            return null;
        }
        
        var course = await _courseRepository.GetByIdAsync(courseId);

        if (course is null)
        {
            return null;
        }

        return await _enrollmentRepository.EnrollToCourseAsync(studentId, courseId);
    }

    public async Task<bool?> UnEnrollFromCourseAsync(Guid studentId, Guid courseId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student is null)
        {
            return null;
        }
        
        var course = await _courseRepository.GetByIdAsync(courseId);

        if (course is null)
        {
            return null;
        }

        return await _enrollmentRepository.UnEnrollFromCourseAsync(studentId, courseId);
    }
}
