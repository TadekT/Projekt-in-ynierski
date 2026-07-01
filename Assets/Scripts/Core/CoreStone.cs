
using System;

public class CoreStone
{
    

    public GridPosition Position {get; private set;}
    public Team TeamSide {get; private set;}
    public int MaxHp {get; private set;}

    private int currentHp;
    public int CurrentHp
    {
        get => currentHp;
        private set => currentHp = Math.Clamp(value,0,MaxHp);
    }


    public bool IsDestroyed => CurrentHp <= 0; 


    public CoreStone(GridPosition pos,Team side, int maxHp )
    {
        
        this.Position = pos;
        this.TeamSide = side;
        this.MaxHp = maxHp;
        this.CurrentHp = maxHp;

    }

    public void TakeDamage(int amount)
    {
        this.CurrentHp -= Math.Abs(amount);
    }


}