using Newtonsoft.Json;

namespace Dometrain.Cart.Api.ShoppingCarts;

public class ShoppingCart
{
    [JsonProperty("pk")]
    [System.Text.Json.Serialization.JsonIgnore]
    public string Pk => StudentId.ToString();
    
    [JsonProperty("id")]
    public required Guid StudentId { get; set; }

    public List<Guid> CourseIds { get; set; } = [];
}
