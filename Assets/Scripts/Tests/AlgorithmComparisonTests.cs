using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using NUnit.Framework;

public class AlgorithmComparisonTests
{
    private const int Repeats = 200;

    private static readonly int[][] Pairs = new int[][]
    {
        new[] { 0, 4, 3, 4 },
        new[] { 0, 0, 9, 9 },
        new[] { 1, 4, 8, 4 },
        new[] { 2, 2, 2, 7 },
        new[] { 5, 1, 9, 8 }
    };

    private GameState EmptyMap()
    {
        return new GameState(new Board(10, 10), new List<Unit>(), new List<CoreStone>());
    }

    private GameState ObstacleMap()
    {
        GameState state = EmptyMap();
        for (int y = 1; y <= 8; y++)
        {
            state.Board.SetTerrain(new GridPosition(4, y), TerrainObject.Rock);
        }
        for (int x = 6; x <= 7; x++)
        {
            for (int y = 3; y <= 6; y++)
            {
                state.Board.SetTerrain(new GridPosition(x, y), TerrainObject.Swamp);
            }
        }
        return state;
    }

    [Test]
    public void CompareAlgorithms_OnTwoMaps_ChecksCorrectness_AndWritesResults()
    {
        StringBuilder csv = new StringBuilder();
                csv.AppendLine("mapa;para;start;cel;koszt ścieżki;długość ścieżki (kroki);węzły A*;węzły jednolite;czas A* (mikrosekundy);czas jednolite (mikrosekundy)");

        int[] totals = new int[2];

        RunMap("pusta", EmptyMap(), csv, totals);
        RunMap("przeszkody", ObstacleMap(), csv, totals);

        string dir = Path.Combine(Directory.GetCurrentDirectory(), "Results");
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, "porownanie_algorytmow.csv"), csv.ToString(), Encoding.UTF8);
        TestContext.Out.WriteLine(csv.ToString());

        Assert.LessOrEqual(totals[0], totals[1], "A* powinien odwiedzać łącznie nie więcej węzłów niż przeszukiwanie jednolite.");
    }

    private void RunMap(string mapName, GameState state, StringBuilder csv, int[] totals)
    {
        for (int i = 0; i < Pairs.Length; i++)
        {
            GridPosition start = new GridPosition(Pairs[i][0], Pairs[i][1]);
            GridPosition goal = new GridPosition(Pairs[i][2], Pairs[i][3]);
            string label = mapName + " P" + (i + 1);

            SearchStats aStarStats = new SearchStats();
            SearchStats uniformStats = new SearchStats();
            Pathfinding.FindPath(state, start, goal, aStarStats);
            Pathfinding.FindPathUniform(state, start, goal, uniformStats);

            Assert.IsTrue(aStarStats.Found, label + ": A* nie znalazł ścieżki");
            Assert.IsTrue(uniformStats.Found, label + ": przeszukiwanie jednolite nie znalazło ścieżki");
            Assert.AreEqual(aStarStats.PathCost, uniformStats.PathCost, label + ": różne koszty A* i jednolitego");

            Unit probe = new Unit(start, Team.Player, 100, 99, 1, 1);
            Dictionary<GridPosition, int> reachable = Pathfinding.GetReachable(state, probe);
            Assert.AreEqual(reachable[goal], aStarStats.PathCost, label + ": koszt różny od GetReachable");

            double aStarMicros = MeasureMicroseconds(() => Pathfinding.FindPath(state, start, goal));
            double uniformMicros = MeasureMicroseconds(() => Pathfinding.FindPathUniform(state, start, goal, null));

            totals[0] += aStarStats.VisitedNodes;
            totals[1] += uniformStats.VisitedNodes;

            csv.AppendLine(string.Join(";", new string[]
            {
                mapName,
                "P" + (i + 1),
                "(" + start.x + "," + start.y + ")",
                "(" + goal.x + "," + goal.y + ")",
                aStarStats.PathCost.ToString(),
                aStarStats.PathLength.ToString(),
                aStarStats.VisitedNodes.ToString(),
                uniformStats.VisitedNodes.ToString(),
                aStarMicros.ToString("F1"),
                uniformMicros.ToString("F1")
            }));
        }
    }

    // najmniejsza średnia z 5 serii po 200 wywołań
    private static double MeasureMicroseconds(Action action)
    {
        action(); // rozgrzewka (JIT)

        double best = double.MaxValue;
        for (int batch = 0; batch < 5; batch++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (int r = 0; r < Repeats; r++)
            {
                action();
            }
            sw.Stop();

            double micros = sw.Elapsed.TotalMilliseconds * 1000.0 / Repeats;
            if (micros < best)
            {
                best = micros;
            }
        }
        return best;
    }
}