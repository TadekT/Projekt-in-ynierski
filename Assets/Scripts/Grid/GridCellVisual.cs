using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellVisual : MonoBehaviour
{
    //refertencja do wyświetlanego spirte w GridCell
    [SerializeField] private SpriteRenderer spriteRenderer;

    // referencja do logicznego pola grida
    private GridCell gridCell;


    // metoda do inicjalizacji wizualnej pola po jego utworzeniu
    public void Initialize(GridCell gridCell)
    {
        
        this.gridCell = gridCell;

        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        Refresh();
    }


    // Metoda odświeżająca pole , do wywoływania po zmianie dostępności pola 
    public void Refresh()
    {
        if(gridCell == null || spriteRenderer == null)
        {
            return;
        }

        // Pole dostępne - kolor zielony
        if (gridCell.IsWalkable)
        {
            spriteRenderer.color = Color.green;
        }
        //Pole niedostępne - kolor czerwony
        else
        {
            spriteRenderer.color = Color.red;
        }

    }

}
