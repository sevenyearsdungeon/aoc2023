
using AdventOfCode;
using System.Net.Sockets;
using System.Reflection;

internal class Day25 : Solution
{
    static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
    static string inputFilePath = Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}Small.txt");
    static string outputFilePath = Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}output.txt");
    static Dictionary<string, Node> nodeMap = new Dictionary<string, Node>();
    static List<Edge> edges = new List<Edge>();
    static Day25()
    {
        var lines = File.ReadAllLines(inputFilePath).ToList();
        foreach (var line in lines)
        {
            var newNode = new Node(line);
            nodeMap.Add(newNode.name, newNode);
        }

        List<Node> firstPassNodes = new List<Node>(nodeMap.Values);
        firstPassNodes.ForEach(c => c.AssignNeighbors());
    }

    public override string SolvePart1()
    {
        return "";
    }

    public override string SolvePart2()
    {

        return "";
    }

    class Node
    {
        public bool parsed = false;
        public string name = "uninitialied";
        public string[] connectedNodeNames;
        public HashSet<Node> connectedNodes = new HashSet<Node>();
        public Node(string s)
        {
            parsed = true;
            this.name = s.GetStringBefore(":");
            this.connectedNodeNames = s.GetSequenceAfter(":", " ", StringSplitOptions.RemoveEmptyEntries);
        }
        public Node()
        {
            name = "uninitialized";
            connectedNodeNames = new string[0];
        }

        public void AssignNeighbors()
        {
            foreach (string s in connectedNodeNames)
            {
                if (!nodeMap.ContainsKey(s))
                {
                    var newNode = new Node();
                    newNode.name = s;
                    nodeMap.Add(newNode.name, newNode);
                }

                Node referencedComponent = nodeMap[s];
                edges.Add(new Edge(this, referencedComponent));
                connectedNodes.Add(referencedComponent);
                referencedComponent.connectedNodes.Add(this);
            }
        }

        public override string ToString()
        {
            return $"{name}: {string.Join(",", connectedNodes.Select(node=>node.name))}";
        }
    }

    class Edge
    {
        public Node a, b;

        public Edge(Node a, Node b)
        {
            this.a = a;
            this.b = b;
        }

        public override bool Equals(object obj)
        {
            Edge edge = obj as Edge;
            if (obj == null)
                return false;
            return (edge.a == a && edge.b == b) ||
                (edge.b == a && edge.a == b);
        }

        public override int GetHashCode()
        {
            return a.GetHashCode() + b.GetHashCode();
        }
    }
}
