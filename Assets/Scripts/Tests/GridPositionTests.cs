using System.Collections.Generic;
using NUnit.Framework;

public class GridPositionTests
{
    [Test]
    public void Equality_SameCoordinates_AreEqual()
    {
        Assert.IsTrue(new GridPosition(2, 3) == new GridPosition(2, 3));
        Assert.IsFalse(new GridPosition(2, 3) != new GridPosition(2, 3));
    }

    [Test]
    public void Equality_DifferentCoordinates_AreNotEqual()
    {
        Assert.IsTrue(new GridPosition(2, 3) != new GridPosition(3, 2));
    }

    [Test]
    public void HashSet_TreatsEqualPositionsAsOne()
    {
        var set = new HashSet<GridPosition> { new GridPosition(1, 1), new GridPosition(1, 1) };
        Assert.AreEqual(1, set.Count);
    }

    [Test]
    public void Manhattan_ReturnsSumOfDifferences()
    {
        Assert.AreEqual(7, GridPosition.Manhattan(new GridPosition(0, 0), new GridPosition(3, 4)));
        Assert.AreEqual(0, GridPosition.Manhattan(new GridPosition(2, 2), new GridPosition(2, 2)));
    }
}