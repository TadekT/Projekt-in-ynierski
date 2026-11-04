using System.Collections.Generic;
using NUnit.Framework;

public class TurnManagerTests
{
    private GameState CreateState()
    {
        var board = new Board(10, 10);
        var units = new List<Unit>
        {
            new Unit(new GridPosition(0, 0), Team.Player, 100),
            new Unit(new GridPosition(4, 4), Team.Enemy, 100)
        };
        var stones = new List<CoreStone>
        {
            new CoreStone(new GridPosition(0, 0), Team.Player, 50),
            new CoreStone(new GridPosition(4, 4), Team.Enemy, 50)
        };
        return new GameState(board, units, stones);
    }

    [Test]
    public void NewGame_StartsInActionPhase_WithPlayerTurn()
    {
        var state = CreateState();
        Assert.AreEqual(TurnPhase.Action, state.CurrentPhase);
        Assert.AreEqual(Team.Player, state.CurrentTeamTurn);
    }

    [Test]
    public void EndTurn_SwitchesToEnemy_AndReturnsToActionPhase()
    {
        var state = CreateState();
        new TurnManager(state).EndTurn();
        Assert.AreEqual(Team.Enemy, state.CurrentTeamTurn);
        Assert.AreEqual(TurnPhase.Action, state.CurrentPhase);
    }

    [Test]
    public void EndTurn_Twice_ReturnsToPlayer()
    {
        var state = CreateState();
        var manager = new TurnManager(state);
        manager.EndTurn();
        manager.EndTurn();
        Assert.AreEqual(Team.Player, state.CurrentTeamTurn);
    }
}