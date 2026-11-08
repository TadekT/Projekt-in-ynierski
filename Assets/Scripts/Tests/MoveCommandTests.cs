using System.Collections.Generic;
using NUnit.Framework;

public class MoveCommandTests
{
    // Test małej gry z jednostkami gracza i wroga na zadanych polach, kamienie stoją w przeciwwległych rogach
    private GameSession CreateSession(
        GridPosition playerPos,
        GridPosition enemyPos,
        out GameState state,
        out Unit player,
        out Unit enemy)
    {
        player = new Unit(playerPos, Team.Player, 100);
        enemy = new Unit(enemyPos, Team.Enemy, 100);
        List<Unit> units = new List<Unit> { player, enemy };
        List<CoreStone> stones = new List<CoreStone>
        {
            new CoreStone(new GridPosition(0, 0), Team.Player, 50),
            new CoreStone(new GridPosition(9, 9), Team.Enemy, 50)
        };
        state = new GameState(new Board(10, 10), units, stones);
        return new GameSession(state);
    }

    [Test]
    public void Submit_MoveToFreeNeighbor_MovesUnit_AndMarksMoved()
    {
        GameSession session = CreateSession(new GridPosition(1, 1), new GridPosition(8, 8),
            out GameState state, out Unit player, out _);

        bool ok = session.Submit(new MoveCommand(player, new GridPosition(1, 2)), out string error);

        Assert.IsTrue(ok);
        Assert.IsNull(error);
        Assert.AreEqual(new GridPosition(1, 2), player.Position);
        Assert.IsTrue(player.HasMoved);
        Assert.AreEqual(1, session.Log.Count);
    }

    [Test]
    public void Submit_MoveBeyondRange_IsRejected_AndUnitStays()
    {
        GameSession session = CreateSession(new GridPosition(1, 1), new GridPosition(8, 8),
            out GameState state, out Unit player, out _);

        bool ok = session.Submit(new MoveCommand(player, new GridPosition(1, 6)), out string error);

        Assert.IsFalse(ok);
        Assert.IsNotNull(error);
        Assert.AreEqual(new GridPosition(1, 1), player.Position);
        Assert.IsFalse(player.HasMoved);
        Assert.AreEqual(0, session.Log.Count);
    }

    [Test]
    public void Submit_MoveOutsideBoard_IsRejected()
    {
        GameSession session = CreateSession(new GridPosition(0, 5), new GridPosition(8, 8),
            out GameState state, out Unit player, out _);

        bool ok = session.Submit(new MoveCommand(player, new GridPosition(-1, 5)), out string error);

        Assert.IsFalse(ok);
        Assert.AreEqual(new GridPosition(0, 5), player.Position);
    }

    [Test]
    public void Submit_MoveOntoOtherUnit_IsRejected()
    {
        GameSession session = CreateSession(new GridPosition(4, 4), new GridPosition(4, 5),
            out GameState state, out Unit player, out _);

        bool ok = session.Submit(new MoveCommand(player, new GridPosition(4, 5)), out string error);

        Assert.IsFalse(ok);
        Assert.AreEqual(new GridPosition(4, 4), player.Position);
    }

    [Test]
    public void Submit_MoveOntoCoreStone_IsRejected()
    {
        // Kamień gracza stoi na (0,0), jednostka na sąsiednim (1,0)
        GameSession session = CreateSession(new GridPosition(1, 0), new GridPosition(8, 8),
            out GameState state, out Unit player, out _);

        bool ok = session.Submit(new MoveCommand(player, new GridPosition(0, 0)), out string error);

        Assert.IsFalse(ok);
        Assert.AreEqual(new GridPosition(1, 0), player.Position);
    }

    [Test]
    public void Submit_MoveOntoUnwalkableCell_IsRejected()
    {
        GameSession session = CreateSession(new GridPosition(4, 4), new GridPosition(8, 8),
            out GameState state, out Unit player, out _);
        state.Board.GetCell(new GridPosition(4, 5)).SetTerrain(TerrainObject.Rock);

        bool ok = session.Submit(new MoveCommand(player, new GridPosition(4, 5)), out string error);

        Assert.IsFalse(ok);
        Assert.AreEqual(new GridPosition(4, 4), player.Position);
    }

    [Test]
    public void Submit_MoveEnemyUnitDuringPlayerTurn_IsRejected()
    {
        GameSession session = CreateSession(new GridPosition(1, 1), new GridPosition(8, 8),
            out GameState state, out _, out Unit enemy);

        bool ok = session.Submit(new MoveCommand(enemy, new GridPosition(8, 7)), out string error);

        Assert.IsFalse(ok);
        Assert.AreEqual(new GridPosition(8, 8), enemy.Position);
    }

    [Test]
    public void Submit_SecondMoveInSameTurn_IsRejected()
    {
        GameSession session = CreateSession(new GridPosition(1, 1), new GridPosition(8, 8),
            out GameState state, out Unit player, out _);
        session.Submit(new MoveCommand(player, new GridPosition(1, 2)), out _);

        bool ok = session.Submit(new MoveCommand(player, new GridPosition(1, 3)), out string error);

        Assert.IsFalse(ok);
        Assert.AreEqual(new GridPosition(1, 2), player.Position);
    }

    [Test]
    public void Submit_MoveAfterAttack_IsRejected()
    {
        GameSession session = CreateSession(new GridPosition(1, 1), new GridPosition(8, 8),
            out GameState state, out Unit player, out _);
        player.MarkActed(); 

        bool ok = session.Submit(new MoveCommand(player, new GridPosition(1, 2)), out string error);

        Assert.IsFalse(ok);
        Assert.AreEqual(new GridPosition(1, 1), player.Position);
    }
}