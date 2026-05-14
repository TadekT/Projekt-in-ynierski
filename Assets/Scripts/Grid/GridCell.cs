using Unity.VisualScripting;
using UnityEngine;

public class GridCell
{
    
    //Pozycja pola na gridzie, publicznie odczytywana, modyfikowana tylko prywatnie
    public GridPosition GridPosition {get; private set;}


    //Pozycja pola w świecie Unity Vector3 (x , y , 0)
    public Vector3 WorldPosition {get; private set;}

    //Sprawdzenie czy pole jest dostępne czy zajęte(np: ściany, jednostki)
    public bool IsWalkable {get; private set;} = true;
    
    
    
    //Kostruktor  klasy GridCell, do tworzenia nowych pól
    public GridCell(GridPosition gridPosition, Vector3 worldPosition)
    {
        // przypisanie pozycji logicznej pola 
        GridPosition = gridPosition;

        // przypisanie pozycji pola w świecie Unity 
        WorldPosition = worldPosition; 

    }


    // metoda do zmiany informacji czy pole jest dostępne do ruchu
    public void SetWalkable(bool isWalkable)
    {
        IsWalkable = isWalkable;
    }


}
