using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hud : MonoBehaviour
{
    [SerializeField] private GameController controller;
    [SerializeField] private BoardInput boardInput;

    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text selectedText;
    [SerializeField] private TMP_Text inspectedText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;

    private void Start()
    {
        gameOverPanel.SetActive(false);
    }

private void Update()
{
    GameState state = controller.State;

    if (state.IsGameOver())
    {
        turnText.text = "Game Over";
        selectedText.text = "";
        inspectedText.text = "";
        gameOverPanel.SetActive(true);
        gameOverText.text = state.GetWinner() == Team.Player ? "You WIN" : "Enemy WIN";
        return;
    }

    turnText.text = state.CurrentTeamTurn == Team.Player ? "Your Turn" : "Enemy Turn";

    Unit selected = boardInput.Selected;
    selectedText.text = selected == null ? "" : DescribeUnit(selected);

    inspectedText.text = DescribeTarget(boardInput.Inspected);
}

private string DescribeTarget(IAttackable target)
{
    if (target == null)
    {
        return "";
    }

    Unit unit = target as Unit;
    if (unit != null)
    {
        return DescribeUnit(unit);
    }

    CoreStone stone = target as CoreStone;
    if (stone != null)
    {
        return $"{SideName(stone.TeamSide)} Core Stone\nHP : {stone.CurrentHp}/{stone.MaxHp}";
    }

    return "";
}

private string DescribeUnit(Unit unit)
{
    string typeName = controller.UnitTypes[unit].Name;
    return $"{SideName(unit.TeamSide)} {typeName}\nHP : {unit.CurrentHp}/{unit.MaxHp}\nMoves : {unit.MoveRange}\nRange : {unit.AttackRange}\nDamage : {unit.Damage}";
}

private static string SideName(Team team)
{
    return team == Team.Player ? "Player" : "Enemy";
}

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}