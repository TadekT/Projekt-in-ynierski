

using System.Collections.Generic;

public class GameState
{
    //Referencja do planszy 
    public Board Board {get; private set;}
    //lista wszystkich jednostek znajdujacych sie na planszy
    public List<Unit> Units {get; private set;}
    //lista kamieni zycia gracza / przeciwnika
    public List<CoreStone> CoreStones {get; private set;}
    //aktualnie aktywna druzyna 
    public Team CurrentTeamTurn {get; private set;}
    // aktualnie aktywna faza rozgrywki
    public TurnPhase CurrentPhase {get; private set;}
    //slownik mapuje druzyne na jej aktualna ilosc many
    private Dictionary<Team,int> mana;

    //konstruktor stanow gry
    public GameState(Board board, List<Unit> units, List<CoreStone> coreStones)
    {

        this.Board = board;
        this.Units = units;
        this.CoreStones = coreStones;
        this.CurrentTeamTurn = Team.Player;
        this.CurrentPhase = TurnPhase.Shop;
        this.mana = new Dictionary<Team, int>
        {
            {Team.Player, 5 },
            {Team.Enemy, 5 }
        };
        
    }

    //metoda do zmieniania faz rozgrywki, uzywana w TurnManager
    public void AdvancePhase()
    {
        switch (CurrentPhase)
        {
            case TurnPhase.Shop:
                CurrentPhase = TurnPhase.Action;
                break;
            case TurnPhase.Action:
                CurrentPhase = TurnPhase.EndTurn;
                break;
            case TurnPhase.EndTurn:
                CurrentPhase = TurnPhase.Shop;
                break;
        }
    }


    //metoda do zmiany druzyny, uzywana w TurnManager
    public void SwitchActiveTeam()
    {
        if(CurrentTeamTurn == Team.Player)
        {
            CurrentTeamTurn = Team.Enemy;
        }
        else
        {
            CurrentTeamTurn = Team.Player;
        }

    }

    // uzyskiwanie dostepu do wartosci zmiennej mana
    public int GetMana(Team team)
    {
        return mana[team];
    }

    // dodawanie many, na starcie tury
    public void AddMana(Team team, int amount)
    {
        mana[team] += amount;
    }

    //  probuje wydac mane; zwraca false i nie odejmuje jesli nie stac
    public bool TrySpendMana(Team team, int cost)
    {
        
        if(mana[team] < cost)
        {
            return false;
        }
        mana[team] -= cost;
        return true;

    }
    //metoda logiczna sprawdzajaca czy kamien zycia zostal zniszczony
    public bool IsGameOver()
    {
        foreach(CoreStone stone in CoreStones)
        {
            if(stone.IsDestroyed) return true;
        }
        return false;
    }

    //metoda zwracajaca, kto wygral rozgrywke, sprawdza ktory kamien zostal zniszczony
    // jest to metoda Nullable, jesli zadna z druzyn jeszcze nie zniszczyla kamienia zycia, metoda zwraca null
    public Team? GetWinner()
    {
        foreach(CoreStone stone in CoreStones)
        {
            if (stone.IsDestroyed)
            {
                return stone.TeamSide == Team.Player ? Team.Enemy : Team.Player;
            }
        }
        return null;
    }


}