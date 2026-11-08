using System.Collections.Generic;
using NUnit.Framework;

public class PathfindingPathTests
{
    private GameState CreateState(Board board, Unit unit, List<Unit> extraUnits)
    {
        List<Unit> units = new List<Unit>();
        units.Add(unit);
        units.AddRange(extraUnits);
        return new GameState(board, units, new List<CoreStone>());
    }

    // koszt ścieżki = suma kosztów wejścia na kolejne pola (bez pola startowego)
    private int PathCost(GameState state, List<GridPosition> path)
    {
        int cost = 0;
        for (int i = 1; i < path.Count; i++)
        {
            cost += state.Board.GetCell(path[i]).MovementCost;
        }
        return cost;
    }

    [Test]
    public void FindPath_StraightLine_ReturnsShortestPath()
    {
        Board board = new Board(5, 5);
        Unit unit = new Unit(new GridPosition(0, 0), Team.Player, 100, 3, 1, 25);
        GameState state = CreateState(board, unit, new List<Unit>());

        List<GridPosition> path = Pathfinding.FindPath(state, new GridPosition(0, 0), new GridPosition(3, 0));

        Assert.IsNotNull(path);
        Assert.AreEqual(4, path.Count);
        Assert.AreEqual(new GridPosition(0, 0), path[0]);
        Assert.AreEqual(new GridPosition(3, 0), path[path.Count - 1]);
    }

    [Test]
    public void FindPath_WallInTheWay_GoesAround()
    {
        Board board = new Board(5, 5);
        board.SetTerrain(new GridPosition(1, 0), TerrainObject.Rock);
        board.SetTerrain(new GridPosition(1, 1), TerrainObject.Rock);
        Unit unit = new Unit(new GridPosition(0, 0), Team.Player, 100, 3, 1, 25);
        GameState state = CreateState(board, unit, new List<Unit>());

        List<GridPosition> path = Pathfinding.FindPath(state, new GridPosition(0, 0), new GridPosition(2, 0));

        Assert.IsNotNull(path);
        Assert.AreEqual(7, path.Count); // objazd górą: 6 kroków
        foreach (GridPosition pos in path)
        {
            Assert.IsTrue(state.Board.GetCell(pos).IsWalkable);
        }
    }

    [Test]
    public void FindPath_TargetCutOff_ReturnsNull()
    {
        Board board = new Board(5, 5);
        for (int y = 0; y < 5; y++)
        {
            board.SetTerrain(new GridPosition(1, y), TerrainObject.Rock);
        }
        Unit unit = new Unit(new GridPosition(0, 0), Team.Player, 100, 3, 1, 25);
        GameState state = CreateState(board, unit, new List<Unit>());

        List<GridPosition> path = Pathfinding.FindPath(state, new GridPosition(0, 0), new GridPosition(3, 0));

        Assert.IsNull(path);
    }

    [Test]
    public void FindPath_CostsMatchGetReachable_ForEveryCell()
    {
        Board board = new Board(7, 7);
        board.SetTerrain(new GridPosition(2, 3), TerrainObject.Swamp);
        board.SetTerrain(new GridPosition(3, 4), TerrainObject.Swamp);
        board.SetTerrain(new GridPosition(4, 2), TerrainObject.Swamp);
        board.SetTerrain(new GridPosition(4, 3), TerrainObject.Rock);
        board.SetTerrain(new GridPosition(3, 2), TerrainObject.Rock);
        Unit unit = new Unit(new GridPosition(3, 3), Team.Player, 100, 4, 1, 25);
        Unit enemy = new Unit(new GridPosition(3, 5), Team.Enemy, 100, 3, 1, 25);
        GameState state = CreateState(board, unit, new List<Unit> { enemy });

        Dictionary<GridPosition, int> reachable = Pathfinding.GetReachable(state, unit);

        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                GridPosition pos = new GridPosition(x, y);
                List<GridPosition> path = Pathfinding.FindPath(state, unit.Position, pos);

                if (reachable.ContainsKey(pos))
                {
                    Assert.IsNotNull(path, "Brak ścieżki do pola w zasięgu: " + pos);
                    Assert.AreEqual(reachable[pos], PathCost(state, path), "Inny koszt dla pola: " + pos);
                }
                else if (path != null)
                {
                    Assert.Greater(PathCost(state, path), unit.MoveRange, "Za tania ścieżka poza zasięgiem: " + pos);
                }
            }
        }
    }
}