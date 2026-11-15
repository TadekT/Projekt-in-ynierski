using System.Collections.Generic;


public static class LegalActions
{

    public static List<ICommand> GetAll(GameState state)
    {
        List<ICommand> result = new List<ICommand>();

        if (state.IsGameOver())
        {
            return result;
        }

        foreach (Unit unit in state.Units)
        {
            if (unit.IsDestroyed || unit.TeamSide != state.CurrentTeamTurn)
            {
                continue;
            }
            result.AddRange(GetForUnit(state, unit));
        }

        AddIfLegal(new EndTurnCommand(state.CurrentTeamTurn), state, result);
        return result;
    }

    // legalne ruchy i ataki jednostki
    public static List<ICommand> GetForUnit(GameState state, Unit unit)
    {
        List<ICommand> result = new List<ICommand>();

        if (state.IsGameOver() || unit == null || unit.IsDestroyed)
        {
            return result;
        }

        AddMoves(state, unit, result);
        AddAttacks(state, unit, result);
        return result;
    }

    private static void AddMoves(GameState state, Unit unit, List<ICommand> result)
    {
        // czy jednostka ma ruch
        if (unit.HasMoved || unit.HasActed)
        {
            return;
        }

        Dictionary<GridPosition, int> reachable = Pathfinding.GetReachable(state, unit);

        // stała kolejność pól x, potem y
        List<GridPosition> cells = new List<GridPosition>(reachable.Keys);
        cells.Sort((a, b) => a.x != b.x ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));

        foreach (GridPosition cell in cells)
        {
            if (cell == unit.Position)
            {
                continue;
            }
            AddIfLegal(new MoveCommand(unit, cell), state, result);
        }
    }

    private static void AddAttacks(GameState state, Unit unit, List<ICommand> result)
    {
        foreach (Unit target in state.Units)
        {
            if (target == unit)
            {
                continue;
            }
            AddIfLegal(new AttackCommand(unit, target), state, result);
        }

        foreach (CoreStone stone in state.CoreStones)
        {
            AddIfLegal(new AttackCommand(unit, stone), state, result);
        }
    }

    private static void AddIfLegal(ICommand command, GameState state, List<ICommand> result)
    {
        string error;
        if (command.Validate(state, out error))
        {
            result.Add(command);
        }
    }
}