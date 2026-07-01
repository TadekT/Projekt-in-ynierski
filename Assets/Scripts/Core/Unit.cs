
using System;

public class Unit 
{
    public GridPosition Position {get ; private set ;}
    public int MaxHp {get; private set;}
    
    private int currentHp;
    public int CurrentHp  
        {
        get => currentHp;
        private set  => currentHp = Math.Clamp(value,0,MaxHp);
        }
    
    public Team TeamSide {get; private set;}

    public int MoveRange {get; private set;}

    public int AttackRange {get; private set;}

    public int Damage{get; private set;}

    public Unit(GridPosition pos, Team side ,int maxHp)
    {
        this.Position = pos;
        this.TeamSide = side;
        this.MaxHp = maxHp;
        this.CurrentHp = maxHp;
        this.MoveRange = 2;
        this.AttackRange = 2;
        this.Damage = 15;

    }

    public void TakeDamage(int amount)
    {
        this.CurrentHp -= Math.Abs(amount);
    }

    public void Heal(int amount)
    {
        this.CurrentHp += Math.Abs(amount);
    }

}
