using System.Collections;
using System.Collections.Generic;


public class MoveCommand : ICommand
{
    private readonly Unit unit;
    private readonly GridPosition target;
    private GridPosition origin; // zapisywane w Execute, potrzebne do opisu w logu

    public MoveCommand(Unit unit, GridPosition target)
    {
        this.unit = unit;
        this.target = target;
    }

    public bool Validate(GameState state, out string error)
    {
        if (unit == null || !state.Units.Contains(unit))
        {
            error = "Jednostka nie należy do tej gry.";
            return false;
        }
        if (unit.IsDestroyed)
        {
            error = "Jednostka jest zniszczona.";
            return false;
        }
        if (unit.TeamSide != state.CurrentTeamTurn)
        {
            error = "To nie jest tura tej jednostki.";
            return false;
        }
        if (state.CurrentPhase != TurnPhase.Action)
        {
            error = "Tura nie jest w fazie akcji.";
            return false;
        }
        if (unit.HasMoved || unit.HasActed)
        {
            error = "Jednostka nie może już się ruszyć w tej turze.";
            return false;
        }

        GridCell cell = state.Board.GetCell(target);
        if (cell == null)
        {
            error = "Pole poza planszą.";
            return false;
        }
        if (!cell.IsWalkable)
        {
            error = "Pole jest nieprzekraczalne.";
            return false;
        }
        if (state.IsOccupied(target))
        {
            error = "Pole jest zajęte.";
            return false;
        }
        
        Dictionary<GridPosition, int> reachable = Pathfinding.GetReachable(state, unit);
        if (!reachable.ContainsKey(target))
        {
            error = "Pole jest poza zasięgiem ruchu.";
            return false;
        }
        error = null;
        return true;
    }

    public void Execute(GameState state)
    {
        origin = unit.Position;
        unit.MoveTo(target);
        unit.MarkMoved();
    }

    public string Describe()
    {
        return $"{unit.TeamSide}: ruch {origin} -> {target}";
    }
}