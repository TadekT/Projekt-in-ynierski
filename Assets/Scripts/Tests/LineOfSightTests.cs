using NUnit.Framework;

public class LineOfSightTests
{
    private Board CreateBoard()
    {
        return new Board(10, 10);
    }

    [Test]
    public void HasLine_NoObstacles_ReturnsTrue()
    {
        Board board = CreateBoard();
        Assert.IsTrue(LineOfSight.HasLine(board, new GridPosition(0, 0), new GridPosition(5, 0)));
    }

    [Test]
    public void HasLine_RockOnLine_ReturnsFalse()
    {
        Board board = CreateBoard();
        board.GetCell(new GridPosition(2, 0)).SetTerrain(TerrainObject.Rock);
        Assert.IsFalse(LineOfSight.HasLine(board, new GridPosition(0, 0), new GridPosition(5, 0)));
    }

    [Test]
    public void HasLine_RockBesideLine_ReturnsTrue()
    {
        Board board = CreateBoard();
        board.SetTerrain(new GridPosition(2, 1), TerrainObject.Rock);
        Assert.IsTrue(LineOfSight.HasLine(board, new GridPosition(0, 0), new GridPosition(5, 0)));
    }

    [Test]
    public void HasLine_AdjacentCells_AlwaysTrue()
    {
        Board board = CreateBoard();
        board.SetTerrain(new GridPosition(1, 0), TerrainObject.Rock);
        Assert.IsTrue(LineOfSight.HasLine(board, new GridPosition(0, 0), new GridPosition(1, 0)));
    }

    [Test]
    public void HasLine_RockOnTarget_DoesNotBlock()
    {
        Board board = CreateBoard();
        board.SetTerrain(new GridPosition(3, 0), TerrainObject.Rock);
        Assert.IsTrue(LineOfSight.HasLine(board, new GridPosition(0, 0), new GridPosition(3, 0)));
    }

    [Test]
    public void HasLine_IsSymmetric_ForAllPairs()
    {
        Board board = CreateBoard();
        board.SetTerrain(new GridPosition(2, 1), TerrainObject.Rock);
        board.SetTerrain(new GridPosition(4, 3), TerrainObject.Rock);

        for (int x1 = 0; x1 < 6; x1++)
        for (int y1 = 0; y1 < 6; y1++)
        for (int x2 = 0; x2 < 6; x2++)
        for (int y2 = 0; y2 < 6; y2++)
        {
            GridPosition a = new GridPosition(x1, y1);
            GridPosition b = new GridPosition(x2, y2);
            Assert.AreEqual(LineOfSight.HasLine(board, a, b), LineOfSight.HasLine(board, b, a));
        }
    }
}