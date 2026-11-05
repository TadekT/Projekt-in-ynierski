using System.Collections.Generic;

public class GameSession
{
    public GameState State { get; private set; }

    private readonly List<ICommand> log = new List<ICommand>();
    public IReadOnlyList<ICommand> Log => log;

    public GameSession(GameState state)
    {
        State = state;
    }

    // walidacja -> wykonanie -> wpis do logu
    public bool Submit(ICommand command, out string error)
    {
        if (command == null)
        {
            error = "Brak komendy.";
            return false;
        }
        if (State.IsGameOver())
        {
            error = "Gra została zakończona.";
            return false;
        }
        if (!command.Validate(State, out error))
        {
            return false;
        }

        command.Execute(State);
        log.Add(command);
        error = null;
        return true;
    }
}