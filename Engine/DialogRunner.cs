using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace EverydayCallouts.Engine
{
    public class DialogueRunner
    {
        private class DialogueNode
        {
            public string id { get; set; }
            public string title { get; set; }
            public bool startsConversation { get; set; }
            public NodeData data { get; set; }
        }

        private class NodeData
        {
            public string questionText { get; set; }
            public List<DialogueAnswer> answers { get; set; }
        }

        private class DialogueAnswer
        {
            public string id { get; set; }
            public string text { get; set; }
            public int probability { get; set; }
            public bool endsCondition { get; set; }
            public string action { get; set; }
            public string condition { get; set; }
        }

        private class Connection
        {
            public string id { get; set; }
            public ConnectionLink from { get; set; }
            public ConnectionLink to { get; set; }
        }

        private class ConnectionLink
        {
            public string nodeId { get; set; }
        }

        private class DialogueFile
        {
            public List<DialogueNode> nodes { get; set; }
            public List<Connection> connections { get; set; }
        }

        private DialogueFile dialogue;
        private Dictionary<string, DialogueNode> nodeMap;
        private Dictionary<string, List<string>> adjacency;
        private Dictionary<string, string> variables;
        private UIMenu menu;

        public event Action OnConversationEnd;

        public void StartConversation(UIMenu menu, string jsonPath, Dictionary<string, string> variables, string startNodeId)
        {
            this.menu = menu;
            this.variables = variables;
            this.dialogue = Load(jsonPath);

            BuildNodeMap();
            BuildGraph();

            DialogueNode start = dialogue.nodes.FirstOrDefault(n => n.id == startNodeId);
            if (start == null)
            {
                Game.LogTrivial($"DialogueRunner: No starting node with id '{startNodeId}' found.");
                return;
            }

            ShowNode(start);
        }

        private DialogueFile Load(string jsonPath)
        {
            string fullPath = Path.Combine("Plugins\\LSPDFR\\EverydayCallouts\\Callouts\\Dialogue", jsonPath);
            string json = File.ReadAllText(fullPath);
            return JsonSerializer.Deserialize<DialogueFile>(json);
        }

        private void BuildNodeMap()
        {
            nodeMap = dialogue.nodes.ToDictionary(n => n.id, n => n);
        }

        private void BuildGraph()
        {
            adjacency = new Dictionary<string, List<string>>();
            foreach (var node in dialogue.nodes)
                adjacency[node.id] = new List<string>();

            foreach (var conn in dialogue.connections)
                adjacency[conn.from.nodeId].Add(conn.to.nodeId);
        }

        private void ShowNode(DialogueNode node)
        {
            Game.DisplaySubtitle(Format(node.data.questionText));
            menu.Clear();

            foreach (var answer in node.data.answers)
            {
                var item = new UIMenuItem(Format(answer.text));
                menu.AddItem(item);

                menu.OnItemSelect += (sender, selectedItem, index) =>
                {
                    if (selectedItem != item) return;

                    Game.DisplaySubtitle(Format(answer.text));

                    if (answer.endsCondition || !adjacency.ContainsKey(node.id) || adjacency[node.id].Count == 0)
                    {
                        OnConversationEnd?.Invoke();
                        return;
                    }

                    string nextId = adjacency[node.id].FirstOrDefault();
                    if (nextId != null && nodeMap.ContainsKey(nextId))
                    {
                        ShowNode(nodeMap[nextId]);
                    }
                };
            }
        }

        private string Format(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            foreach (var kvp in variables)
                text = text.Replace("{" + kvp.Key + "}", kvp.Value);
            return text;
        }
    }
}
