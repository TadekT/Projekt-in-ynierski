using System;

public class GridCell
{
    public GridPosition Position { get; private set; }
    public TerrainObject Terrain { get; private set; }

    // jedno źródło prawdy: przekraczalność i koszt wynikają z terenu
    public bool IsWalkable
    {
        get { return Terrain.IsPassable; }
    }

    public int MovementCost
    {
        get { return Terrain.MovementCost; }
    }

    public GridCell(GridPosition position)
    {
        this.Position = position;
        this.Terrain = TerrainObject.Plain;
    }

    public void SetTerrain(TerrainObject terrain)
    {
        if (terrain == null)
        {
            throw new ArgumentNullException(nameof(terrain));
        }
        this.Terrain = terrain;
    }

    public override string ToString()
    {
        return $"Cell : [{Position}], Terrain = {Terrain.Name}, Walkable = {IsWalkable}";
    }
}