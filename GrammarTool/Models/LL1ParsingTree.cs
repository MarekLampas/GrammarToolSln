using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using AvaloniaGraphControl;
using GrammarTool.Helpers;

namespace GrammarTool
{
    public class LL1ParsingTree : NamedGraph
    {
        public Dictionary<string, StandardItem> Nodes;

        public LL1ParsingTree() : base("Colored Edges")
        {
            Nodes = new Dictionary<string, StandardItem>();
        }

        public void AddNode(string node, string symbol, ISolidColorBrush background)
        {
            Nodes.Add(node, new StandardItem(node, symbol, background));
        }

        public void AddEdge(string nodeA, string nodeB)
        {
            Edges.Add(new ColoredEdge(Nodes[nodeA], Nodes[nodeB]));
        }
    }

    public class NamedGraph : Graph
    {
        public NamedGraph(string name) { Name = name; }
        public string Name { get; private set; }
        public override string ToString() => Name;
    }

    public class StandardItem
    {
        public StandardItem(string name, string symbol, ISolidColorBrush background) { Name = name; Symbol = symbol; Background = background; }
        public string Name { get; private set; }
        public string Symbol { get; private set; }
        public ISolidColorBrush Background { get; private set; }
    }

    public class ColoredEdge : Edge
    {
        public ColoredEdge(StandardItem tail, StandardItem head) : base(tail, head)
        {
            if (tail.Name == "A")
                MyCustomColor = (head.Name == "B") ? "Plum" : "Peru";
            else if (tail.Name == "B")
                MyCustomColor = "DarkRed";
            else
                MyCustomColor = "YellowGreen";
        }
        public string MyCustomColor { get; }
    }
}
