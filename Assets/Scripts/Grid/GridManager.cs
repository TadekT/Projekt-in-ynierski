using UnityEngine;

public class GridManager : MonoBehaviour
{

    [Header("Grid Settings")]
    
    //Szerokość planszy (wiersze), czyli liczba pól osi x
    [SerializeField] private int width = 10;

    //Wysokość planszy (kolumny), czyli liczba pól osi y
    [SerializeField] private int height = 10;

    //rozmiar jednego pola  1f = 1x1 
    [SerializeField] private float cellSize = 1f;

    [SerializeField] private Vector3 originPosition = Vector3.zero;



    [Header("Visuals")]

    // prefab do wizualizacji pola w świecie unity 
    [SerializeField] private GameObject cellVisualPrefab;

    // opcjonalny rodzic dla wizualnych pól.
    // pomaga utrzymać porządek w hierarchii sceny.
    [SerializeField] private Transform cellVisualParetn;


    //Tablica dwuwymiarow przechowująca logiczne pola planszy
    //
    private GridCell[,] gridCells;

    void Awake()
    {
        GenerateGrid();
    }


    private void GenerateGrid()
    {
        
        //Utworzenie tablicy dwuwymiarowej o wymiarach width x hegigh
        gridCells = new GridCell[width,height];

        //pierwsza pętla przechodząca po osi x
        for(int x = 0; x < width; x++)
        {
            //druga pentla przechodząca po osi y
            for(int y = 0; y < height; y++)
            {
                
                //tworzenie pozycja logicznej pola 
                GridPosition gridPosition = new GridPosition(x,y);

                
                Vector3 worldPosition = GridToWorldPosition(gridPosition);


                GridCell gridCell = new GridCell(gridPosition, worldPosition);

                gridCells[x,y] = gridCell;

                CreateCellVisual(gridCell);



            }

        }


    }

    //metoda tworząca wizalą reprentacje pola na grid (planszy)
    private void CreateCellVisual(GridCell gridCell)
    {
        
        // 
        if(gridCell == null)
        {
            return;
        }


        
        // Jeżeli w Inspectorze przypisano obiekt rodzica,
        // wizualne pola zostaną do niego przypięte.
        // Jeśli nie przypisano rodzica, pola zostaną przypięte
        // do obiektu, na którym znajduje się GridManager.
        Transform parent = cellVisualParetn != null ? cellVisualParetn : transform;


        // Utworzenie nowego obiektu w scenie na podstawie prefabu
        GameObject cellVisualObject  =  Instantiate(cellVisualPrefab, gridCell.WorldPosition, Quaternion.identity,parent);

        //Nadanie obiektowi nazwy w hierarchi
        cellVisualObject.name = $"Cell {gridCell.GridPosition.x}, {gridCell.GridPosition.y}";

        //Ustawienie skali wizualnego pola na grid (planszy)
        cellVisualObject.transform.localScale = Vector3.one * cellSize * 0.95f;


        // Pobieramy komponent GridCellVisual z utworzonego obiektu.
        GridCellVisual cellVisual = cellVisualObject.GetComponent<GridCellVisual>();
        
        // Jeśli prefab rzeczywiście posiada komponent GridCellVisual,
        // przekazujemy mu logiczne dane pola.       
        if (cellVisual != null)
        {
            cellVisual.Initialize(gridCell);
        }

    }


    
    
    //metoda przelicze pozycje logiczną grid(planszy) na pozycje w świecie Unity 
    //( przykład : GridPosition (0, 0) może dać WorldPosition (0.5, 0.5, 0) )
    public Vector3 GridToWorldPosition(GridPosition gridPosition)
    {  
        return originPosition + new Vector3( 
            gridPosition.x * cellSize + cellSize * 0.5f,
            gridPosition.y * cellSize + cellSize * 0.5f,
            0f
        );
    }


    // Metoda przelicza pozycję świata Unity na pozycję logiczną grid (planszy)
    public GridPosition WorldToGridPosition(Vector3 worldPosition)
    {
        // Odejmujemy originPosition, żeby uwzględnić przesunięcie całej planszy.
        // Następnie dzielimy przez cellSize, żeby uzyskać indeks pola.
        int x = Mathf.FloorToInt((worldPosition.x - originPosition.x) / cellSize);
        int y = Mathf.FloorToInt((worldPosition.y - originPosition.y) / cellSize);

        // Zwracamy pozycję pola na gridzie.
        return new GridPosition(x, y);
    }

    //Metoda sprawdza czy dana pozycja mieści się w granicach grid (planszy)
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        
        return gridPosition.x >= 0 &&
               gridPosition.y >= 0 &&
               gridPosition.x < width &&
               gridPosition.y < height;

    } 



    public GridCell GetGridCell(GridPosition gridPosition)
    {
        //sprawdzamy czy pozycja istnieje na grid(planszy)
        if (!IsValidGridPosition(gridPosition))
        {
            return null;
        }

        //Jeśli pozycja jest poprawna zwracamy pole z tablicy
        return gridCells[gridPosition.x, gridPosition.y];

    }



}
