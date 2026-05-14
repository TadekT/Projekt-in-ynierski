using System;


/* IEquatable<GridPosition> pozwala porównywać dwie pozycje GridPosition
 w bardziej kontrolowany i wydajny sposób.
 */
[Serializable]
public struct GridPosition : IEquatable<GridPosition>
{
    // współrzędna X na grid, oznaczenie kolumn pola
    public int x;

    // współrzędna Y na grid, oznaczenie wierszy pola
    public int y;


    // konstruktor struktury GridPosition, tworzy nową pozycje przypisując argumenty do zmiennych 
    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    // metoda sprawdzająca czy dwie pozycje są sobie równe poprzez porównanie x - x, y == y
    public bool Equals(GridPosition other)
    {
        return x == other.x && y == other.y;
    }


    /*
        Nadpisanie standardowej metody Equals(obj) z klasy object
        Pozwala porównać czy GridPosition gdy jest traktowany jako ogólny typ object
    */
    public override bool Equals(object obj)
    {
        return obj is GridPosition other && Equals(other);
    }


    /*
        Nadpisanie metody GetHashCode()
        przydatna jeśli GridPosition ma być użyte np w Dictionary
        Liczba 397 jest często używana jako liczba pierwsza pomagająca 
        zmniejszyć liczbe kolizji hash'y, czyli zmniejsza prawdopodobieństwo nadania różnym komórką tego samego HashCode
        x * 397 ^ y = konkretny numer HashCode
        Liczba 397 - Nie jest to liczna specjalna/konkretna, 
        ale jest liczbą pierwszą i jest wystarczająco duża aby sprawnie mieszać wartości x i y
    */
    public override int GetHashCode()
    {
        return x * 397 ^ y;
    }


    /*
        Nadpisanie standardowej metody ToString()
        Pozwala łatwo wyświetlic pozycje jako tekst
    */
    public override string ToString()
    {
        return $"({x},{y})";
    }

}
