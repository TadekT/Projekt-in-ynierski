using System.Collections.Generic;
using UnityEngine;


// rysuje planszę, jednostki i Kamienie
public class BoardView : MonoBehaviour
{
    [Header("GameController ref")]
    [SerializeField] private GameController controller;

    [Header("Grid tile prefabs")]
    [SerializeField] private GameObject plainTilePrefab;
    [SerializeField] private GameObject swampTilePrefab;
    [SerializeField] private GameObject rockTilePrefab;

    [Header("Player units")]
    [SerializeField] private GameObject playerWarriorPrefab;
    [SerializeField] private GameObject playerArcherPrefab;
    [SerializeField] private GameObject playerTankPrefab;

    [Header("Enemy units")]
    [SerializeField] private GameObject enemyWarriorPrefab;
    [SerializeField] private GameObject enemyArcherPrefab;
    [SerializeField] private GameObject enemyTankPrefab;

    [Header("Core Stone")]
    [SerializeField] private GameObject playerStonePrefab;
    [SerializeField] private GameObject enemyStonePrefab;

    private readonly Dictionary<Unit, GameObject> unitViews = new Dictionary<Unit, GameObject>();
    private readonly Dictionary<CoreStone, GameObject> stoneViews = new Dictionary<CoreStone, GameObject>();


    public static Vector3 GridToWorld(GridPosition pos)
    {
        return new Vector3(pos.x, pos.y, 0f);
    }

    // przyda się przy klikaniu myszą
    public static GridPosition WorldToGrid(Vector3 world)
    {
        return new GridPosition(Mathf.RoundToInt(world.x), Mathf.RoundToInt(world.y));
    }

    private void Start()
    {
        BuildTiles();
        BuildPieces();
        Refresh();
    }

    private void BuildTiles()
    {
        Board board = controller.State.Board;
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                GridPosition pos = new GridPosition(x, y);
                TerrainObject terrain = board.GetCell(pos).Terrain;

                GameObject tile = Instantiate(TilePrefabFor(terrain), GridToWorld(pos), Quaternion.identity, transform);
                tile.name = "Tile " + x + "," + y;
            }
        }
    }

    private void BuildPieces()
    {
        foreach (CoreStone stone in controller.State.CoreStones)
        {
            GameObject prefab = stone.TeamSide == Team.Player ? playerStonePrefab : enemyStonePrefab;
            GameObject go = Instantiate(prefab, GridToWorld(stone.Position), Quaternion.identity, transform);
            go.name = "Stone " + stone.TeamSide;
            stoneViews[stone] = go;
        }

        foreach (Unit unit in controller.State.Units)
        {
            GameObject prefab = UnitPrefabFor(unit);
            GameObject go = Instantiate(prefab, GridToWorld(unit.Position), Quaternion.identity, transform);
            go.name = "Unit " + unit.TeamSide;
            unitViews[unit] = go;
        }
    }

    // synchronizuje widok ze stanem
    public void Refresh()
    {
        foreach (KeyValuePair<Unit, GameObject> pair in unitViews)
        {
            pair.Value.SetActive(!pair.Key.IsDestroyed);
            pair.Value.transform.position = GridToWorld(pair.Key.Position);
        }
        foreach (KeyValuePair<CoreStone, GameObject> pair in stoneViews)
        {
            pair.Value.SetActive(!pair.Key.IsDestroyed);
        }
    }

    private GameObject TilePrefabFor(TerrainObject terrain)
    {
        if (terrain == TerrainObject.Rock) return rockTilePrefab;
        if (terrain == TerrainObject.Swamp) return swampTilePrefab;
        return plainTilePrefab;
    }

    private GameObject UnitPrefabFor(Unit unit)
    {
        UnitType type = controller.UnitTypes[unit];
        bool isPlayer = unit.TeamSide == Team.Player;

        if (type == UnitType.Archer) return isPlayer ? playerArcherPrefab : enemyArcherPrefab;
        if (type == UnitType.Tank) return isPlayer ? playerTankPrefab : enemyTankPrefab;
        return isPlayer ? playerWarriorPrefab : enemyWarriorPrefab;
    }
}