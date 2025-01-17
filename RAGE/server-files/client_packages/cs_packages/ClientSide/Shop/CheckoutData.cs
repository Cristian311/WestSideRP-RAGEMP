using System.Collections.Generic;
using Newtonsoft.Json;

public class CheckoutData
{
    [JsonProperty("items")]
    public List<CartItem> Items { get; set; }

    [JsonProperty("totalCost")]
    public decimal TotalCost { get; set; }

    [JsonProperty("paymentMethod")]
    public string PaymentMethod { get; set; }
}

public class CartItem
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("price")]
    public decimal Price { get; set; }

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("totalPrice")]
    public decimal TotalPrice { get; set; }
}
