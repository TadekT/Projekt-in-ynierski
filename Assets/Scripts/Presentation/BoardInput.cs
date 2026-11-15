using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// obsługa myszy gracza: wybór jednostki, podświetlenie legalnych akcji, wysłanie komendy.
// Niczego nie zmienia w stanie bezpośrednio - tylko GameSession.Submit
public class BoardInput : MonoBehaviour
{
    [SerializeField] private GameController controller;
    [SerializeField] private BoardView boardView;

    [Header("Podświetlenia (prefaby)")]
    [SerializeField] private GameObject selectionPrefab;
    [SerializeField] private GameObject movePrefab;
    [SerializeField] private GameObject attackPrefab;

    private GameControls controls;
    private Unit selected;

    // pole -> komenda, którą wyślemy po kliknięciu tego pola
    private readonly Dictionary<GridPosition, ICommand> commandsByCell = new Dictionary<GridPosition, ICommand>();
    private readonly List<GameObject> highlights = new List<GameObject>();

    private void Awake()
    {
        controls = new GameControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Gameplay.Click.performed += OnClick;
        controls.Gameplay.EndTurn.performed += OnEndTurn;
    }

    private void OnDisable()
    {
        controls.Gameplay.Click.performed -= OnClick;
        controls.Gameplay.EndTurn.performed -= OnEndTurn;
        controls.Disable();
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        if (!CanAct()) return;
        HandleClick();
    }

    private void OnEndTurn(InputAction.CallbackContext context)
    {
        if (!CanAct()) return;
        EndTurn();
    }

    // wejście gracza działa tylko w jego turze i przed końcem gry
    private bool CanAct()
    {
        GameState state = controller.State;
        return !state.IsGameOver() && state.CurrentTeamTurn == Team.Player;
    }

    private void HandleClick()
    {
        Vector2 screen = controls.Gameplay.Point.ReadValue<Vector2>();
        Vector3 world = Camera.main.ScreenToWorldPoint(screen);
        GridPosition pos = BoardView.WorldToGrid(world);
        GameState state = controller.State;

        // 1. pole ma przypisaną komendę (ruch lub atak) -> wysyłamy ją
        ICommand command;
        if (selected != null && commandsByCell.TryGetValue(pos, out command))
        {
            string error;
            if (controller.Session.Submit(command, out error))
            {
                boardView.Refresh();
                Select(selected); // po ruchu może zostać atak, po ataku lista będzie pusta
            }
            else
            {
                Debug.Log(error);
            }
            return;
        }

        // 2. kliknięta własna jednostka -> wybór
        Unit unit = state.GetUnitAt(pos);
        if (unit != null && unit.TeamSide == Team.Player)
        {
            Select(unit);
            return;
        }

        // 3. kliknięcie gdziekolwiek indziej -> odznaczenie
        Deselect();
    }

    private void Select(Unit unit)
    {
        ClearHighlights();
        selected = unit;
        SpawnHighlight(selectionPrefab, unit.Position);

        foreach (ICommand command in LegalActions.GetForUnit(controller.State, unit))
        {
            MoveCommand move = command as MoveCommand;
            if (move != null)
            {
                commandsByCell[move.Destination] = command;
                SpawnHighlight(movePrefab, move.Destination);
                continue;
            }

            AttackCommand attack = command as AttackCommand;
            if (attack != null)
            {
                commandsByCell[attack.Target.Position] = command;
                SpawnHighlight(attackPrefab, attack.Target.Position);
            }
        }
    }

    private void Deselect()
    {
        selected = null;
        ClearHighlights();
    }

    private void EndTurn()
    {
        string error;
        if (controller.Session.Submit(new EndTurnCommand(Team.Player), out error))
        {
            Deselect();
            boardView.Refresh();
            Debug.Log("Koniec tury gracza. Teraz: " + controller.State.CurrentTeamTurn);
        }
        else
        {
            Debug.Log(error);
        }
    }

    private void SpawnHighlight(GameObject prefab, GridPosition pos)
    {
        highlights.Add(Instantiate(prefab, BoardView.GridToWorld(pos), Quaternion.identity, transform));
    }

    private void ClearHighlights()
    {
        foreach (GameObject h in highlights)
        {
            Destroy(h);
        }
        highlights.Clear();
        commandsByCell.Clear();
    }
}