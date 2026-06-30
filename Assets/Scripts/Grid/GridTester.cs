using UnityEngine;
using System.Collections.Generic;
public class GridTester : MonoBehaviour
{

    void Start()
    {

        GridPosition p = new GridPosition(3,5);
        GridCell c = new GridCell(new GridPosition(2,4));

        Board board = new Board(10,10);


        Debug.Log(p);
        Debug.Log(c);

        Debug.Log(board.GetCell(new GridPosition(3,5)));
        Debug.Log(board.IsInBounds(new GridPosition(3,5)));
        Debug.Log(board.IsInBounds(new GridPosition(-1,5)));
        Debug.Log(board.GetCell(new GridPosition(99,99)));

        List<GridCell> sasiedzi = board.GetNeighbors(new GridPosition(3, 4));
        Debug.Log($"Sąsiedzi (3,4): {sasiedzi.Count}");
        foreach (GridCell cell in sasiedzi)
        Debug.Log(cell);


    }


}
