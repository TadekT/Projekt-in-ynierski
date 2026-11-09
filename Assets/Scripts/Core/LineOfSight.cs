using System;
using System.Collections.Generic;

public static class LineOfSight
{
    // Zwraca wszystkie pola odcinka a-b (z końcami), w kanonicznej kolejności.
    public static List<GridPosition> GetLine(GridPosition a, GridPosition b)
    {
        // Kanoniczna kolejność końców: mniejszy X, a przy remisie mniejszy Y.
        // Dzięki temu GetLine(a, b) i GetLine(b, a) dają ten sam zbiór pól.
        GridPosition start = a;
        GridPosition end = b;
        if (a.x > b.x || (a.x == b.x && a.y > b.y))
        {
            start = b;
            end = a;
        }

        int x0 = start.x;
        int y0 = start.y;
        int x1 = end.x;
        int y1 = end.y;

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        List<GridPosition> line = new List<GridPosition>();

        while (true)
        {
            line.Add(new GridPosition(x0, y0));
            if (x0 == x1 && y0 == y1)
            {
                break;
            }

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        return line;
    }

    // Czy między a i b jest czysta linia strzału.
    public static bool HasLine(Board board, GridPosition a, GridPosition b)
    {
        List<GridPosition> line = GetLine(a, b);

        for (int i = 1; i < line.Count - 1; i++)
        {
            GridCell cell = board.GetCell(line[i]);
            if (cell.Terrain.BlockLineOfSight)
            {
                return false;
            }
        }

        return true;
    }
}