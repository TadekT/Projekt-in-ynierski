
// wagi funkcji oceny
public class AiWeights
{
    public int DamageDealt = 2;          // za każdy punkt zadanych obrażeń
    public int KillBonus = 50;           // zabicie celu
    public int CoreStoneBonus = 30;      // atak na Kamień Źródła
    public int WinBonus = 1000;          // zniszczenie Kamienia

    public int ProgressPerCost = 5;      // punkt za zbliżenie do celu
    public int FutureAttackPercent = 50; // jaką część wartości ataku z pola docelowego liczymy przy ruchu
    public int DangerPercent = 10;       // kara = suma obrażeń wrogów, którzy dosięgną pole

    public int EndTurnScore = 0;     
}