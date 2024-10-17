namespace GraphCenterVertices
{
    /// <summary>
    /// Класс конфигов
    /// </summary>
    public static class Configs
    {
        /// <summary>
        /// АПИ токен для работы с ВК.
        /// </summary>
        public static string VkAccessToken = "заглушка";

        /// <summary>
        /// Путь отправки запроса для получения списка друзей.
        /// </summary>
        public static string VkBaseRoute = "https://api.vk.com/method/";
        
        /// <summary>
        /// Путь отправки запроса для получения списка друзей.
        /// </summary>
        public static string GetFriendsRoute = "friends.get";
        
        /// <summary>
        /// Версия ВК АПИ.
        /// </summary>
        public static string ApiVersion = "5.199";
        
        /// <summary>
        /// Путь до графа в виде json.
        /// </summary>
        public static string GraphJsonName = "graph2.json";
    }
}