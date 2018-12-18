using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace черновик
{
    public class Trie
    {
        private class Node
        {
            public bool Terminal { get; set; }
            public Dictionary<char, Node> Nodes { get; private set; }
            public Node ParentNode { get; private set; }
            public char C { get; private set; }

            public string Word
            {
                get
                {
                    var b = new StringBuilder();
                    b.Insert(0, C.ToString(CultureInfo.InvariantCulture));
                    var selectedNode = ParentNode;
                    while (selectedNode != null)
                    {
                        b.Insert(0, selectedNode.C.ToString(CultureInfo.InvariantCulture));
                        selectedNode = selectedNode.ParentNode;
                    }
                    return b.ToString();
                }
            }

            public Node(Node parent, char c)
            {
                C = c;
                ParentNode = parent;
                Terminal = false;
                Nodes = new Dictionary<char, Node>();
            }

            public IEnumerable<Node> TerminalNodes(char? ignoreChar = null)
            {
                var r = new List<Node>();
                if (Terminal) r.Add(this);
                foreach (var node in Nodes.Values)
                {
                    if (ignoreChar != null && node.C == ignoreChar) continue;
                    r = r.Concat(node.TerminalNodes()).ToList();
                }
                return r;
            }
        }

        private Node TopNode_ { get; set; }
        private Node TopNode
        {
            get
            {
                if (TopNode_ == null) TopNode_ = new Node(null, ' ');
                return TopNode_;
            }
        }
        private bool CaseSensitive { get; set; }

        public HashSet<string> GetAutocompleteSuggestions(string wordStart)
        {
            wordStart = NormaliseWord(wordStart);

            var r = new HashSet<string>();

            var selectedNode = TopNode;
            foreach (var c in wordStart)
            {
                if (!selectedNode.Nodes.ContainsKey(c)) return r;
                selectedNode = selectedNode.Nodes[c];
            }
            {
                var terminalNodes = selectedNode.TerminalNodes();
                foreach (var node in terminalNodes)
                {
                    r.Add(node.Word);
                }
            }

            return r;
        }

        public Trie(IEnumerable<string> words, bool caseSensitive = false)
        {
            CaseSensitive = caseSensitive;
            foreach (var word in words)
            {
                AddWord(word);
            }
        }

        public void AddWord(string word)
        {
            word = NormaliseWord(word);
            var selectedNode = TopNode;

            for (var i = 0; i < word.Length; i++)
            {
                var c = word[i];
                if (!selectedNode.Nodes.ContainsKey(c))
                {
                    selectedNode.Nodes.Add(c, new Node(selectedNode, c));
                }
                selectedNode = selectedNode.Nodes[c];
            }
            selectedNode.Terminal = true;
        }

        private string NormaliseWord(string word)
        {
            if (String.IsNullOrWhiteSpace(word)) word = String.Empty;
            word = word.Trim();
            if (!CaseSensitive)
            {
                word = word.Trim();
            }
            return word;
        }
        public bool IsWordInTrie(string word)
        {
            word = NormaliseWord(word);
            if (String.IsNullOrWhiteSpace(word)) return false;
            var selectedNode = TopNode;
            foreach (var c in word)
            {
                if (!selectedNode.Nodes.ContainsKey(c)) return false;
                selectedNode = selectedNode.Nodes[c];
            }
            return selectedNode.Terminal;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("dictionary.txt");
            var trie = new Trie(lines);
            string input = "";
            while (true)
            {
                input = ReadNextChar(input);
                Console.Clear();
                Console.WriteLine("Press enter to exit");
                Console.WriteLine(input);
                if (input == "")
                    continue;
                var autoCompleteSuggestions = trie.GetAutocompleteSuggestions(input);
                foreach (var s in autoCompleteSuggestions)
                {
                    Console.Write(s + "\n");
                }
            }
        }
        private static string ReadNextChar(string input)
        {
            var key = Console.ReadKey().Key;
            Console.WriteLine(key.ToString());
            if (key == ConsoleKey.Backspace)
            {
                input = input.Length>1 ?input.Substring(0, input.Length - 1): "" ;
                return input;
            }
            if (key == ConsoleKey.Enter)
                Environment.Exit(1);
            return  input += SwitchEngToRus(key.ToString().ToLower()).ToString();
        }
        private static char SwitchEngToRus(string c)
        {
            switch (c)
            {
                case "q" : return 'й';
                case "w" : return 'ц';
                case "e": return 'у';
                case "r": return 'к';
                case "t": return 'е';
                case "y": return 'н';
                case "u": return 'г';
                case "i": return 'ш';
                case "o": return 'щ';
                case "p": return 'з';
                case "oem4": return 'х';
                case "oem6": return 'ъ';
                case "a": return 'ф';
                case "s": return 'ы';
                case "d": return 'в';
                case "f": return 'а';
                case "g": return 'п';
                case "h": return 'р';
                case "j": return 'о';
                case "k": return 'л';
                case "l": return 'д';
                case "oem1": return 'ж';
                case "oem7": return 'э';
                case "z": return 'я';
                case "x": return 'ч';
                case "c": return 'с';
                case "v": return 'м';
                case "b": return 'и';
                case "n": return 'т';
                case "m": return 'ь';
                case "oemcomma": return 'б';
                case "oemperiod": return 'ю';
                default: return new char();
            }
        }
    }
}
