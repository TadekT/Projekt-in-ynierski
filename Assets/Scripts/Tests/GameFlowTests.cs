using System.Collections.Generic;
using NUnit.Framework;

public class GameFlowTests
{
    [Test]
    public void FullGame_MoveAttackEndTurn_UntilCoreStoneDestroyed()
    {
        // wojownik gracza musi dojść do Kamienia wroga na (9,9) i zadać dwa ciosy po 25 obrażeń
        Unit player = UnitType.Warrior.Create(new GridPosition(9, 6), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(0, 5), Team.Enemy);
        List<Unit> units = new List<Unit> { player, enemy };
        List<CoreStone> stones = new List<CoreStone>
        {
            new CoreStone(new GridPosition(0, 0), Team.Player, 50),
            new CoreStone(new GridPosition(9, 9), Team.Enemy, 50)
        };
        GameState state = new GameState(new Board(10, 10), units, stones);
        GameSession session = new GameSession(state);
        string error;

        // tura 1 gracza: ruch na (9,7), koniec tury
        Assert.IsTrue(session.Submit(new MoveCommand(player, new GridPosition(9, 7)), out error), error);
        Assert.IsTrue(session.Submit(new EndTurnCommand(Team.Player), out error), error);

        // tura wroga: tylko koniec tury
        Assert.IsTrue(session.Submit(new EndTurnCommand(Team.Enemy), out error), error);

        // tura 2 gracza: ruch na (9,8), pierwszy atak na Kamień, koniec tury
        Assert.IsTrue(session.Submit(new MoveCommand(player, new GridPosition(9, 8)), out error), error);
        Assert.IsTrue(session.Submit(new AttackCommand(player, stones[1]), out error), error);
        Assert.AreEqual(25, stones[1].CurrentHp);
        Assert.IsFalse(state.IsGameOver());
        Assert.IsTrue(session.Submit(new EndTurnCommand(Team.Player), out error), error);

        // tura wroga: koniec tury
        Assert.IsTrue(session.Submit(new EndTurnCommand(Team.Enemy), out error), error);

        // tura 3 gracza: drugi atak niszczy Kamień
        Assert.IsTrue(session.Submit(new AttackCommand(player, stones[1]), out error), error);

        Assert.IsTrue(state.IsGameOver());
        Assert.AreEqual(Team.Player, state.GetWinner());
        Assert.AreEqual(8, session.Log.Count);

        // po końcu gry nic już nie przechodzi
        Assert.IsFalse(session.Submit(new EndTurnCommand(Team.Player), out error));
    }
}