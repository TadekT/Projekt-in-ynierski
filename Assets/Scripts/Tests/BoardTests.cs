using NUnit.Framework;

public class BoardTests
{
    [Test]
    public void GetNeighbors_InCorner_ReturnsTwoNeighbors()
    {
        var board = new Board(10, 10);
        Assert.AreEqual(2, board.GetNeighbors(new GridPosition(0, 0)).Count);
    }

    [Test]
    public void GetNeighbors_InMiddle_ReturnsFourNeighbors()
    {
        var board = new Board(10, 10);
        Assert.AreEqual(4, board.GetNeighbors(new GridPosition(3, 4)).Count);
    }

    [Test]
    public void IsInBounds_OutsideBoard_ReturnsFalse()
    {
        var board = new Board(10, 10);
        Assert.IsFalse(board.IsInBounds(new GridPosition(-1, 5)));
        Assert.IsFalse(board.IsInBounds(new GridPosition(10, 0)));
    }
}