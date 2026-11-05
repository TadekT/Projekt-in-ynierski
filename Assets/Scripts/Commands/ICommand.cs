

public interface ICommand 
{
    // sprawdzenie czy komenda może zostać wykonana
    bool Validate(GameState state, out string error);

    //zmienia stan gry po uprzedniej walidacji 
    void Execute(GameState state);

    //opis wykonanej komendy
    string Describe();

}
