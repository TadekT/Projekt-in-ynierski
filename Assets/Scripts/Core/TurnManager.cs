
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
            // domknięcie tury: zmiana strony, reset jednostek (mana uśpiona, bez rozbudowy)
            gameState.SwitchActiveTeam();
            gameState.AddMana(gameState.CurrentTeamTurn, 3);
            ResetUnitsForActiveTeam();
            gameState.AdvancePhase(); // EndTurn -> Action, nowa tura zaczyna się od akcji
        }
    }

    // Metoda resetujaca "HasActed" dla jednostek rozpoczynajacych swoja ture 
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