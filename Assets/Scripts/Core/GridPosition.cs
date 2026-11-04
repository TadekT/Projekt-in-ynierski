using System;

public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool Equals(GridPosition other)
    {
        return x == other.x && y == other.y;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition other && Equals(other);
    }

    public override int GetHashCode()
    {
        return unchecked((x * 397) ^ y);
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !a.Equals(b);
    }

    // odległość Manhattan: suma różnic współrzędnych (metryka dla siatki 4-kierunkowej)
    public static int Manhattan(GridPosition a, GridPosition b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }

    public override string ToString()
    {
        return $"Position X : {x}, Position Y : {y}";
    }
}