using NUnit.Framework;

public class UnitTests
{
    private Unit CreateUnit()
    {
        return new Unit(new GridPosition(0, 0), Team.Player, 100);
    }

    [Test]
    public void TakeDamage_ReducesHp_AndClampsAtZero()
    {
        Unit unit = CreateUnit();
        unit.TakeDamage(30);
        Assert.AreEqual(70, unit.CurrentHp);
        unit.TakeDamage(999);
        Assert.AreEqual(0, unit.CurrentHp);
        Assert.IsTrue(unit.IsDestroyed);
    }

    [Test]
    public void TakeDamage_NegativeValue_StillReducesHp()
    {
        Unit unit = CreateUnit();
        unit.TakeDamage(-10);
        Assert.AreEqual(90, unit.CurrentHp);
    }

    [Test]
    public void Heal_DoesNotExceedMaxHp()
    {
        Unit unit = CreateUnit();
        unit.TakeDamage(20);
        unit.Heal(999);
        Assert.AreEqual(100, unit.CurrentHp);
    }

    [Test]
    public void MoveTo_ChangesPosition()
    {
        Unit unit = CreateUnit();
        unit.MoveTo(new GridPosition(3, 3));
        Assert.AreEqual(new GridPosition(3, 3), unit.Position);
    }

    [Test]
    public void ResetTurnState_ClearsActivationFlags()
    {
        Unit unit = CreateUnit();
        unit.MarkMoved();
        unit.MarkActed();
        unit.ResetTurnState();
        Assert.IsFalse(unit.HasMoved);
        Assert.IsFalse(unit.HasActed);
    }
}