using System;

// Klasa jednostki
public class Unit : IAttackable
{
    // położenie jednostki na planszy
    public GridPosition Position { get; private set; }
    public int MaxHp { get; private set; }

    // reguła aktywacji: ruch maksymalnie raz, atak maksymalnie raz, atak kończy aktywację
    public bool HasMoved { get; private set; }
    public bool HasActed { get; private set; }

    private int currentHp;

    // property pilnuje, że HP zostaje w zakresie 0..MaxHp niezależnie od operacji
    public int CurrentHp
    {
        get => currentHp;
        private set => currentHp = Math.Clamp(value, 0, MaxHp);
    }

    public bool IsDestroyed => CurrentHp <= 0;

    // do którego zespołu należy jednostka (Player / Enemy)
    public Team TeamSide { get; private set; }

    public int MoveRange { get; private set; }
    public int AttackRange { get; private set; }
    public int Damage { get; private set; }

    // konstruktor z domyślnymi statystykami (zachowuje dotychczasowe wywołania)
    public Unit(GridPosition pos, Team side, int maxHp)
        : this(pos, side, maxHp, 2, 2, 15)
    {
    }

    // konstruktor z pełnym zestawem statystyk (pod typy jednostek)
    public Unit(GridPosition pos, Team side, int maxHp, int moveRange, int attackRange, int damage)
    {
        this.Position = pos;
        this.TeamSide = side;
        this.MaxHp = maxHp;
        this.CurrentHp = maxHp;
        this.MoveRange = moveRange;
        this.AttackRange = attackRange;
        this.Damage = damage;
    }

    public void TakeDamage(int amount)
    {
        // Math.Abs gwarantuje, że obrażenia zawsze zmniejszają HP, nawet przy ujemnej wartości
        this.CurrentHp -= Math.Abs(amount);
    }

    public void Heal(int amount)
    {
        this.CurrentHp += Math.Abs(amount);
    }

    // zmiana pozycji; wołana wyłącznie z komend (walidacja jest po stronie komendy)
    public void MoveTo(GridPosition newPosition)
    {
        this.Position = newPosition;
    }

    public void MarkMoved()
    {
        HasMoved = true;
    }

    public void MarkActed()
    {
        HasActed = true;
    }

    // reset na początku tury strony
    public void ResetTurnState()
    {
        HasMoved = false;
        HasActed = false;
    }
}