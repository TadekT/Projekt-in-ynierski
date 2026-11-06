using NUnit.Framework;

public class TerrainTests
{
    [Test]
    public void NewCell_IsPlain_WalkableWithCostOne()
    {
        Board board = new Board(5, 5);
        GridCell cell = board.GetCell(new GridPosition(2, 2));

        Assert.IsTrue(cell.IsWalkable);
        Assert.AreEqual(1, cell.MovementCost);
    }

    [Test]
    public void Swamp_IsWalkable_CostsTwo()
    {
        Board board = new Board(5, 5);
        board.SetTerrain(new GridPosition(1, 1), TerrainObject.Swamp);
        GridCell cell = board.GetCell(new GridPosition(1, 1));

        Assert.IsTrue(cell.IsWalkable);
        Assert.AreEqual(2, cell.MovementCost);
    }

    [Test]
    public void Rock_IsNotWalkable_BlocksLineOfSight()
    {
        Board board = new Board(5, 5);
        board.SetTerrain(new GridPosition(3, 3), TerrainObject.Rock);
        GridCell cell = board.GetCell(new GridPosition(3, 3));

        Assert.IsFalse(cell.IsWalkable);
        Assert.IsTrue(cell.Terrain.BlockLineOfSight);
    }

    [Test]
    public void SetTerrain_OutOfBounds_ReturnsFalse()
    {
        Board board = new Board(5, 5);

        Assert.IsFalse(board.SetTerrain(new GridPosition(9, 9), TerrainObject.Rock));
    }
}