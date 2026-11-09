public class AttackCommand : ICommand
{
    private readonly Unit attacker;
    private readonly IAttackable target;

    public AttackCommand(Unit attacker, IAttackable target)
    {
        this.attacker = attacker;
        this.target = target;
    }

    public bool Validate(GameState state, out string error)
    {
        if (attacker == null || !state.Units.Contains(attacker))
        {
            error = "Atakująca jednostka nie należy do tej gry.";
            return false;
        }
        if (target == null || !IsTargetInGame(state))
        {
            error = "Cel nie należy do tej gry.";
            return false;
        }
        if (attacker.IsDestroyed)
        {
            error = "Jednostka jest zniszczona.";
            return false;
        }
        if (attacker.TeamSide != state.CurrentTeamTurn)
        {
            error = "To nie jest tura tej jednostki.";
            return false;
        }
        if (state.CurrentPhase != TurnPhase.Action)
        {
            error = "Tura nie jest w fazie akcji.";
            return false;
        }
        if (attacker.HasActed)
        {
            error = "Jednostka już zaatakowała w tej turze.";
            return false;
        }
        if (target.IsDestroyed)
        {
            error = "Cel jest już zniszczony.";
            return false;
        }
        if (target.TeamSide == attacker.TeamSide)
        {
            error = "Nie można atakować własnej strony.";
            return false;
        }
        if (GridPosition.Manhattan(attacker.Position, target.Position) > attacker.AttackRange)
        {
            error = "Cel poza zasięgiem ataku.";
            return false;
        }
        // atak dystansowy wymaga czystej linii strzału
        if (attacker.AttackRange > 1 && !LineOfSight.HasLine(state.Board, attacker.Position, target.Position))
        {
            error = "Linia strzału jest zasłonięta.";
            return false;
        }

        error = null;
        return true;
    }

    public void Execute(GameState state)
    {
        target.TakeDamage(attacker.Damage);
        attacker.MarkActed();
    }

    public string Describe()
    {
        return $"{attacker.TeamSide}: atak {attacker.Position} -> {target.Position} ({attacker.Damage} obr.)";
    }

    // cel może być jednostką albo Kamieniem Źródła; sprawdzamy, że należy do tego stanu gry
    private bool IsTargetInGame(GameState state)
    {
        if (target is Unit unit)
        {
            return state.Units.Contains(unit);
        }
        if (target is CoreStone stone)
        {
            return state.CoreStones.Contains(stone);
        }
        return false;
    }
}