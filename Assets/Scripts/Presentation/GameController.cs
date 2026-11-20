using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameState State { get; private set; }
    public GameSession Session { get; private set; }

    // jaki typ ma dana jednostka
    private readonly Dictionary<Unit, UnitType> unitTypes = new Dictionary<Unit, UnitType>();
    public IReadOnlyDictionary<Unit, UnitType> UnitTypes { get { return unitTypes; } }

    private void Awake()
    {
        State = CreateStartingState();
        Session = new GameSession(State);

        Debug.Log($"Gra gotowa: jednostek {State.Units.Count}, tura: {State.CurrentTeamTurn}");
    }

    private GameState CreateStartingState()
    {
        Board board = new Board(10, 10);

        // teren na środku planszy.  
        board.SetTerrain(new GridPosition(6, 5), TerrainObject.Swamp);
        board.SetTerrain(new GridPosition(3, 4), TerrainObject.Swamp);
        board.SetTerrain(new GridPosition(4, 4), TerrainObject.Rock);
        board.SetTerrain(new GridPosition(5, 5), TerrainObject.Rock);
        board.SetTerrain(new GridPosition(4, 5), TerrainObject.Swamp);
        board.SetTerrain(new GridPosition(5, 4), TerrainObject.Swamp);
        board.SetTerrain(new GridPosition(1, 5), TerrainObject.Rock);
        board.SetTerrain(new GridPosition(8, 4), TerrainObject.Rock);
        
        List<Unit> units = new List<Unit>();
        AddUnit(units, UnitType.Warrior, 1, 0, Team.Player);
        AddUnit(units, UnitType.Warrior, 0, 1, Team.Player);
        AddUnit(units, UnitType.Archer, 1, 1, Team.Player);
        AddUnit(units, UnitType.Tank, 2, 2, Team.Player);
        AddUnit(units, UnitType.Warrior, 8, 9, Team.Enemy);
        AddUnit(units, UnitType.Warrior, 9, 8, Team.Enemy);
        AddUnit(units, UnitType.Archer, 8, 8, Team.Enemy);
        AddUnit(units, UnitType.Tank, 7, 7, Team.Enemy);

        List<CoreStone> stones = new List<CoreStone>
        {
            new CoreStone(new GridPosition(0, 0), Team.Player, 50),
            new CoreStone(new GridPosition(9, 9), Team.Enemy, 50)
        };

        return new GameState(board, units, stones);
    }

    private void AddUnit(List<Unit> units, UnitType type, int x, int y, Team team)
    {
        Unit unit = type.Create(new GridPosition(x, y), team);
        units.Add(unit);
        unitTypes[unit] = type;
    }
}