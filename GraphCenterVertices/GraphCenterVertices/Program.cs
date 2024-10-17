using GraphCenterVertices;
using GraphCenterVertices.FileHelpers;
using GraphCenterVertices.Helpers;
using static System.Formats.Asn1.AsnWriter;

var graph = JsonHelper.LoadGraphFromJson(Configs.GraphJsonName);
var count1 = 0;
var count2 = 0;

var totalConnections = GraphHelper.CountUniqueConnections(graph);
Console.WriteLine($"Общее количество вершин: {totalConnections}");
var betweennessCentrality = GraphHelper.CalculateBetweennessCentrality(graph);
/*foreach (var kvp in betweennessCentrality)
{
    Console.WriteLine($"ID узла: {kvp.Key}, Центральность: {kvp.Value}");
}*/
var friendChain = GraphHelper.FindFriendChain(graph, 321819438, 272353);
var centralityScores = GraphHelper.CalculateEigenvectorCentrality(graph);
var centralityValues = GraphHelper.CalculateClosenessCentrality(graph);

foreach (var kvp in centralityValues)
{
    if (kvp.Value != 0) { 
        Console.WriteLine($"Узел {kvp.Key}: Центральность близости = {kvp.Value}");
        count1++;
    }
        
}
foreach (var score in centralityScores)
{
    if (score.Value != 0)
    {
        Console.WriteLine($"Пользователь ID {score.Key} имеет центральность {score.Value}");
        count2++;
    }    
}
Console.WriteLine("Цепочка друзей:");
Console.WriteLine(string.Join(" -> ", friendChain));
Console.WriteLine(count1);
Console.WriteLine(count2);