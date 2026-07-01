

// klasa okreslajaca przeszkode znajdujaca sie na planszy
// opisuje ilosc potrzebnych punktow akcji do przebycia przeszkody
// oraz czy przeszkoda zaslania linie strzalu
public class TerrainObject 
{

    public string Name {get; private set;}
    
    public int MovementCost{get ; private set;}

    public bool BlockLineOfSight{get; private set;}

    public TerrainObject(string name, int moveCost, bool blockLoS)
    {
        this.Name = name;
        this.MovementCost = moveCost;
        this.BlockLineOfSight = blockLoS;
        
    } 
    

}
