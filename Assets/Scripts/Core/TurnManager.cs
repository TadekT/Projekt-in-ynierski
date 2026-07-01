
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
        if(gameState.CurrentPhase == TurnPhase.EndTurn)
        {
            // domkniecie tury: zmiana strony, przychod many, reset jednostek
            gameState.SwitchActiveTeam();
            gameState.AddMana(gameState.CurrentTeamTurn, 3);
            ResetUnitsForActiveTeam();
            gameState.AdvancePhase(); // EndTurn -> Shop, nowa tura zaczyna sie od zakupow

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