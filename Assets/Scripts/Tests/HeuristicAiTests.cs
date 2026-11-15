using System.Collections.Generic;
using NUnit.Framework;

public class HeuristicAiTests
{
    private GameState CreateState(params Unit[] units)
    {
        List<CoreStone> stones = new List<CoreStone>
        {
            new CoreStone(new GridPosition(0, 0), Team.Player, 50),
            new CoreStone(new GridPosition(9, 9), Team.Enemy, 50)
        };
        return new GameState(new Board(10, 10), new List<Unit>(units), stones);
    }

    [Test]
    public void ChooseNextCommand_NotMyTurn_ReturnsNull()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(8, 8), Team.Enemy);
        GameState state = CreateState(player, enemy);
        HeuristicAi enemyAi = new HeuristicAi(Team.Enemy);

        Assert.IsNull(enemyAi.ChooseNextCommand(state));
    }

    [Test]
    public void ChooseNextCommand_AfterGameOver_ReturnsNull()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(8, 8), Team.Enemy);
        GameState state = CreateState(player, enemy);
        state.CoreStones[1].TakeDamage(999);
        HeuristicAi ai = new HeuristicAi(Team.Player);

        Assert.IsNull(ai.ChooseNextCommand(state));
    }

    [Test]
    public void ChooseNextCommand_CanKill_PrefersKillOverPlainHit()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit healthy = UnitType.Warrior.Create(new GridPosition(4, 5), Team.Enemy);
        Unit weak = UnitType.Warrior.Create(new GridPosition(3, 4), Team.Enemy);
        weak.TakeDamage(80); // zostaje 20 HP, jeden cios zabija
        GameState state = CreateState(player, healthy, weak);
        HeuristicAi ai = new HeuristicAi(Team.Player);

        AttackCommand attack = ai.ChooseNextCommand(state) as AttackCommand;

        Assert.IsNotNull(attack);
        Assert.AreSame(weak, attack.Target);
    }

    [Test]
    public void ChooseNextCommand_StoneAndUnitInRange_PrefersCoreStone()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(9, 8), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(8, 8), Team.Enemy);
        GameState state = CreateState(player, enemy);
        HeuristicAi ai = new HeuristicAi(Team.Player);

        AttackCommand attack = ai.ChooseNextCommand(state) as AttackCommand;

        Assert.IsNotNull(attack);
        Assert.IsInstanceOf<CoreStone>(attack.Target); 
    }

    [Test]
    public void ChooseNextCommand_CanDestroyCoreStone_DoesItEvenIfUnitCanDie()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(9, 8), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(8, 8), Team.Enemy);
        enemy.TakeDamage(90);                 
        GameState state = CreateState(player, enemy);
        state.CoreStones[1].TakeDamage(25);   
        HeuristicAi ai = new HeuristicAi(Team.Player);

        AttackCommand attack = ai.ChooseNextCommand(state) as AttackCommand;

        Assert.IsNotNull(attack);
        Assert.IsInstanceOf<CoreStone>(attack.Target);
    }

    [Test]
    public void ChooseNextCommand_NothingToAttack_MovesTowardEnemyStone()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(0, 9), Team.Enemy);
        GameState state = CreateState(player, enemy);
        HeuristicAi ai = new HeuristicAi(Team.Player);

        MoveCommand move = ai.ChooseNextCommand(state) as MoveCommand;

        Assert.IsNotNull(move);
        Assert.AreSame(player, move.MovingUnit);
        Assert.AreEqual(7, GridPosition.Manhattan(move.Destination, new GridPosition(9, 9)));
    }

    [Test]
    public void ChooseNextCommand_EqualAlternatives_AvoidsThreatenedCell()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(2, 9), Team.Enemy);
        GameState state = CreateState(player, enemy);
        HeuristicAi ai = new HeuristicAi(Team.Player);

        MoveCommand move = ai.ChooseNextCommand(state) as MoveCommand;

        Assert.IsNotNull(move);
        Assert.AreNotEqual(new GridPosition(4, 7), move.Destination);
        Assert.AreEqual(new GridPosition(5, 6), move.Destination);
    }

    [Test]
    public void ChooseNextCommand_AfterMoveWithNothingToAttack_EndsTurn()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(0, 9), Team.Enemy);
        GameState state = CreateState(player, enemy);
        GameSession session = new GameSession(state);
        session.Submit(new MoveCommand(player, new GridPosition(4, 5)), out _);
        HeuristicAi ai = new HeuristicAi(Team.Player);

        ICommand command = ai.ChooseNextCommand(state);

        Assert.IsInstanceOf<EndTurnCommand>(command);
    }

    [Test]
    public void ChooseNextCommand_SameState_GivesSameCommand()
    {
        GameState first = CreateMatchState();
        GameState second = CreateMatchState();
        HeuristicAi ai = new HeuristicAi(Team.Player);

        string a = ai.ChooseNextCommand(first).Describe();
        string b = ai.ChooseNextCommand(second).Describe();

        Assert.AreEqual(a, b);
    }

    private GameState CreateMatchState()
    {
        List<Unit> units = new List<Unit>
        {
            UnitType.Warrior.Create(new GridPosition(1, 0), Team.Player),
            UnitType.Warrior.Create(new GridPosition(0, 1), Team.Player),
            UnitType.Archer.Create(new GridPosition(1, 1), Team.Player),
            UnitType.Tank.Create(new GridPosition(2, 2), Team.Player),
            UnitType.Warrior.Create(new GridPosition(8, 9), Team.Enemy),
            UnitType.Warrior.Create(new GridPosition(9, 8), Team.Enemy),
            UnitType.Archer.Create(new GridPosition(8, 8), Team.Enemy),
            UnitType.Tank.Create(new GridPosition(7, 7), Team.Enemy)
        };
        List<CoreStone> stones = new List<CoreStone>
        {
            new CoreStone(new GridPosition(0, 0), Team.Player, 50),
            new CoreStone(new GridPosition(9, 9), Team.Enemy, 50)
        };
        return new GameState(new Board(10, 10), units, stones);
    }

    private int PlayMatch(GameState state, int maxTurns)
    {
        GameSession session = new GameSession(state);
        IAiPlayer playerAi = new HeuristicAi(Team.Player);
        IAiPlayer enemyAi = new HeuristicAi(Team.Enemy);

        int turns = 0;
        int commands = 0;

        while (!state.IsGameOver() && turns < maxTurns)
        {
            IAiPlayer ai = state.CurrentTeamTurn == Team.Player ? playerAi : enemyAi;
            ICommand command = ai.ChooseNextCommand(state);
            Assert.IsNotNull(command);

            string error;
            bool ok = session.Submit(command, out error);
            Assert.IsTrue(ok, command.Describe() + " -> " + error);

            if (command is EndTurnCommand)
            {
                turns++;
            }
            commands++;
            Assert.Less(commands, 5000, "Za dużo komend - pętla bez końca?");
        }
        return turns;
    }

    [Test]
    public void FullMatch_AiVsAi_EndsWithWinnerBeforeTurnLimit()
    {
        GameState state = CreateMatchState();

        int turns = PlayMatch(state, 100);

        Assert.IsTrue(state.IsGameOver(), "Brak rozstrzygnięcia po " + turns + " turach (pat albo remis).");
        Assert.IsNotNull(state.GetWinner());
    }

    [Test]
    public void FullMatch_AiVsAi_IsDeterministic()
    {
        GameState first = CreateMatchState();
        GameState second = CreateMatchState();

        int turnsA = PlayMatch(first, 100);
        int turnsB = PlayMatch(second, 100);

        Assert.AreEqual(turnsA, turnsB);
        Assert.AreEqual(first.GetWinner(), second.GetWinner());
    }
}