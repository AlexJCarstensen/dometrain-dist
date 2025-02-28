using System.Diagnostics.Metrics;
using Dometrain.Monolith.Api.Sdk;

namespace Dometrain.Cart.Api.ShoppingCarts;

public interface IShoppingCartService
{
    Task<ShoppingCart?> AddCourseAsync(Guid studentId, Guid courseId);

    Task<ShoppingCart?> GetByIdAsync(Guid studentId);
    
    Task<ShoppingCart?> RemoveItemAsync(Guid studentId, Guid courseId);
    
    Task<ShoppingCart?> ClearAsync(Guid studentId);
}

public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IStudentsApiClient _studentsApiClient;
    private readonly ICourseApiClient _coursesApiClient;
    private readonly Counter<long> _itemsAddedCounter;
    
    public ShoppingCartService(IShoppingCartRepository shoppingCartRepository, IStudentsApiClient studentsApiClient, ICourseApiClient coursesApiClient, IMeterFactory meterFactory)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _studentsApiClient = studentsApiClient;
        _coursesApiClient = coursesApiClient;
        _itemsAddedCounter = meterFactory.Create("cart.meter").CreateCounter<long>("cart.items.added");
    }

    public async Task<ShoppingCart?> AddCourseAsync(Guid studentId, Guid courseId)
    {
        var student = await _studentsApiClient.GetAsync(studentId.ToString());
        if (student is null)
        {
            return null;
        }
        
        var course = await _coursesApiClient.GetAsync(courseId.ToString());
        if (course is null)
        {
            return null;
        }
        
        await _shoppingCartRepository.AddCourseAsync(studentId, courseId);
        _itemsAddedCounter.Add(1);
        return await GetByIdAsync(studentId);
    }

    public async Task<ShoppingCart?> GetByIdAsync(Guid studentId)
    {
        return await _shoppingCartRepository.GetByIdAsync(studentId);
    }

    public async Task<ShoppingCart?> RemoveItemAsync(Guid studentId, Guid courseId)
    {
        var student = await _studentsApiClient.GetAsync(studentId.ToString());
        if (student is null)
        {
            return null;
        }
        
        var course = await _coursesApiClient.GetAsync(courseId.ToString());
        if (course is null)
        {
            return null;
        }
        
        await _shoppingCartRepository.RemoveItemAsync(studentId, courseId);
        return await GetByIdAsync(studentId);
    }

    public async Task<ShoppingCart?> ClearAsync(Guid studentId)
    {
        var student = await _studentsApiClient.GetAsync(studentId.ToString());
        if (student is null)
        {
            return null;
        }

        await _shoppingCartRepository.ClearAsync(studentId);
        return new ShoppingCart
        {
            StudentId = studentId
        };
    }
}
