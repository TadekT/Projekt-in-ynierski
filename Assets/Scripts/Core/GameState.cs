

using System.Collections.Generic;

public class GameState
{
    public Board Board {get; private set;}
    public List<Unit> Units {get; private set;}

    public Team CurrentTurn {get; private set;}

    public TurnPhase CurrentPhase {get; private set;}

    public GameState(Board board, List<Unit> units)
    {

        this.Board = board;
        this.Units = units;
        this.CurrentTurn = Team.Player;
        this.CurrentPhase = TurnPhase.Shop;
        
    }



}