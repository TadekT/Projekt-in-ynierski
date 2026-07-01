

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
