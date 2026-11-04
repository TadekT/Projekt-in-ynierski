// opis typu jednostki
public class UnitType
{
    public string Name { get; private set; }
    public int MaxHp { get; private set; }
    public int MoveRange { get; private set; }
    public int AttackRange { get; private set; }
    public int Damage { get; private set; }

    private UnitType(string name, int maxHp, int moveRange, int attackRange, int damage)
    {
        Name = name;
        MaxHp = maxHp;
        MoveRange = moveRange;
        AttackRange = attackRange;
        Damage = damage;
    }

    // wartości startowe do dostrojenia przy balansie
    public static readonly UnitType Warrior = new UnitType("Wojownik", 100, 3, 1, 25);
    public static readonly UnitType Archer = new UnitType("Łucznik", 60, 3, 3, 20);
    public static readonly UnitType Tank = new UnitType("Tank", 160, 2, 1, 15);

    public Unit Create(GridPosition position, Team team)
    {
        return new Unit(position, team, MaxHp, MoveRange, AttackRange, Damage);
    }
}