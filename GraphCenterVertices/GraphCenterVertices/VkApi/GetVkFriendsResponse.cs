using Newtonsoft.Json;

namespace GraphCenterVertices.VkApi
{
    /// <summary>
    /// Класс для представления ответа API VK на запрос списка друзей.
    /// </summary>
    public class GetVkFriendsResponse
    {
        /// <summary>
        /// Ответ от ВК.
        /// </summary>
        [JsonProperty("response")] 
        public VkFriendsData Response { get; set; }
    }
}