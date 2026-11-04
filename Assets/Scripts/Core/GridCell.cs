

public class GridCell
{
    public GridPosition Position { get; private set; }
    public bool IsWalkable { get; private set; }

    public GridCell(GridPosition position)
    {
        this.Position = position;
        this.IsWalkable = true;
    }

    public void SetWalkable(bool value)
    {
        this.IsWalkable = value;
    }

    public override string ToString()
    {
        return $"Cell : [{Position}], Walkable = {IsWalkable}";
    }
}