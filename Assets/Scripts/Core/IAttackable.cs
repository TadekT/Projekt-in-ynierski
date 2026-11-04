    
    // wspólny kontrakt dla wszystkiego, co można zaatakować (jednostka, Kamień Źródła)
    public interface IAttackable
    {
        GridPosition Position { get; }
        Team TeamSide { get; }
        int CurrentHp { get; }
        bool IsDestroyed { get; }
        void TakeDamage(int amount);
    }