public interface IAiPlayer
{
    ICommand ChooseNextCommand(GameState state);
}