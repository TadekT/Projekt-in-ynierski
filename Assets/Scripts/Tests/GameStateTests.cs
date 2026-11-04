using System.Collections.Generic;
using NUnit.Framework;

public class GameStateTests
{
    private GameState CreateState(out Unit playerUnit, out List<CoreStone> stones)
    {
        playerUnit = new Unit(new GridPosition(1, 1), Team.Player, 100);
        List<Unit> units = new List<Unit>
        {
            playerUnit,
            new Unit(new GridPosition(5, 5), Team.Enemy, 100)
        };
        stones = new List<CoreStone>
        {
            new CoreStone(new GridPosition(0, 0), Team.Player, 50),
            new CoreStone(new GridPosition(9, 9), Team.Enemy, 50)
        };
        return new GameState(new Board(10, 10), units, stones);
    }

    [Test]
    public void GetUnitAt_ReturnsUnit_OrNullForEmptyCell()
    {
        GameState state = CreateState(out Unit playerUnit, out _);
        Assert.AreSame(playerUnit, state.GetUnitAt(new GridPosition(1, 1)));
        Assert.IsNull(state.GetUnitAt(new GridPosition(2, 2)));
    }

    [Test]
    public void IsOccupied_TrueForUnitAndCoreStone_FalseForEmptyCell()
    {
        GameState state = CreateState(out _, out _);
        Assert.IsTrue(state.IsOccupied(new GridPosition(1, 1)));
        Assert.IsTrue(state.IsOccupied(new GridPosition(9, 9)));
        Assert.IsFalse(state.IsOccupied(new GridPosition(3, 3)));
    }

    [Test]
    public void DestroyedUnit_DoesNotOccupyCell()
    {
        GameState state = CreateState(out var playerUnit, out _);
        playerUnit.TakeDamage(999);
        Assert.IsFalse(state.IsOccupied(new GridPosition(1, 1)));
    }

    [Test]
    public void GetWinner_IsNullAtStart_AndSetAfterEnemyStoneDestroyed()
    {
        GameState state = CreateState(out _, out List<CoreStone> stones);
        Assert.IsFalse(state.IsGameOver());
        Assert.IsNull(state.GetWinner());
        stones[1].TakeDamage(999);
        Assert.IsTrue(state.IsGameOver());
        Assert.AreEqual(Team.Player, state.GetWinner());
    }
}