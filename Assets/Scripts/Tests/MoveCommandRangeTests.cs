using System.Collections.Generic;
using NUnit.Framework;

public class MoveCommandRangeTests
{
    private GameState CreateState(Board board, Unit unit)
    {
        List<Unit> units = new List<Unit>();
        units.Add(unit);
        return new GameState(board, units, new List<CoreStone>());
    }

    [Test]
    public void Validate_TargetWithinRange_ReturnsTrue()
    {
        Board board = new Board(5, 5);
        Unit unit = new Unit(new GridPosition(2, 2), Team.Player, 100, 2, 1, 25);
        GameState state = CreateState(board, unit);
        MoveCommand command = new MoveCommand(unit, new GridPosition(4, 2));

        string error;
        bool valid = command.Validate(state, out error);

        Assert.IsTrue(valid);
    }

    [Test]
    public void Validate_TargetBeyondRange_ReturnsFalse()
    {
        Board board = new Board(5, 5);
        Unit unit = new Unit(new GridPosition(2, 2), Team.Player, 100, 2, 1, 25);
        GameState state = CreateState(board, unit);
        MoveCommand command = new MoveCommand(unit, new GridPosition(4, 3));

        string error;
        bool valid = command.Validate(state, out error);

        Assert.IsFalse(valid);
    }

    [Test]
    public void Validate_SwampMakesTargetTooExpensive_ReturnsFalse()
    {
        Board board = new Board(5, 5);
        board.SetTerrain(new GridPosition(3, 2), TerrainObject.Swamp);
        Unit unit = new Unit(new GridPosition(2, 2), Team.Player, 100, 2, 1, 25);
        GameState state = CreateState(board, unit);
        MoveCommand command = new MoveCommand(unit, new GridPosition(4, 2));

        string error;
        bool valid = command.Validate(state, out error);

        Assert.IsFalse(valid);
    }
}