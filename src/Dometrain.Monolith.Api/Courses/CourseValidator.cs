using FluentValidation;

namespace Dometrain.Monolith.Api.Courses;

public class CourseValidator : AbstractValidator<Course>
{
    private readonly ICourseRepository _courseRepository;
    
    public CourseValidator(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Author).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Slug).MustAsync(ValidateSlug)
            .WithMessage("A course with this name already exists");
    }
    
    private async Task<bool> ValidateSlug(Course course, string slug, CancellationToken token = default)
    {
        var existingCourse = await _courseRepository.GetBySlugAsync(slug);

        if (existingCourse is not null)
        {
            return existingCourse.Id == course.Id;
        }

        return existingCourse is null;
    }
}
