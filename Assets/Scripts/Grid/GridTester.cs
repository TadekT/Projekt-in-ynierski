using UnityEngine;

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
    }


}
