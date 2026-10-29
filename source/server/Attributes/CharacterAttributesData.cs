using Newtonsoft.Json;

public class CharacterAttributesData
{
    [JsonProperty("age")]
    public int Age { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("outfit")]
    public string Outfit { get; set; }

    [JsonProperty("tattoos")]
    public string Tattoos { get; set; }
}
