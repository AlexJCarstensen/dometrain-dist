using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Dometrain.Monolith.Api.Students;

public interface IStudentService
{
    Task<bool> CheckCredentialsAsync(string email, string password);
    
    Task<Student?> CreateAsync(Student student, string password);
    
    Task<IEnumerable<Student?>> GetAllAsync(int pageNumber, int pageSize);
    
    Task<Student?> GetByEmailAsync(string email);
    
    Task<Student?> GetByIdAsync(Guid id);
    
    Task<bool> DeleteAsync(Guid id);
}

public class StudentService : IStudentService
{
    private readonly IPasswordHasher<Student> _passwordHasher;
    private readonly IStudentRepository _studentRepository;
    private readonly IValidator<Student> _validator;

    public StudentService(IPasswordHasher<Student> passwordHasher, IStudentRepository studentRepository, IValidator<Student> validator)
    {
        _passwordHasher = passwordHasher;
        _studentRepository = studentRepository;
        _validator = validator;
    }

    public async Task<bool> CheckCredentialsAsync(string email, string password)
    {
        var storedHash = await _studentRepository.GetPasswordHashAsync(email);

        if (storedHash is null)
        {
            return false;
        }
       
        var valid = _passwordHasher.VerifyHashedPassword(null!, storedHash, password);
        return valid == PasswordVerificationResult.Success;
    }

    public async Task<Student?> CreateAsync(Student student, string password)
    {
        await _validator.ValidateAndThrowAsync(student);
        var hash = _passwordHasher.HashPassword(null!, password);
        return await _studentRepository.CreateAsync(student, hash);
    }

    public async Task<IEnumerable<Student?>> GetAllAsync(int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 50) pageSize = 50;
        
        return await _studentRepository.GetAllAsync(pageNumber, pageSize);
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await _studentRepository.GetByEmailAsync(email);
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        return await _studentRepository.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _studentRepository.DeleteByIdAsync(id);
    }
}
