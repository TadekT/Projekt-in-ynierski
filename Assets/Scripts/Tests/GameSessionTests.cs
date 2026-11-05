using System.Collections.Generic;
using NUnit.Framework;

public class GameSessionTests
{
    // pomocnicza metoda: buduje małą grę i sesję, żeby każdy test zaczynał od tego samego stanu
    private GameSession CreateSession(out GameState state, out List<CoreStone> stones)
    {
        List<Unit> units = new List<Unit>
        {
            new Unit(new GridPosition(1, 1), Team.Player, 100),
            new Unit(new GridPosition(8, 8), Team.Enemy, 100)
        };
        stones = new List<CoreStone>
        {
            new CoreStone(new GridPosition(0, 0), Team.Player, 50),
            new CoreStone(new GridPosition(9, 9), Team.Enemy, 50)
        };
        state = new GameState(new Board(10, 10), units, stones);
        return new GameSession(state);
    }

    [Test]
    public void Submit_EndTurnByActiveTeam_SwitchesTeam_AndIsLogged()
    {
        GameSession session = CreateSession(out GameState state, out _);

        bool ok = session.Submit(new EndTurnCommand(Team.Player), out string error);

        Assert.IsTrue(ok);
        Assert.IsNull(error);
        Assert.AreEqual(Team.Enemy, state.CurrentTeamTurn);
        Assert.AreEqual(1, session.Log.Count);
    }

    [Test]
    public void Submit_EndTurnByWrongTeam_IsRejected_AndStateUnchanged()
    {
        GameSession session = CreateSession(out GameState state, out _);

        bool ok = session.Submit(new EndTurnCommand(Team.Enemy), out string error);

        Assert.IsFalse(ok);
        Assert.IsNotNull(error);
        Assert.AreEqual(Team.Player, state.CurrentTeamTurn);
        Assert.AreEqual(0, session.Log.Count);
    }

    [Test]
    public void Submit_AfterGameOver_IsRejected()
    {
        GameSession session = CreateSession(out GameState state, out List<CoreStone> stones);
        stones[1].TakeDamage(999); // niszczymy Kamień wroga

        bool ok = session.Submit(new EndTurnCommand(Team.Player), out string error);

        Assert.IsFalse(ok);
        Assert.IsNotNull(error);
        Assert.AreEqual(Team.Player, state.CurrentTeamTurn);
    }

    [Test]
    public void Validate_DoesNotChangeState()
    {
        CreateSession(out GameState state, out _);

        new EndTurnCommand(Team.Player).Validate(state, out _);

        Assert.AreEqual(Team.Player, state.CurrentTeamTurn);
        Assert.AreEqual(TurnPhase.Action, state.CurrentPhase);
    }
}