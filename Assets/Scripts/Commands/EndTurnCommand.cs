

public class EndTurnCommand : ICommand
{
    private readonly Team team;

    public EndTurnCommand(Team team)
    {
        this.team = team;
    }

    public bool Validate(GameState state, out string error)
    {
        if (state.CurrentTeamTurn != team)
        {
            error = "To nie jest tura tej drużyny.";
            return false;
        }
        if (state.CurrentPhase != TurnPhase.Action)
        {
            error = "Tura nie jest w fazie akcji.";
            return false;
        }
        error = null;
        return true;
    }

    public void Execute(GameState state)
    {
        new TurnManager(state).EndTurn();
    }

    public string Describe()
    {
        return $"{team}: koniec tury";
    }

}