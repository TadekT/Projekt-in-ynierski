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
}