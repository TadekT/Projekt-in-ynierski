using System.Collections.Generic;
public class Board
{

    public int Width {get ; private set;}
    public int Height {get ; private set;}
    private GridCell[,] cells;

    public Board(int width, int height)
    {
        
        this.Width = width;
        this.Height = height;
        this.cells = new GridCell[width,height];

        // wypełnianie tablicy komurkami 
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                // tworzenie komórki z pozycją i wstawianie jej do tablicy
                cells[x,y] = new GridCell(new GridPosition(x,y));
            }
        }
    }


    public List<GridCell> GetNeighbors(GridPosition pos)
    {
        List<GridCell> neighbors = new List<GridCell>();

        GridPosition right = new GridPosition(pos.x + 1, pos.y); 
        GridPosition left = new GridPosition(pos.x - 1, pos.y);
        GridPosition up = new GridPosition(pos.x, pos.y + 1);
        GridPosition down = new GridPosition(pos.x, pos.y - 1);

        GridCell cellRight = GetCell(right);
        if(cellRight != null)
        {
            neighbors.Add(cellRight);
        }

        GridCell cellLeft = GetCell(left);
        if(cellLeft != null)
        {
            neighbors.Add(cellLeft);
        }

        GridCell cellUp = GetCell(up);
        if(cellUp != null)
        {
            neighbors.Add(cellUp);
        }

        GridCell cellDown = GetCell(down);
        if(cellDown != null)
        {
            neighbors.Add(cellDown);
        }

        return neighbors;
    }






    public bool IsInBounds(GridPosition pos)
    {
        return pos.x >= 0 && pos.x < Width && pos.y >=0 && pos.y < Height;
    }

    public GridCell GetCell(GridPosition pos)
    {

        if (!IsInBounds(pos))
        {
            return null;
        }
        return cells[pos.x,pos.y];

    }



}
