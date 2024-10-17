using Newtonsoft.Json;

namespace GraphCenterVertices.VkApi
{
    /// <summary>
    /// Класс, представляющий данные списка друзей (количество и список идентификаторов).
    /// </summary>
    public class VkFriendsData
    {
        [JsonProperty("count")] 
        public int Count { get; set; }

        [JsonProperty("items")] 
        public List<long> Identifiers { get; set; }
    }
}