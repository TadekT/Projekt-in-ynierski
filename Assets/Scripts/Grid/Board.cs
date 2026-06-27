
public class Board
{

    public int width;
    public int height;
    private GridCell[,] cells;


    public Board(int width, int height)
    {
        
        this.width = width;
        this.height = height;
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

    public bool IsInBounds(GridPosition pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >=0 && pos.y < height;
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
