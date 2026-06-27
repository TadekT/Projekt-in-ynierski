
public class GridCell
{

    public GridPosition position;
    public bool isWalkable;
    public bool isOccupied ;

    public GridCell(GridPosition position)
    {
        
        this.position = position;
        this.isWalkable = true;
        this.isOccupied = false;

    }

    public override string ToString()
    {
        return $"Cell : [{position}], Walkable = {isWalkable}, Occupite = {isOccupied}";
    }



}
