using FluentValidation;

namespace Dometrain.Monolith.Api.Courses;

public interface ICourseService
{
    Task<Course?> CreateAsync(Course course);
    
    Task<Course?> GetByIdAsync(Guid id);
    
    Task<Course?> GetBySlugAsync(string slug);
    
    Task<IEnumerable<Course>> GetAllAsync(string nameFilter, int pageNumber, int pageSize);
    
    Task<Course?> UpdateAsync(Course course);
    
    Task<bool> DeleteAsync(Guid id);
}

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IValidator<Course> _validator;

    public CourseService(ICourseRepository courseRepository, IValidator<Course> validator)
    {
        _courseRepository = courseRepository;
        _validator = validator;
    }

    public async Task<Course?> CreateAsync(Course course)
    {
        await _validator.ValidateAndThrowAsync(course);
        return await _courseRepository.CreateAsync(course);
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _courseRepository.GetByIdAsync(id);
    }

    public async Task<Course?> GetBySlugAsync(string slug)
    {
        return await _courseRepository.GetBySlugAsync(slug);
    }

    public async Task<IEnumerable<Course>> GetAllAsync(string nameFilter, int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 50) pageSize = 50;

        return await _courseRepository.GetAllAsync(nameFilter, pageNumber, pageSize);
    }

    public async Task<Course?> UpdateAsync(Course course)
    {
        await _validator.ValidateAndThrowAsync(course);
        var existingCourse = await GetByIdAsync(course.Id);
        if (existingCourse is null)
        {
            return null;
        }
        
        return await _courseRepository.UpdateAsync(course);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _courseRepository.DeleteAsync(id);
    }
}
