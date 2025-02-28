using System.Text.RegularExpressions;

namespace Dometrain.Monolith.Api.Courses;

public partial class Course
{
    public required Guid Id { get; init; }

    public required string Name { get; set; }
    
    public required string Description { get; set; }

    public string Slug => GenerateSlug();

    public required string Author { get; set; }
    
    internal string GenerateSlug()
    {
        return SlugRegex().Replace(Name, string.Empty)
            .ToLower().Replace(" ", "-");
    }

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 100)]
    internal static partial Regex SlugRegex();
}
