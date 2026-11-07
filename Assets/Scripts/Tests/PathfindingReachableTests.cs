using System.Collections.Generic;
using NUnit.Framework;

public class PathfindingReachableTests
{
    //stan gry z jedną jednostką gracza w (2,2)
    private GameState CreateState(Board board, Unit unit, List<Unit> extraUnits)
    {
        List<Unit> units = new List<Unit>();
        units.Add(unit);
        units.AddRange(extraUnits);
        return new GameState(board, units, new List<CoreStone>());
    }

    [Test]
    public void OpenBoard_Range2_ReturnsDiamondOf13Cells()
    {
        Board board = new Board(5, 5);
        Unit unit = new Unit(new GridPosition(2, 2), Team.Player, 100, 2, 1, 25);
        GameState state = CreateState(board, unit, new List<Unit>());

        Dictionary<GridPosition, int> reachable = Pathfinding.GetReachable(state, unit);

        Assert.AreEqual(13, reachable.Count);
        Assert.AreEqual(0, reachable[new GridPosition(2, 2)]);
        Assert.AreEqual(2, reachable[new GridPosition(4, 2)]);
    }

    [Test]
    public void Swamp_CostsTwo_ShortensReach()
    {
        Board board = new Board(5, 5);
        board.SetTerrain(new GridPosition(3, 2), TerrainObject.Swamp);
        Unit unit = new Unit(new GridPosition(2, 2), Team.Player, 100, 2, 1, 25);
        GameState state = CreateState(board, unit, new List<Unit>());

        Dictionary<GridPosition, int> reachable = Pathfinding.GetReachable(state, unit);

        Assert.AreEqual(2, reachable[new GridPosition(3, 2)]);
        Assert.IsFalse(reachable.ContainsKey(new GridPosition(4, 2)));
    }

    [Test]
    public void Rock_IsNotReachable_AndForcesDetour()
    {
        Board board = new Board(5, 5);
        board.SetTerrain(new GridPosition(3, 2), TerrainObject.Rock);
        Unit unit = new Unit(new GridPosition(2, 2), Team.Player, 100, 3, 1, 25);
        GameState state = CreateState(board, unit, new List<Unit>());

        Dictionary<GridPosition, int> reachable = Pathfinding.GetReachable(state, unit);

        Assert.IsFalse(reachable.ContainsKey(new GridPosition(3, 2)));
        // objazd wokół skały kosztuje 4, a zasięg to 3
        Assert.IsFalse(reachable.ContainsKey(new GridPosition(4, 2)));
    }

    [Test]
    public void OccupiedCell_IsNotReachable_AndBlocksPassage()
    {
        Board board = new Board(5, 5);
        Unit unit = new Unit(new GridPosition(2, 2), Team.Player, 100, 2, 1, 25);
        Unit enemy = new Unit(new GridPosition(3, 2), Team.Enemy, 100, 2, 1, 25);
        GameState state = CreateState(board, unit, new List<Unit> { enemy });

        Dictionary<GridPosition, int> reachable = Pathfinding.GetReachable(state, unit);

        Assert.IsFalse(reachable.ContainsKey(new GridPosition(3, 2)));
        Assert.IsFalse(reachable.ContainsKey(new GridPosition(4, 2)));
    }
}