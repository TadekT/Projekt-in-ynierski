using NUnit.Framework;

public class AttackableTests
{
    [Test]
    public void Unit_And_CoreStone_ShareAttackableInterface()
    {
        IAttackable unit = new Unit(new GridPosition(1, 1), Team.Player, 100);
        IAttackable stone = new CoreStone(new GridPosition(0, 0), Team.Enemy, 50);
        unit.TakeDamage(30);
        stone.TakeDamage(50);
        Assert.AreEqual(70, unit.CurrentHp);
        Assert.IsFalse(unit.IsDestroyed);
        Assert.IsTrue(stone.IsDestroyed);
    }
}

public class UnitTypeTests
{
    [Test]
    public void Create_AppliesTypeStats()
    {
        Unit archer = UnitType.Archer.Create(new GridPosition(2, 2), Team.Player);
        Assert.AreEqual(UnitType.Archer.MaxHp, archer.MaxHp);
        Assert.AreEqual(UnitType.Archer.AttackRange, archer.AttackRange);
        Assert.AreEqual(Team.Player, archer.TeamSide);
        Assert.AreEqual(new GridPosition(2, 2), archer.Position);
    }

    [Test]
    public void Archer_HasLongerRangeThanWarrior()
    {
        Assert.Greater(UnitType.Archer.AttackRange, UnitType.Warrior.AttackRange);
    }
}