using System.Linq;

namespace GraphCenterVertices.Helpers
{
    /// <summary>
    /// Класс с вспомогательными методами для работы с графами.
    /// </summary>
    public static class GraphHelper
    {
        /// <summary>
        /// Подсчитывает количество уникальных связей в графе.
        /// </summary>
        /// <param name="graph">Граф, представленный в виде словаря, где ключ — ID пользователя, а значение — список его друзей.</param>
        /// <returns>Количество уникальных связей в графе.</returns>
        public static int CountUniqueConnections(Dictionary<long, List<long>> graph)
        {
            var uniqueConnections = new HashSet<(long, long)>();

            foreach (var kv in graph)
            {
                var user = kv.Key;
                foreach (var friend in kv.Value)
                {
                    var connection = user < friend ? (user, friend) : (friend, user);
                    uniqueConnections.Add(connection);
                }
            }

            return uniqueConnections.Count;
        }

        /// <summary>
        /// Преобразует граф, представленный в виде словаря, в матрицу смежности.
        /// </summary>
        /// <param name="graph">Граф, где ключ — ID пользователя, а значение — список его друзей.</param>
        /// <returns>Матрица смежности, представляющая связи между пользователями.</returns>
        public static int[,] ConvertGraphToAdjacencyMatrix(Dictionary<long, List<long>> graph)
        {
            var userIds = graph.Keys.ToList();
            var n = userIds.Count;

            var adjacencyMatrix = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                var userId = userIds[i];
                var friends = graph[userId];

                for (int j = 0; j < n; j++)
                {
                    var friendId = userIds[j];
                    adjacencyMatrix[i, j] = friends.Contains(friendId) ? 1 : 0;
                }
            }

            return adjacencyMatrix;
        }

        /// <summary>
        /// Находит цепочку друзей от исходного пользователя до конечного.
        /// </summary>
        /// <param name="graph">Граф, представленный в виде словаря, где ключ — ID пользователя, а значение — список его друзей.</param>
        /// <param name="startUserId">ID исходного пользователя.</param>
        /// <param name="endUserId">ID конечного пользователя.</param>
        /// <returns>Цепочка друзей от исходного пользователя до конечного или null, если путь не найден.</returns>
        public static List<long> FindFriendChain(Dictionary<long, List<long>> graph, long startUserId, long endUserId)
        {
            if (!graph.ContainsKey(startUserId))
            {
                Console.WriteLine($"ID исходного пользователя {startUserId} отсутствует в графе.");
                return null;
            }

            if (!graph.ContainsKey(endUserId))
            {
                Console.WriteLine($"ID конечного пользователя {endUserId} отсутствует в графе.");
                return null;
            }

            var queue = new Queue<List<long>>();
            queue.Enqueue(new List<long> { startUserId });
            var visited = new HashSet<long> { startUserId };

            while (queue.Count > 0)
            {
                var path = queue.Dequeue();
                long currentUserId = path[^1];

                // Обрабатываем всех друзей текущего пользователя
                foreach (var friendId in graph[currentUserId])
                {
                    if (!visited.Contains(friendId))
                    {
                        visited.Add(friendId);
                        var newPath = new List<long>(path) { friendId };

                        if (friendId == endUserId)
                        {
                            return newPath;
                        }

                        queue.Enqueue(newPath);
                    }
                }
            }

            Console.WriteLine("Цепочка друзей не найдена.");
            return null;
        }
        /// <summary>
        /// Вычисляет степень посредничества для каждого узла в графе.
        /// </summary>
        /// <param name="graph">Граф, представленный в виде словаря, где ключ — ID пользователя, а значение — список его друзей.</param>
        /// <returns>Массив значений степени посредничества для каждого узла.</returns>
        public static Dictionary<long, double> CalculateBetweennessCentrality(Dictionary<long, List<long>> graph)
        {
            var betweenness = new Dictionary<long, double>();
            foreach (var userId in graph.Keys)
            {
                betweenness[userId] = 0;
            }

            foreach (var s in graph.Keys)
            {
                foreach (var t in graph.Keys)
                {
                    if (s != t)
                    {
                        var paths = new List<List<long>>();
                        FindPaths(graph, s, t, new List<long>(), paths);
                        foreach (var path in paths)
                        {
                            for (int i = 1; i < path.Count - 1; i++)
                            {
                                betweenness[path[i]]++;
                            }
                        }
                    }
                }
            }

            return betweenness;
        }

        /// <summary>
        /// Находит все пути между двумя узлами в графе.
        /// </summary>
        /// <param name="graph">Граф, представленный в виде словаря.</param>
        /// <param name="start">ID начального узла.</param>
        /// <param name="end">ID конечного узла.</param>
        /// <param name="path">Текущий путь.</param>
        /// <param name="paths">Список всех найденных путей.</param>
        private static void FindPaths(Dictionary<long, List<long>> graph, long start, long end, List<long> path, List<List<long>> paths)
        {
            path.Add(start);
            if (start == end)
            {
                paths.Add(new List<long>(path));
            }
            else
            {
                foreach (var friendId in graph[start])
                {
                    if (!path.Contains(friendId))
                    {
                        FindPaths(graph, friendId, end, path, paths);
                    }
                }
            }
            path.RemoveAt(path.Count - 1);
        }
        /// <summary>
        /// Вычисляет центральность по собственному вектору для графа.
        /// </summary>
        /// <param name="graph">Граф, представленный в виде словаря, где ключ — ID пользователя, а значение — список его друзей.</param>
        /// <returns>Словарь, где ключ — ID пользователя, а значение — его центральность.</returns>
        public static Dictionary<long, double> CalculateEigenvectorCentrality(Dictionary<long, List<long>> graph)
        {
            var centrality = new Dictionary<long, double>();
            var userIds = graph.Keys.ToList();
            int n = userIds.Count;

            // Инициализация центральности
            foreach (var userId in userIds)
            {
                centrality[userId] = 1.0; // Начальное значение
            }

            // Итерации для вычисления центральности
            for (int iteration = 0; iteration < 100; iteration++) // Количество итераций можно настроить
            {
                var newCentrality = new Dictionary<long, double>();

                foreach (var userId in userIds)
                {
                    double sum = 0.0;

                    // Суммируем центральности соседей
                    foreach (var friendId in graph[userId])
                    {
                        sum += centrality[friendId];
                    }

                    // Обновляем центральность для текущего узла
                    newCentrality[userId] = sum;
                }
                // Нормализация
                double norm = newCentrality.Values.Sum();
                foreach (var userId in userIds)
                {
                    newCentrality[userId] /= norm; // Нормализуем так, чтобы сумма была равна 1
                }

                centrality = newCentrality;
            }

            return centrality;
        }
        public static Dictionary<long, double> CalculateClosenessCentrality(Dictionary<long, List<long>> graph)
        {
            var centrality = new Dictionary<long, double>();

            foreach (var node in graph.Keys)
            {
                var distances = BFS(graph, node);
                var reachableNodes = distances.Count(d => d.Value != double.MaxValue); // Изменено на d.Value
                var totalDistance = distances.Values.Where(d => d != double.MaxValue).Sum(); // Суммируем только доступные расстояния

                var closeness = reachableNodes > 1 ? (reachableNodes - 1) / totalDistance : 0;
                centrality[node] = closeness;
            }

            return centrality;
        }

        private static Dictionary<long, double> BFS(Dictionary<long, List<long>> graph, long startNode)
        {
            var distances = new Dictionary<long, double>();
            var queue = new Queue<long>();
            var visited = new HashSet<long>();

            foreach (var node in graph.Keys)
            {
                distances[node] = double.MaxValue; // Изначально назначаем бесконечность
            }

            distances[startNode] = 0;
            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var neighbor in graph[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        distances[neighbor] = distances[current] + 1;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return distances;
        }
    }
}