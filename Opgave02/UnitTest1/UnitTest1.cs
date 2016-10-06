using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using Opgave02;
using System.Text;
using System.Linq;

namespace UnitTestProject1
{
    public static class PathChecker
    {
        public static bool ContainsPath(this List<List<string>> pathList, List<string> path)
        {
            foreach (var pathFromList in pathList)
            {
                if (pathFromList.ContainsSamePath(path)) return true;
            }
            return false;
        }

        public static bool ContainsSamePath(this List<string> thisPath, List<string> path)
        {
            if (thisPath.Count != path.Count) return false;
            for (int i = 0; i < path.Count; i++)
            {
                if (thisPath[i] != path[i]) return false;
            }
            return true;
        }

        public static bool ContainsTextPath(this string textString, List<string> path)
        {
            var pathText = GetTextStringFromPath(path);
            return textString.Normalize().Contains(pathText);
        }

        private static string GetTextStringFromPath(List<string> path)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < path.Count; i++)
            {
                sb.Append(path[i].Normalize());
            }
            return "";
        }

        public static string Normalize(this string text)
        {
            text = text.ToUpper();
            var sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsLetterOrDigit(text[i])) sb.Append(text[i]);
            }
            return sb.ToString();
        }




    }

    public static class Node
    {
        private static List<string> nodeList = new List<string>();
        private static Random random = new Random();

        public static string GetUniqueName()
        {
            string name;
            do
            {
                name = GetName();
            } while (nodeList.Contains(name));
            nodeList.Add(name);
            return name;
        }

        private static string GetName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            string textPart = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
            string digitPart = new string(Enumerable.Repeat(digits, 3).Select(s => s[random.Next(s.Length)]).ToArray());
            return textPart + digitPart;
        }

    }


    [TestClass]
    public class General
    {
        [TestMethod]
        public void CodeBuilds()
        {
            // startscore for building code = 1.
        }
    }

    [TestClass]
    public class TestAddNode
    {

        [TestMethod(), Timeout(50)]
        public void TestAddSingleNodeFirstTime()
        {
            var terrain = new Terrain();
            string node = Node.GetUniqueName();
            Assert.IsNotNull(terrain.TerrainGraph, $"Terrain: Constructor does not initialise its \'TerrainGraph\' property!");
            Assert.IsTrue((terrain.TerrainGraph is Dictionary<string, List<string>>), $"Terrain: \'TerrainGraph\' property is not of type \'Dictionary<string,List<string>>\'");
            terrain.AddNode(node, new List<string>());

            if (!terrain.TerrainGraph.ContainsKey(node)) node = node.ToLower();

            Assert.IsTrue(terrain.TerrainGraph.ContainsKey(node), $"Terrain: Node passed to \'AddNode\'-method was not stored in TerrainGraph");
            Assert.IsTrue((terrain.TerrainGraph[node].Count == 0), $"Terrain: Node passed to \'AddNode\'-NeighboutList for added single node should be empty, contains {terrain.TerrainGraph[node].Count} nodes");
        }

        [TestMethod(), Timeout(50)]
        public void TestAddNodeSecondTime()
        {
            var terrain = new Terrain();
            string node = Node.GetUniqueName();
            Assert.IsNotNull(terrain.TerrainGraph, $"Terrain: Constructor does not initialise its \'TerrainGraph\' property!");
            Assert.IsTrue((terrain.TerrainGraph is Dictionary<string, List<string>>), $"Terrain: \'TerrainGraph\' property is not of type \'Dictionary<string,List<string>>\'");

            terrain.AddNode(node, new List<string>());
            try
            {
                terrain.AddNode(node, new List<string>());
                // No exception should be thrown.
            }
            catch
            {
                Assert.Fail("Terrain: Calling \'AddNode\' twice with same node, throws an exception (should be permissible)");
            }
        }

        [TestMethod(), Timeout(50)]
        public void TestAddNodeWithExistingNeighbours()
        {
            var terrain = new Terrain();
            string nodeA = Node.GetUniqueName();
            string nodeB = Node.GetUniqueName();
            string nodeC = Node.GetUniqueName();

            Assert.IsNotNull(terrain.TerrainGraph, $"Terrain: Constructor does not initialise its \'TerrainGraph\' property!");
            Assert.IsTrue((terrain.TerrainGraph is Dictionary<string, List<string>>), $"Terrain: \'TerrainGraph\' property is not of type \'Dictionary<string,List<string>>\'");
            terrain.AddNode(nodeA, new List<string>());
            terrain.AddNode(nodeB, new List<string>());
            terrain.AddNode(nodeC, new List<string>() { nodeA, nodeB });

            if (!terrain.TerrainGraph.ContainsKey(nodeA))
            {
                nodeA = nodeA.ToLower();
                nodeB = nodeB.ToLower();
                nodeC = nodeC.ToLower();
            }


            Assert.IsTrue(terrain.TerrainGraph.ContainsKey(nodeC), $"Terrain: Node passed to \'AddNode\'-method was not stored in TerrainGraph");
            Assert.IsTrue((terrain.TerrainGraph[nodeC].Count == 2), $"Terrain: Node passed to \'AddNode\'-method has wrong NeighbourList count: {terrain.TerrainGraph[nodeA].Count} nodes, 2 expected. ");
            Assert.IsTrue(((terrain.TerrainGraph[nodeC].Contains(nodeA)) && (terrain.TerrainGraph[nodeC].Contains(nodeB))), $"Terrain: Node passed to \'AddNode\'-method has wrong neighbours.");
            Assert.IsTrue(((terrain.TerrainGraph[nodeA].Contains(nodeC)) && (terrain.TerrainGraph[nodeB].Contains(nodeC))), $"Terrain: Neighbours passed to \'AddNode\'-method do not get declaring node as neighbour.");
        }

        [TestMethod(), Timeout(50)]
        public void TestAddNodeWithNonExistingNeighbours()
        {
            var terrain = new Terrain();
            string nodeA = Node.GetUniqueName(); ;
            string nodeB = Node.GetUniqueName();
            string nodeC = Node.GetUniqueName();
            Assert.IsNotNull(terrain.TerrainGraph, $"Terrain: Constructor does not initialise its \'TerrainGraph\' property!");
            Assert.IsTrue((terrain.TerrainGraph is Dictionary<string, List<string>>), $"Terrain: \'TerrainGraph\' property is not of type \'Dictionary<string,List<string>>\'");
            terrain.AddNode(nodeA, new List<string>() { nodeB, nodeC });

            if (!terrain.TerrainGraph.ContainsKey(nodeA))
            {
                nodeA = nodeA.ToLower();
                nodeB = nodeB.ToLower();
                nodeC = nodeC.ToLower();
            }

            Assert.IsTrue(terrain.TerrainGraph.ContainsKey(nodeA), $"Terrain: Node passed to \'AddNode\'-method was not stored in TerrainGraph");
            Assert.IsTrue((terrain.TerrainGraph[nodeA].Count == 2), $"Terrain: Node passed to \'AddNode\'-method has wrong NeighbourList count: {terrain.TerrainGraph[nodeA].Count} nodes, 2 expected. ");
            Assert.IsTrue(((terrain.TerrainGraph[nodeA].Contains(nodeB)) && (terrain.TerrainGraph[nodeA].Contains(nodeC))), $"Terrain: Node passed to \'AddNode\'-method has wrong neighbours.");
            Assert.IsTrue(terrain.TerrainGraph.ContainsKey(nodeB), $"Terrain: Non existing neighbour passed to \'AddNode\'-method was not added to TerrainGraph");
            Assert.IsTrue((terrain.TerrainGraph[nodeB].Count == 1), $"Terrain: Non existing neighbour passed to \'AddNode\'-method has wrong NeighbourList count: {terrain.TerrainGraph[nodeB].Count} nodes, 1 expected. ");
            Assert.IsTrue((terrain.TerrainGraph[nodeB].Contains(nodeA)), $"Terrain: Non existing neighbour passed to \'AddNode\'-method does not get declaring node in neighbourtlist. ");
            Assert.IsTrue(terrain.TerrainGraph.ContainsKey(nodeC), $"Terrain: Non existing neighbour passed to \'AddNode\'-method was not added to TerrainGraph");
            Assert.IsTrue((terrain.TerrainGraph[nodeC].Count == 1), $"Terrain: Non existing neighbour passed to \'AddNode\'-method has wrong NeighbourList count: {terrain.TerrainGraph[nodeC].Count} nodes, 1 expected. ");
            Assert.IsTrue((terrain.TerrainGraph[nodeC].Contains(nodeA)), $"Terrain: Non existing neighbour passed to \'AddNode\'-method does not get declaring node in neighbourtlist. ");
        }
    }

    [TestClass]
    public class TestSearchAllPaths
    {

        [TestMethod(), Timeout(50)]
        public void TestPathToSelf()
        {
            var terrain = new Terrain();
            string nodeA = Node.GetUniqueName();
            string nodeB = Node.GetUniqueName();
            string nodeC = Node.GetUniqueName();
            string nodeD = Node.GetUniqueName();
            Assert.IsNotNull(terrain.TerrainGraph, $"Terrain: Constructor does not initialise its \'TerrainGraph\' property!");
            Assert.IsTrue((terrain.TerrainGraph is Dictionary<string, List<string>>), $"Terrain: \'TerrainGraph\' property is not of type \'Dictionary<string,List<string>>\'");
            terrain.AddNode(nodeA, new List<string>() { nodeB, nodeC, nodeD });
            terrain.AddNode(nodeD, new List<string>() { nodeC });

            if (!terrain.TerrainGraph.ContainsKey(nodeA))
            {
                nodeA = nodeA.ToLower();
                nodeB = nodeB.ToLower();
                nodeC = nodeC.ToLower();
                nodeD = nodeD.ToLower();
            }

            var pathList = terrain.SearchAllPaths(nodeA, nodeA);
            Assert.IsNotNull(pathList, $"Terrain: \'SearchAllPaths\' returns null when called with \"{nodeA}\" (start) and \"{nodeA}\"(destination)");
            Assert.IsTrue((pathList.Count == 1), $"Terrain: \'SearchAllPaths\' returns multiple paths when called with \"{nodeA}\" (start) and \"{nodeA}\"(destination), there should only be one path");
            Assert.IsTrue((pathList[0].Count == 1), $"Terrain: \'SearchAllPaths\' returns multiple nodes in path when called with \"{nodeA}\" (start) and \"{nodeA}\"(destination), there should only be one node (\"{nodeA}\")");
            Assert.IsTrue((pathList[0].Contains(nodeA)), $"Terrain: \'SearchAllPaths\' returns wrong nodes in path when called with \"{nodeA}\" (start) and \"{nodeA}\"(destination), node is \"{pathList[0][0]}\" should be \"{nodeA}\"");
        }

        [TestMethod(), Timeout(50)]
        public void TestPathToSelfClone1()
        {
            TestPathToSelf();
        }


        [TestMethod(), Timeout(50)]
        public void TestSinglePath()
        {
            var terrain = new Terrain();
            string nodeA = Node.GetUniqueName();
            string nodeB = Node.GetUniqueName();
            string nodeC = Node.GetUniqueName();
            string nodeD = Node.GetUniqueName();
            Assert.IsNotNull(terrain.TerrainGraph, $"Terrain: Constructor does not initialise its \'TerrainGraph\' property!");
            Assert.IsTrue((terrain.TerrainGraph is Dictionary<string, List<string>>), $"Terrain: \'TerrainGraph\' property is not of type \'Dictionary<string,List<string>>\'");
            terrain.AddNode(nodeA, new List<string>() { nodeB, nodeC, nodeD });
            terrain.AddNode(nodeD, new List<string>() { nodeC });

            if (!terrain.TerrainGraph.ContainsKey(nodeA))
            {
                nodeA = nodeA.ToLower();
                nodeB = nodeB.ToLower();
                nodeC = nodeC.ToLower();
                nodeD = nodeD.ToLower();
            }

            var pathList = terrain.SearchAllPaths(nodeA, nodeB);
            Assert.IsNotNull(pathList, $"Terrain: \'SearchAllPaths\' returns null when called with \"{nodeA}\" (start) and \"{nodeB}\"(destination)");
            Assert.IsTrue((pathList.Count == 1), $"Terrain: \'SearchAllPaths\' returns multiple paths when called with \"{nodeA}\" (start) and \"{nodeB}\"(destination), there should only be one path");
            Assert.IsTrue((pathList[0].Count == 2), $"Terrain: \'SearchAllPaths\' returns wrong number of nodes in path when called with \"{nodeA}\" (start) and \"{nodeB}\"(destination), there should only be two nodes(\"{nodeA}\" and \"{nodeB}\")");
            Assert.IsTrue(((pathList[0].Contains(nodeA)) && (pathList[0].Contains(nodeB))), $"Terrain: \'SearchAllPaths\' returns wrong nodes in single path when called with \"{nodeA}\" (start) and \"{nodeB}\"(destination), returnded nodes are \"{pathList[0][0]}\" and \"{pathList[0][1]}\" ");
        }

        [TestMethod(), Timeout(50)]
        public void TestSinglePathClone1()
        {
            TestSinglePath();
        }

        [TestMethod(), Timeout(50)]
        public void TestSinglePathClone2()
        {
            TestSinglePath();
        }




        [TestMethod(), Timeout(50)]
        public void TestMultiplePaths()
        {
            var terrain = new Terrain();
            string nodeA = Node.GetUniqueName();
            string nodeB = Node.GetUniqueName();
            string nodeC = Node.GetUniqueName();
            string nodeD = Node.GetUniqueName();
            Assert.IsNotNull(terrain.TerrainGraph, $"Terrain: Constructor does not initialise its \'TerrainGraph\' property!");
            Assert.IsTrue((terrain.TerrainGraph is Dictionary<string, List<string>>), $"Terrain: \'TerrainGraph\' property is not of type \'Dictionary<string,List<string>>\'");
            terrain.AddNode(nodeA, new List<string>() { nodeB, nodeC, nodeD });
            terrain.AddNode(nodeD, new List<string>() { nodeC });

            if (!terrain.TerrainGraph.ContainsKey(nodeA))
            {
                nodeA = nodeA.ToLower();
                nodeB = nodeB.ToLower();
                nodeC = nodeC.ToLower();
                nodeD = nodeD.ToLower();
            }


            var pathList = terrain.SearchAllPaths(nodeB, nodeD);
            Assert.IsNotNull(pathList, $"Terrain: \'SearchAllPaths\' returns null when called with multiply connected nodes \"{nodeB}\" (start) and \"{nodeD}\"(destination)");
            Assert.IsTrue((pathList.Count == 2), $"Terrain: \'SearchAllPaths\' returns wrong number of  paths when called with \"{nodeB}\" (start) and \"{nodeD}\"(destination), there should be two paths ({pathList.Count} returned)");

            // check if corrrect paths returned
            Assert.IsTrue(pathList.ContainsPath(new List<string>() { nodeB, nodeA, nodeD }), $"Terrain: \'SearchAllPaths\' does not return one of the elligible paths");
            Assert.IsTrue(pathList.ContainsPath(new List<string>() { nodeB, nodeA, nodeC, nodeD }), $"Terrain: \'SearchAllPaths\' does not return one of the elligible paths");

        }

        [TestMethod(), Timeout(50)]
        public void TestMultiplePathsClone1()
        {
            TestMultiplePaths();
        }

        [TestMethod(), Timeout(50)]
        public void TestMultiplePathsClone2()
        {
            TestMultiplePaths();
        }


        [TestMethod(), Timeout(50)]
        public void TestImpossiblePath()
        {
            var terrain = new Terrain();
            string nodeA = Node.GetUniqueName();
            string nodeB = Node.GetUniqueName();
            string nodeC = Node.GetUniqueName();
            string nodeD = Node.GetUniqueName();
            Assert.IsNotNull(terrain.TerrainGraph, $"Terrain: Constructor does not initialise its \'TerrainGraph\' property!");
            Assert.IsTrue((terrain.TerrainGraph is Dictionary<string, List<string>>), $"Terrain: \'TerrainGraph\' property is not of type \'Dictionary<string,List<string>>\'");
            terrain.AddNode(nodeA, new List<string>() { nodeB });
            terrain.AddNode(nodeD, new List<string>() { nodeC });

            if (!terrain.TerrainGraph.ContainsKey(nodeA))
            {
                nodeA = nodeA.ToLower();
                nodeB = nodeB.ToLower();
                nodeC = nodeC.ToLower();
                nodeD = nodeD.ToLower();
            }

            var pathList = terrain.SearchAllPaths(nodeA, nodeC);
            Assert.IsNotNull(pathList, $"Terrain: \'SearchAllPaths\' returns null when called with unconnected nodes.");
            Assert.IsTrue((pathList.Count == 0), $"Terrain: \'SearchAllPaths\' returns at least one path when called with unconnected nodes");
        }

        [TestMethod(), Timeout(50)]
        public void TestImpossiblePathClone1()
        {
            TestImpossiblePath();
        }

    }

    [TestClass]

    public class TestConsoleUI
    {

        [TestMethod(), Timeout(50)]
        public void TestConsoleUIContructor()
        {
            var terrain = new Terrain();
            // instantiating a ConsoleUI object injecting a terrain object should succeed
            var consoleUI = new ConsoleUI(terrain);
        }

        [TestMethod(), Timeout(200)]
        public void TestReadingTerrainFileToTerrain()
        {
            string nodeA = Node.GetUniqueName();
            string nodeB = Node.GetUniqueName(); 
            string nodeC = Node.GetUniqueName();
            string nodeD = Node.GetUniqueName();

            string filePath = "test1.txt";
            
            string[] lines = new string[] { nodeB, $"{nodeA} -{nodeB},{nodeC.ToLower()},  {nodeD}", $"{nodeD}-{nodeC}" };
            File.WriteAllLines(filePath, lines);
            
            var terrain = new Terrain();
            Assert.IsNotNull(terrain.TerrainGraph, $"Terrain: Constructor does not initialise its \'TerrainGraph\' property!");
            Assert.IsTrue((terrain.TerrainGraph is Dictionary<string, List<string>>), $"Terrain: \'TerrainGraph\' property is not of type \'Dictionary<string,List<string>>\'");

            var consoleUI = new ConsoleUI(terrain);

            string consoleInput = "";
            using (var sw = new StringWriter())
            {
                using (var sr = new StringReader($"{consoleInput}\n\n"))
                {
                    Console.SetOut(sw);
                    Console.SetIn(sr);
                    consoleUI.ReadGraph(filePath);
                }
            }

            if (!terrain.TerrainGraph.ContainsKey(nodeA))
            {
                nodeA = nodeA.ToLower();
                nodeB = nodeB.ToLower();
                nodeC = nodeC.ToLower();
                nodeD = nodeD.ToLower();
            }
            Assert.IsTrue((terrain.TerrainGraph.Count == 4), $"ConsoleUI: after calling \'ReadGraph\' terrain has wrong number of nodes: {terrain.TerrainGraph.Count} found, 4 expected");
            Assert.IsTrue((terrain.TerrainGraph.ContainsKey(nodeA)), $"ConsoleUI: after calling \'ReadGraph\' terrain is missing a node.");
            Assert.IsTrue((terrain.TerrainGraph.ContainsKey(nodeC)), $"ConsoleUI: after calling \'ReadGraph\' terrain is missing a node.");
            Assert.IsTrue((terrain.TerrainGraph[nodeC].Contains(nodeD)), $"ConsoleUI: \'ReadGraph\' is not processing neighbours correctly.");
            Assert.IsTrue((terrain.TerrainGraph[nodeC].Contains(nodeA)), $"ConsoleUI: \'ReadGraph\' is not processing neighbours correctly (not case insensitive).");
            Assert.IsTrue((terrain.TerrainGraph[nodeA].Contains(nodeD)), $"ConsoleUI: \'ReadGraph\' is not processing neighbours correctly (are spaces skipped correctly?).");
        }

        [TestMethod(), Timeout(50)]
        public void TestInvalidQueryExceptionHandling()
        {
            var terrain = new Terrain();
            Assert.IsNotNull(terrain.TerrainGraph, $"Terrain: Constructor does not initialise its \'TerrainGraph\' property!");
            Assert.IsTrue((terrain.TerrainGraph is Dictionary<string, List<string>>), $"Terrain: \'TerrainGraph\' property is not of type \'Dictionary<string,List<string>>\'");
            var consoleUI = new ConsoleUI(terrain);

            string consoleInput = "This is not a valid - query";

            using (var sw = new StringWriter())
            {
                using (var sr = new StringReader($"{consoleInput}\n\n"))
                {
                    Console.SetOut(sw);
                    Console.SetIn(sr);

                    // Act
                    try
                    {
                        consoleUI.ProcessQueries();
                    }
                    catch 
                    {
                        Assert.Fail($"ConsoleUI: \'ProcessQueries\' does not catch exceptions thrown because of invalid user input.");
                    }

                    // var result = sw.ToString();                   
                }
            }
        }

        [TestMethod(), Timeout(50)]
        public void TestOutputTerrainLinesToConsole()
        {
            var terrain = new Terrain();
            Assert.IsNotNull(terrain.TerrainGraph, $"Terrain: Constructor does not initialise its \'TerrainGraph\' property!");
            Assert.IsTrue((terrain.TerrainGraph is Dictionary<string, List<string>>), $"Terrain: \'TerrainGraph\' property is not of type \'Dictionary<string,List<string>>\'");
            var consoleUI = new ConsoleUI(terrain);

            string nodeA = Node.GetUniqueName();
            string nodeB = Node.GetUniqueName();
            string nodeC = Node.GetUniqueName();
            string nodeD = Node.GetUniqueName();

            string filePath = "test.txt";

            string[] lines = new string[] { nodeB, $"{nodeA} -{nodeB},{nodeC},  {nodeD}", $"{nodeD}-{nodeC}" };
            File.WriteAllLines(filePath, lines);


            string consoleInput = "";
            string consoleOutput;
            using (var sw = new StringWriter())
            {
                using (var sr = new StringReader($"{consoleInput}\n\n"))
                {
                    Console.SetOut(sw);
                    Console.SetIn(sr);
                    consoleUI.ReadGraph(filePath);
                    consoleOutput = sw.ToString().ToUpper();
                }
            }
            Assert.IsTrue(consoleOutput.Contains($"{nodeA} -{nodeB},{nodeC},  {nodeD}".ToUpper()), $"ConsoleUI: \'ReadGraph\' does not print out the lines from the input file.");
            Assert.IsTrue(consoleOutput.Contains($"{nodeD}-{nodeC}".ToUpper()), $"ConsoleUI: \'ReadGraph\' does not print out the lines from the input file.");
        }

        
        [TestMethod(), Timeout(50)]
        public void TestOutputForMultiplePaths()
        {
            var terrain = new Terrain();
            string nodeA = Node.GetUniqueName();
            string nodeB = Node.GetUniqueName();
            string nodeC = Node.GetUniqueName();
            string nodeD = Node.GetUniqueName();
            Assert.IsNotNull(terrain.TerrainGraph, $"Terrain: Constructor does not initialise its \'TerrainGraph\' property!");
            Assert.IsTrue((terrain.TerrainGraph is Dictionary<string, List<string>>), $"Terrain: \'TerrainGraph\' property is not of type \'Dictionary<string,List<string>>\'");
            terrain.AddNode(nodeA, new List<string>() { nodeB, nodeC, nodeD });
            terrain.AddNode(nodeD, new List<string>() { nodeC });

            var consoleUI = new ConsoleUI(terrain);
            string consoleInput = $"{nodeB} - {nodeD}";
            string consoleOutput;
            using (var sw = new StringWriter())
            {
                using (var sr = new StringReader($"{consoleInput}\n\n"))
                {
                    Console.SetOut(sw);
                    Console.SetIn(sr);
                    consoleUI.ProcessQueries();
                    consoleOutput = sw.ToString().ToUpper();
                }
            }
            Assert.IsTrue(consoleOutput.ContainsTextPath(new List<string>() { nodeB, nodeA, nodeD }), $"ConsoleUI: \'ProcessQueries\' does not print out one of the elligible paths");
            Assert.IsTrue(consoleOutput.ContainsTextPath(new List<string>() { nodeB, nodeA, nodeC, nodeD }), $"ConsoleUI: \'SearchAllPaths\' does not print out one of the elligible paths");
        }
    }
}
