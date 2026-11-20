
public class TurnManager
{
    
    //referencja do klasy GameState 
    private GameState gameState;

    //konstruktor
    public TurnManager(GameState state)
    {
        
        this.gameState = state;

    }

    // Metoda obejmujaca logike zakonczenia tury
    public void EndTurn()
    {
        gameState.AdvancePhase(); // Action -> EndTurn
        if (gameState.CurrentPhase == TurnPhase.EndTurn)
        {
            // zmiana strony, reset jednostek
            gameState.SwitchActiveTeam();
            ResetUnitsForActiveTeam();
            gameState.AdvancePhase(); // EndTurn -> Action, nowa tura zaczyna się od akcji
        }
    }

    private void ResetUnitsForActiveTeam()
    {
        
        foreach( Unit unit in gameState.Units)
        {
            if (unit.TeamSide == gameState.CurrentTeamTurn)
            {
                unit.ResetTurnState();
            }
        }

    }


}