using Newtonsoft.Json;

namespace GraphCenterVertices.FileHelpers;

public static class JsonHelper
{
    /// <summary>
    /// Сохранить граф в формате JSON.
    /// </summary>
    public static void SaveGraphToJson(Dictionary<long, List<long>> graph)
    {
        var json = JsonConvert.SerializeObject(graph, Formatting.Indented);
        File.WriteAllText(Configs.GraphJsonName, json);
    }
    
    /// <summary>
    /// Выгрузить граф в формате JSON.
    /// </summary>
    public static Dictionary<long, List<long>> LoadGraphFromJson(string filePath)
    {
        var json = File.ReadAllText(filePath);
        
        return JsonConvert.DeserializeObject<Dictionary<long, List<long>>>(json);
    }
}