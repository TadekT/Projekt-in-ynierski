using System.Collections.Generic;
using NUnit.Framework;

public class AttackCommandTests
{
    // buduje grę z podanymi jednostkami; Kamienie stoją w rogach (0,0) i (9,9)
    private GameSession CreateSession(
        Unit player,
        Unit enemy,
        out GameState state,
        out List<CoreStone> stones)
    {
        List<Unit> units = new List<Unit> { player, enemy };
        stones = new List<CoreStone>
        {
            new CoreStone(new GridPosition(0, 0), Team.Player, 50),
            new CoreStone(new GridPosition(9, 9), Team.Enemy, 50)
        };
        state = new GameState(new Board(10, 10), units, stones);
        return new GameSession(state);
    }

    [Test]
    public void Submit_AttackEnemyInRange_DealsDamage_AndMarksActed()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(4, 5), Team.Enemy);
        GameSession session = CreateSession(player, enemy, out _, out _);

        bool ok = session.Submit(new AttackCommand(player, enemy), out string error);

        Assert.IsTrue(ok);
        Assert.IsNull(error);
        Assert.AreEqual(75, enemy.CurrentHp); // 100 - 25 obrażeń wojownika
        Assert.IsTrue(player.HasActed);
        Assert.AreEqual(1, session.Log.Count);
    }

    [Test]
    public void Submit_AttackOutOfRange_IsRejected_AndNothingChanges()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(4, 6), Team.Enemy); // Manhattan 2, zasięg 1
        GameSession session = CreateSession(player, enemy, out _, out _);

        bool ok = session.Submit(new AttackCommand(player, enemy), out string error);

        Assert.IsFalse(ok);
        Assert.IsNotNull(error);
        Assert.AreEqual(100, enemy.CurrentHp);
        Assert.IsFalse(player.HasActed);
        Assert.AreEqual(0, session.Log.Count);
    }

    [Test]
    public void Submit_ArcherAttacksAtDistanceThree_Works()
    {
        Unit player = UnitType.Archer.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(4, 7), Team.Enemy); // Manhattan 3
        GameSession session = CreateSession(player, enemy, out _, out _);

        bool ok = session.Submit(new AttackCommand(player, enemy), out string error);

        Assert.IsTrue(ok);
        Assert.AreEqual(80, enemy.CurrentHp); // 100 - 20 obrażeń łucznika
    }

    [Test]
    public void Submit_AttackOwnUnit_IsRejected()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(8, 8), Team.Enemy);
        GameSession session = CreateSession(player, enemy, out GameState state, out _);
        Unit ally = UnitType.Warrior.Create(new GridPosition(4, 5), Team.Player);
        state.Units.Add(ally);

        bool ok = session.Submit(new AttackCommand(player, ally), out string error);

        Assert.IsFalse(ok);
        Assert.AreEqual(100, ally.CurrentHp);
    }

    [Test]
    public void Submit_SecondAttackInSameTurn_IsRejected()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(4, 5), Team.Enemy);
        GameSession session = CreateSession(player, enemy, out _, out _);
        session.Submit(new AttackCommand(player, enemy), out _);

        bool ok = session.Submit(new AttackCommand(player, enemy), out string error);

        Assert.IsFalse(ok);
        Assert.AreEqual(75, enemy.CurrentHp); // tylko pierwszy atak zadziałał
    }

    [Test]
    public void Submit_EnemyAttacksDuringPlayerTurn_IsRejected()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(4, 5), Team.Enemy);
        GameSession session = CreateSession(player, enemy, out _, out _);

        bool ok = session.Submit(new AttackCommand(enemy, player), out string error);

        Assert.IsFalse(ok);
        Assert.AreEqual(100, player.CurrentHp);
    }

    [Test]
    public void Submit_AttackDestroyedTarget_IsRejected()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(4, 5), Team.Enemy);
        GameSession session = CreateSession(player, enemy, out _, out _);
        enemy.TakeDamage(999);

        bool ok = session.Submit(new AttackCommand(player, enemy), out string error);

        Assert.IsFalse(ok);
        Assert.IsFalse(player.HasActed);
    }

    [Test]
    public void Submit_MoveAfterAttack_IsRejected()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(4, 5), Team.Enemy);
        GameSession session = CreateSession(player, enemy, out _, out _);
        session.Submit(new AttackCommand(player, enemy), out _);

        bool ok = session.Submit(new MoveCommand(player, new GridPosition(3, 4)), out string error);

        Assert.IsFalse(ok);
        Assert.AreEqual(new GridPosition(4, 4), player.Position);
    }

    [Test]
    public void Submit_AttackEnemyCoreStone_ReducesItsHp()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(9, 8), Team.Player); // obok Kamienia na (9,9)
        Unit enemy = UnitType.Warrior.Create(new GridPosition(5, 5), Team.Enemy);
        GameSession session = CreateSession(player, enemy, out GameState state, out List<CoreStone> stones);

        bool ok = session.Submit(new AttackCommand(player, stones[1]), out string error);

        Assert.IsTrue(ok);
        Assert.AreEqual(25, stones[1].CurrentHp); // 50 - 25
        Assert.IsFalse(state.IsGameOver());
    }

    [Test]
    public void Submit_DestroyingCoreStone_EndsGame_AndFurtherCommandsAreRejected()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(9, 8), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(5, 5), Team.Enemy);
        GameSession session = CreateSession(player, enemy, out GameState state, out List<CoreStone> stones);
        stones[1].TakeDamage(25); // zostaje 25 HP, czyli dokładnie jeden atak wojownika

        bool ok = session.Submit(new AttackCommand(player, stones[1]), out string error);

        Assert.IsTrue(ok);
        Assert.IsTrue(state.IsGameOver());
        Assert.AreEqual(Team.Player, state.GetWinner());

        bool afterEnd = session.Submit(new EndTurnCommand(Team.Player), out string error2);
        Assert.IsFalse(afterEnd);
    }
}