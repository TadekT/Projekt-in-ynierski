using NUnit.Framework;

public class CoreStoneTests
{
    [Test]
    public void TakeDamage_ReducesHp_UntilDestroyed()
    {
        CoreStone stone = new CoreStone(new GridPosition(0, 0), Team.Player, 100);
        stone.TakeDamage(40);
        Assert.AreEqual(60, stone.CurrentHp);
        Assert.IsFalse(stone.IsDestroyed);
        stone.TakeDamage(60);
        Assert.IsTrue(stone.IsDestroyed);
    }

    [Test]
    public void TakeDamage_BeyondHp_ClampsAtZero()
    {
        CoreStone stone = new CoreStone(new GridPosition(0, 0), Team.Player, 100);
        stone.TakeDamage(999);
        Assert.AreEqual(0, stone.CurrentHp);
    }
}
