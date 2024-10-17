using Newtonsoft.Json;

namespace GraphCenterVertices.VkApi
{
    /// <summary>
    /// Класс для работы в Вк Апи.
    /// </summary>
    public class VkApiClient
    {
        private readonly HttpClient _httpClient;

        public VkApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.vk.com/method/");
        }

        /// <summary>
        /// Получить список друзей пользователя.
        /// </summary>
        /// <param name="userId">ID пользователя, для которого нужно получить список друзей.</param>
        /// <returns>Список идентификаторов друзей.</returns>
        public async Task<List<long>> GetVkFriends(long userId)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "user_id", userId.ToString() },
                { "access_token", Configs.VkAccessToken },
                { "v", Configs.ApiVersion }
            };
            
            var queryString = new FormUrlEncodedContent(queryParams);
            var requestUri = $"{Configs.GetFriendsRoute}?{await queryString.ReadAsStringAsync()}";
            
            var result = await _httpClient.GetAsync(requestUri, CancellationToken.None);

            result.EnsureSuccessStatusCode(); // Убедиться, что статус успешный
            
            var responseContent = await result.Content.ReadAsStringAsync(CancellationToken.None);
            
            var vkResponse = JsonConvert.DeserializeObject<GetVkFriendsResponse>(responseContent);
            
            if (vkResponse?.Response == null)
            {
                return new List<long>();
            }
            
            return vkResponse.Response.Identifiers;
        }
        
        public async Task<Dictionary<long, List<long>>> GetFriendsAndFriendsOfFriends(long userId)
        {
            var friendsGraph = new Dictionary<long, List<long>>();
            
            Console.WriteLine("Получаем список моих друзей");
            var friends = await GetVkFriends(userId);
            friendsGraph[userId] = friends;
            Console.WriteLine("Мои друзья получены, получаем список моих друзей");
            
            var count = 1;
            
            foreach (var friendId in friends)
            {
                var friendsOfFriend = await GetVkFriends(friendId);
                friendsGraph[friendId] = friendsOfFriend;

                foreach (var friendOfFriend in friendsOfFriend)
                {
                    if (!friendsGraph.ContainsKey(friendOfFriend))
                    {
                        friendsGraph[friendOfFriend] = new List<long>();
                    }
                }
                
                Console.WriteLine("Список получен, задержка " + count);
                await Task.Delay(333);
                count++;
            }
            
            SaveGraphToJson(friendsGraph);

            return friendsGraph;
        }
        
        /// <summary>
        /// Сохранить граф в формате JSON.
        /// </summary>
        private void SaveGraphToJson(Dictionary<long, List<long>> graph)
        {
            var json = JsonConvert.SerializeObject(graph, Formatting.Indented);
            File.WriteAllText(Configs.GraphJsonName, json);
        }
    }
}