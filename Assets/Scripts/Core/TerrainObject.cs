// klasa opisująca rodzaj terenu na polu planszy:
// koszt wejścia, blokowanie linii strzału i możliwość przejścia
public class TerrainObject
{
    public string Name { get; private set; }
    public int MovementCost { get; private set; }
    public bool BlockLineOfSight { get; private set; }
    public bool IsPassable { get; private set; }

    //rodzaje terenu
    public static readonly TerrainObject Plain = new TerrainObject("Plain", 1, false, true);
    public static readonly TerrainObject Swamp = new TerrainObject("Swamp", 2, false, true);
    public static readonly TerrainObject Rock = new TerrainObject("Rock", 1, true, false);

    public TerrainObject(string name, int moveCost, bool blockLoS, bool isPassable)
    {
        this.Name = name;
        this.MovementCost = moveCost;
        this.BlockLineOfSight = blockLoS;
        this.IsPassable = isPassable;
    }

    // stary konstruktor żeby nie rozwalić testów
    public TerrainObject(string name, int moveCost, bool blockLoS)
        : this(name, moveCost, blockLoS, true)
    {
    }
}