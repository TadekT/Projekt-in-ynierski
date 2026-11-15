    
    // wspólny interfejs dla wszystkiego co można zaatakować
    public interface IAttackable
    {
        GridPosition Position { get; }
        Team TeamSide { get; }
        int CurrentHp { get; }
        bool IsDestroyed { get; }
        void TakeDamage(int amount);
    }