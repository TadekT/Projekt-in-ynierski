using System.Collections;
using UnityEngine;

public class AiTurnRunner : MonoBehaviour
{
    [SerializeField] private GameController controller;
    [SerializeField] private BoardView boardView;
    [SerializeField] private float delayBetweenCommands = 0.4f;

    private IAiPlayer ai;
    private bool running;

    private void Start()
    {
        ai = new HeuristicAi(Team.Enemy);
    }

    private void Update()
    {
        GameState state = controller.State;
        if (running || state.IsGameOver() || state.CurrentTeamTurn != Team.Enemy)
        {
            return;
        }
        StartCoroutine(RunAiTurn());
    }

    private IEnumerator RunAiTurn()
    {
        running = true;
        GameState state = controller.State;

        while (!state.IsGameOver() && state.CurrentTeamTurn == Team.Enemy)
        {
            yield return new WaitForSeconds(delayBetweenCommands);

            ICommand command = ai.ChooseNextCommand(state);
            string error = null;
            if (command == null || !controller.Session.Submit(command, out error))
            {
                Debug.LogError("AI nie wykonało komendy: " + error);
                enabled = false;
                yield break;
            }

            boardView.Refresh();
            Debug.Log(command.Describe());
        }

        if (state.IsGameOver())
        {
            Debug.Log("Koniec gry. Wygrywa: " + state.GetWinner());
        }
        running = false;
    }
}