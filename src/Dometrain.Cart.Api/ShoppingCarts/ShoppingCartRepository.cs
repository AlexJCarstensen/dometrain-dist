using System.Net;
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using StackExchange.Redis;

namespace Dometrain.Cart.Api.ShoppingCarts;

public class ShoppingCartItemDto
{
    public required Guid StudentId { get; init; }
    public required Guid CourseId { get; init; }
}

public interface IShoppingCartRepository
{
    Task<bool> AddCourseAsync(Guid studentId, Guid courseId);

    Task<ShoppingCart?> GetByIdAsync(Guid studentId);
    
    Task<bool> RemoveItemAsync(Guid studentId, Guid courseId);
    
    Task<bool> ClearAsync(Guid studentId);
}

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly CosmosClient _cosmosClient;
    private const string DatabaseId = "cartdb";
    private const string ContainerId = "carts";

    public ShoppingCartRepository(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    public async Task<bool> AddCourseAsync(Guid studentId, Guid courseId)
    {
        var container = _cosmosClient.GetContainer(DatabaseId, ContainerId);
        ShoppingCart cart;
        try
        {
            var cartResponse = await container.ReadItemAsync<ShoppingCart>(
                studentId.ToString(), new PartitionKey(studentId.ToString()));
            cart = cartResponse.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            cart = new ShoppingCart
            {
                StudentId = studentId,
                CourseIds = []
            };
        }

        if (!cart.CourseIds.Contains(courseId))
        {
            cart.CourseIds.Add(courseId);
        }

        var response = await container.UpsertItemAsync(cart);

        return response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created;
    }

    public async Task<ShoppingCart?> GetByIdAsync(Guid studentId)
    {
        var container = _cosmosClient.GetContainer(DatabaseId, ContainerId);
        try
        {
            return await container.ReadItemAsync<ShoppingCart>(studentId.ToString(),
                new PartitionKey(studentId.ToString()));
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<bool> RemoveItemAsync(Guid studentId, Guid courseId)
    {
        var container = _cosmosClient.GetContainer(DatabaseId, ContainerId);
        try
        {
            var cart = await GetByIdAsync(studentId);
            if (cart is null)
            {
                return true;
            }

            cart.CourseIds.Remove(courseId);
            var response = await container.UpsertItemAsync(cart);
            return response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return true;
        }
    }

    public async Task<bool> ClearAsync(Guid studentId)
    {
        var container = _cosmosClient.GetContainer(DatabaseId, ContainerId);
        try
        {
            var cart = new ShoppingCart
            {
                StudentId = studentId,
                CourseIds = []
            };

            var response = await container.UpsertItemAsync(cart);
            return response.StatusCode == HttpStatusCode.OK;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return true;
        }
    }
}

public class CachedShoppingCartRepository : IShoppingCartRepository
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public CachedShoppingCartRepository(
        IShoppingCartRepository shoppingCartRepository, 
        IConnectionMultiplexer connectionMultiplexer)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<bool> AddCourseAsync(Guid studentId, Guid courseId)
    {
        return await _shoppingCartRepository.AddCourseAsync(studentId, courseId);
    }

    public async Task<ShoppingCart?> GetByIdAsync(Guid studentId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var cachedCartString = await db.StringGetAsync($"cart_id_{studentId}");
        if (!cachedCartString.IsNull)
        {
            return JsonSerializer.Deserialize<ShoppingCart>(cachedCartString.ToString());
        }
        
        return await _shoppingCartRepository.GetByIdAsync(studentId);
    }

    public async Task<bool> RemoveItemAsync(Guid studentId, Guid courseId)
    {
        return await _shoppingCartRepository.RemoveItemAsync(studentId, courseId);
    }

    public async Task<bool> ClearAsync(Guid studentId)
    {
        return await _shoppingCartRepository.ClearAsync(studentId);
    }
}
