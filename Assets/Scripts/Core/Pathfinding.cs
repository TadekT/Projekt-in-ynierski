using System.Collections.Generic;

// algorytmy przeszukiwania planszy
public static class Pathfinding
{
    // stała kolejność sąsiadów: góra, prawo, dół, lewo
    private static readonly GridPosition[] Directions = new GridPosition[]
    {
        new GridPosition(0, 1),
        new GridPosition(1, 0),
        new GridPosition(0, -1),
        new GridPosition(-1, 0)
    };

    // zwraca wszystkie pola, do których jednostka dojdzie w ramach MoveRange,
    // razem z kosztem dojścia, pole startowe ma koszt 0
    public static Dictionary<GridPosition, int> GetReachable(GameState state, Unit unit)
    {
        Dictionary<GridPosition, int> bestCost = new Dictionary<GridPosition, int>();
        List<GridPosition> open = new List<GridPosition>();

        bestCost[unit.Position] = 0;
        open.Add(unit.Position);

        while (open.Count > 0)
        {
            // wybór pola o najmniejszym koszcie dojścia, przy remisie - pierwsze na liście
            int bestIndex = 0;
            for (int i = 1; i < open.Count; i++)
            {
                if (bestCost[open[i]] < bestCost[open[bestIndex]])
                {
                    bestIndex = i;
                }
            }
            GridPosition current = open[bestIndex];
            open.RemoveAt(bestIndex);

            foreach (GridPosition dir in Directions)
            {
                GridPosition next = new GridPosition(current.x + dir.x, current.y + dir.y);

                GridCell cell = state.Board.GetCell(next);
                if (cell == null || !cell.IsWalkable)
                {
                    continue; // poza planszą albo skała
                }
                if (state.IsOccupied(next))
                {
                    continue; // jednostka lub Kamień Źródła
                }

                int newCost = bestCost[current] + cell.MovementCost;
                if (newCost > unit.MoveRange)
                {
                    continue; // nie starcza punktów ruchu
                }

                int oldCost;
                if (bestCost.TryGetValue(next, out oldCost) && oldCost <= newCost)
                {
                    continue; // już znamy tańszą lub równie tanią drogę
                }

                bestCost[next] = newCost;
                if (!open.Contains(next))
                {
                    open.Add(next);
                }
            }
        }

        return bestCost;
    }
    
    public static List<GridPosition> FindPath(GameState state, GridPosition from, GridPosition to)
    {
        if (from == to)
        {
            List<GridPosition> single = new List<GridPosition>();
            single.Add(from);
            return single;
        }

        GridCell targetCell = state.Board.GetCell(to);
        if (targetCell == null || !targetCell.IsWalkable || state.IsOccupied(to))
        {
            return null;
        }

        Dictionary<GridPosition, int> gScore = new Dictionary<GridPosition, int>();
        Dictionary<GridPosition, GridPosition> cameFrom = new Dictionary<GridPosition, GridPosition>();
        List<GridPosition> open = new List<GridPosition>();

        gScore[from] = 0;
        open.Add(from);

        while (open.Count > 0)
        {
            // wybór pola o najmniejszym f = g + h (h: odległość Manhattan do celu);
            // przy remisie: pierwsze na liście
            int bestIndex = 0;
            int bestF = gScore[open[0]] + GridPosition.Manhattan(open[0], to);
            for (int i = 1; i < open.Count; i++)
            {
                int f = gScore[open[i]] + GridPosition.Manhattan(open[i], to);
                if (f < bestF)
                {
                    bestF = f;
                    bestIndex = i;
                }
            }
            GridPosition current = open[bestIndex];
            open.RemoveAt(bestIndex);

            if (current == to)
            {
                return ReconstructPath(cameFrom, current);
            }

            foreach (GridPosition dir in Directions)
            {
                GridPosition next = new GridPosition(current.x + dir.x, current.y + dir.y);

                GridCell cell = state.Board.GetCell(next);
                if (cell == null || !cell.IsWalkable)
                {
                    continue;
                }
                if (state.IsOccupied(next))
                {
                    continue;
                }

                int tentative = gScore[current] + cell.MovementCost;
                int oldCost;
                if (gScore.TryGetValue(next, out oldCost) && oldCost <= tentative)
                {
                    continue;
                }

                cameFrom[next] = current;
                gScore[next] = tentative;
                if (!open.Contains(next))
                {
                    open.Add(next);
                }
            }
        }

        return null; // cel odcięty
    }

    // odtwarza ścieżkę od końca (po wskaźnikach cameFrom), potem odwraca kolejność
    private static List<GridPosition> ReconstructPath(
        Dictionary<GridPosition, GridPosition> cameFrom, GridPosition end)
    {
        List<GridPosition> path = new List<GridPosition>();
        GridPosition current = end;
        path.Add(current);
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }
}