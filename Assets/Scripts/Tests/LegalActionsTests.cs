using System.Collections.Generic;
using NUnit.Framework;

public class LegalActionsTests
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

    private int Count<T>(List<ICommand> commands)
    {
        int n = 0;
        foreach (ICommand c in commands)
        {
            if (c is T) n++;
        }
        return n;
    }

    [Test]
    public void GetAll_AtStart_HasMovesAndEndTurn_ButNoAttacks()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(8, 8), Team.Enemy);
        GameState state = CreateState(player, enemy);

        List<ICommand> actions = LegalActions.GetAll(state);

        Assert.Greater(Count<MoveCommand>(actions), 0);
        Assert.AreEqual(0, Count<AttackCommand>(actions));
        Assert.AreEqual(1, Count<EndTurnCommand>(actions));
        Assert.IsInstanceOf<EndTurnCommand>(actions[actions.Count - 1]);
    }

    [Test]
    public void GetAll_WarriorOnOpenBoard_Has24Moves()
    {
        
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(8, 8), Team.Enemy);
        GameState state = CreateState(player, enemy);

        List<ICommand> actions = LegalActions.GetAll(state);

        Assert.AreEqual(24, Count<MoveCommand>(actions));
    }

    [Test]
    public void GetAll_EnemyInMeleeRange_HasOneAttack()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(4, 5), Team.Enemy);
        GameState state = CreateState(player, enemy);

        List<ICommand> actions = LegalActions.GetAll(state);

        Assert.AreEqual(1, Count<AttackCommand>(actions));
    }

    [Test]
    public void GetAll_EnemyCoreStoneInRange_HasOneAttack()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(9, 8), Team.Player); 
        Unit enemy = UnitType.Warrior.Create(new GridPosition(5, 5), Team.Enemy);
        GameState state = CreateState(player, enemy);

        List<ICommand> actions = LegalActions.GetAll(state);

        Assert.AreEqual(1, Count<AttackCommand>(actions));
    }

    [Test]
    public void GetAll_ArcherWithClearLine_CanShoot()
    {
        Unit player = UnitType.Archer.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(4, 7), Team.Enemy);
        GameState state = CreateState(player, enemy);

        List<ICommand> actions = LegalActions.GetAll(state);

        Assert.AreEqual(1, Count<AttackCommand>(actions));
    }

    [Test]
    public void GetAll_ArcherBlockedByRock_HasNoAttack()
    {
        Unit player = UnitType.Archer.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(4, 7), Team.Enemy);
        GameState state = CreateState(player, enemy);
        state.Board.SetTerrain(new GridPosition(4, 5), TerrainObject.Rock); 

        List<ICommand> actions = LegalActions.GetAll(state);

        Assert.AreEqual(0, Count<AttackCommand>(actions)); 
    }

    [Test]
    public void GetForUnit_AfterMove_HasNoMoves()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(8, 8), Team.Enemy);
        GameState state = CreateState(player, enemy);
        GameSession session = new GameSession(state);
        session.Submit(new MoveCommand(player, new GridPosition(4, 5)), out _);

        List<ICommand> actions = LegalActions.GetForUnit(state, player);

        Assert.AreEqual(0, actions.Count);
        Assert.AreEqual(1, LegalActions.GetAll(state).Count); 
    }

    [Test]
    public void GetForUnit_AfterAttack_HasNoActions()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(4, 5), Team.Enemy);
        GameState state = CreateState(player, enemy);
        GameSession session = new GameSession(state);
        session.Submit(new AttackCommand(player, enemy), out _);

        List<ICommand> actions = LegalActions.GetForUnit(state, player);

        Assert.AreEqual(0, actions.Count); 
    }

    [Test]
    public void GetAll_AfterEndTurn_GeneratesForEnemyTeam()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(4, 5), Team.Enemy);
        GameState state = CreateState(player, enemy);
        GameSession session = new GameSession(state);
        session.Submit(new EndTurnCommand(Team.Player), out _);

        List<ICommand> actions = LegalActions.GetAll(state);

        Assert.AreEqual(Team.Enemy, state.CurrentTeamTurn);
        Assert.AreEqual(1, Count<AttackCommand>(actions)); 
        Assert.AreEqual(1, Count<EndTurnCommand>(actions));
    }

    [Test]
    public void GetAll_AfterGameOver_IsEmpty()
    {
        Unit player = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit enemy = UnitType.Warrior.Create(new GridPosition(8, 8), Team.Enemy);
        GameState state = CreateState(player, enemy);
        state.CoreStones[1].TakeDamage(999);

        List<ICommand> actions = LegalActions.GetAll(state);

        Assert.IsTrue(state.IsGameOver());
        Assert.AreEqual(0, actions.Count);
    }

    [Test]
    public void GetAll_EveryCommandPassesValidate()
    {
        Unit warrior = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit archer = UnitType.Archer.Create(new GridPosition(3, 4), Team.Player);
        Unit enemyWarrior = UnitType.Warrior.Create(new GridPosition(4, 5), Team.Enemy);
        Unit enemyTank = UnitType.Tank.Create(new GridPosition(3, 7), Team.Enemy);
        GameState state = CreateState(warrior, archer, enemyWarrior, enemyTank);
        state.Board.SetTerrain(new GridPosition(5, 4), TerrainObject.Rock);

        List<ICommand> actions = LegalActions.GetAll(state);

        Assert.Greater(actions.Count, 0);
        foreach (ICommand command in actions)
        {
            bool ok = command.Validate(state, out string error);
            Assert.IsTrue(ok, command.Describe() + " -> " + error);
        }
    }

    [Test]
    public void GetAll_SameState_GivesSameCommandsInSameOrder()
    {
        Unit warrior = UnitType.Warrior.Create(new GridPosition(4, 4), Team.Player);
        Unit archer = UnitType.Archer.Create(new GridPosition(3, 4), Team.Player);
        Unit enemyWarrior = UnitType.Warrior.Create(new GridPosition(4, 5), Team.Enemy);
        GameState state = CreateState(warrior, archer, enemyWarrior);

        List<ICommand> first = LegalActions.GetAll(state);
        List<ICommand> second = LegalActions.GetAll(state);

        Assert.AreEqual(first.Count, second.Count);
        for (int i = 0; i < first.Count; i++)
        {
            Assert.AreEqual(first[i].Describe(), second[i].Describe());
        }
    }
}