using UnityEngine;
using System.Collections.Generic;

public class GameStateTester : MonoBehaviour
{
    void Start()
    {
        Board board = new Board(10, 10);

        List<Unit> units = new List<Unit>
        {
            new Unit(new GridPosition(0, 0), Team.Player, 100),
            new Unit(new GridPosition(4, 4), Team.Enemy, 100)
        };

        List<CoreStone> coreStones = new List<CoreStone>
        {
            new CoreStone(new GridPosition(0, 0), Team.Player, 50),
            new CoreStone(new GridPosition(4, 4), Team.Enemy, 50)
        };

        GameState gameState = new GameState(board, units, coreStones);
        TurnManager turnManager = new TurnManager(gameState);

        Debug.Log($"Start: faza={gameState.CurrentPhase}, tura={gameState.CurrentTeamTurn}, mana Player={gameState.GetMana(Team.Player)}");

        turnManager.EndTurn();
        Debug.Log($"Po EndTurn: faza={gameState.CurrentPhase}, tura={gameState.CurrentTeamTurn}, mana Enemy={gameState.GetMana(Team.Enemy)}");

        // test many
        bool spent = gameState.TrySpendMana(Team.Player, 999);
        Debug.Log($"Próba wydania 999 many (za dużo): udało się? {spent}");

        // test warunku zwycięstwa
        coreStones[1].TakeDamage(999);
        Debug.Log($"Po zniszczeniu kamienia Enemy: IsGameOver={gameState.IsGameOver()}, zwycięzca={gameState.GetWinner()}");
    }
}