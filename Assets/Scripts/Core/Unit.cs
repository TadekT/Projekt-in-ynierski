
using System;

// Klasa jednostki 
public class Unit 
{
    //okreslenie polozenia jednostki na plnaszy
    public GridPosition Position {get ; private set ;}
    public int MaxHp {get; private set;}
    
    //czy jednosta juz wykonala ruch, jedna aktywacja jednostki na ture 
    private bool HasActed = false;
    private int currentHp;

    //property pilnuje ze HP zostanie w zakresie 0..MaxHp niezaleznie od operacji
    public int CurrentHp  
        {
        get => currentHp;
        private set  => currentHp = Math.Clamp(value,0,MaxHp);
        }
    
    // Do ktorego zespolu nalezy dana jednostka player / enemy
    public Team TeamSide {get; private set;}

    public int MoveRange {get; private set;}

    public int AttackRange {get; private set;}

    public int Damage{get; private set;}

    //Konstruktor jednostki 
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
        //Math.Abs gwarantuje ze obrazenia zawsze zmniejszaja HP, nawet gdy podano ujemna wartosc
        this.CurrentHp -= Math.Abs(amount); 
    }

    public void Heal(int amount)
    {
        this.CurrentHp += Math.Abs(amount);
    }

    
    public void ResetTurnState()
    {
        
        HasActed = false;

    }

}
