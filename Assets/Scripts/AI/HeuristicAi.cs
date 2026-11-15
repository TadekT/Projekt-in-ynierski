using System.Collections.Generic;

public class HeuristicAi : IAiPlayer
{
    private const int Unreachable = 999;

    private readonly Team team;
    private readonly AiWeights weights;

    public HeuristicAi(Team team) : this(team, new AiWeights()) { }

    public HeuristicAi(Team team, AiWeights weights)
    {
        this.team = team;
        this.weights = weights;
    }

    public ICommand ChooseNextCommand(GameState state)
    {
        if (state.IsGameOver() || state.CurrentTeamTurn != team)
        {
            return null;
        }

        // co mogę teraz zrobić
        List<ICommand> actions = LegalActions.GetAll(state);

        // najlepsza komenda wg wag
        ICommand endTurn = null;
        ICommand best = null;
        int bestScore = weights.EndTurnScore;

        foreach (ICommand action in actions)
        {
            if (action is EndTurnCommand)
            {
                endTurn = action;
                continue;
            }
            int score = Score(state, action);
            if (score > bestScore)
            {
                best = action;
                bestScore = score;
            }
        }

        // jeśli żaden ruch ,atak nie jest wart więcej niż koniec tury, kończymy turę
        return best ?? endTurn;
    }

    private int Score(GameState state, ICommand action)
    {
        AttackCommand attack = action as AttackCommand;
        if (attack != null)
        {
            return AttackScore(attack.Attacker, attack.Target);
        }

        MoveCommand move = action as MoveCommand;
        if (move != null)
        {
            return MoveScore(state, move.MovingUnit, move.Destination);
        }

        return 0;
    }

    // obrażenia, zabicie, Kamień Źródła, wygrana
    private int AttackScore(Unit attacker, IAttackable target)
    {
        int hp = target.CurrentHp;
        bool kills = attacker.Damage >= hp;
        int dealt = kills ? hp : attacker.Damage;

        int score = dealt * weights.DamageDealt;
        if (kills)
        {
            score += weights.KillBonus;
        }
        if (target is CoreStone)
        {
            score += weights.CoreStoneBonus;
            if (kills)
            {
                score += weights.WinBonus;
            }
        }
        return score;
    }

    // zbliżenie do celu 
    private int MoveScore(GameState state, Unit unit, GridPosition dest)
    {
        int score = 0;

        GridPosition goal;
        if (TryFindGoalCell(state, unit, out goal))
        {
            int now = RemainingCost(state, unit.Position, goal);
            int after = RemainingCost(state, dest, goal);
            if (now != Unreachable && after != Unreachable)
            {
                score += (now - after) * weights.ProgressPerCost;
            }
        }

        // czy z pola docelowego będzie co atakować
        score += BestAttackFrom(state, unit, dest) * weights.FutureAttackPercent / 100;

        // ilu wrogów dosięgnie to pole
        score -= Threat(state, unit.TeamSide, dest) * weights.DangerPercent / 100;

        return score;
    }

    // wolne pole, z którego jednostka trafi Kamień wroga
    private bool TryFindGoalCell(GameState state, Unit unit, out GridPosition goal)
    {
        goal = default(GridPosition);
        CoreStone stone = GetEnemyStone(state, unit.TeamSide);
        if (stone == null)
        {
            return false;
        }

        bool found = false;
        int bestDist = int.MaxValue;
        bool fallbackFound = false;
        GridPosition fallback = default(GridPosition);
        int bestStoneDist = int.MaxValue;

        for (int x = 0; x < state.Board.Width; x++)
        {
            for (int y = 0; y < state.Board.Height; y++)
            {
                GridPosition p = new GridPosition(x, y);
                GridCell cell = state.Board.GetCell(p);
                if (!cell.IsWalkable)
                {
                    continue;
                }
                if (p != unit.Position && state.IsOccupied(p))
                {
                    continue;
                }

                if (CanHitFrom(state, unit, p, stone.Position))
                {
                    int dist = GridPosition.Manhattan(unit.Position, p);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        goal = p;
                        found = true;
                    }
                }
                else
                {
                    int stoneDist = GridPosition.Manhattan(stone.Position, p);
                    if (stoneDist < bestStoneDist)
                    {
                        bestStoneDist = stoneDist;
                        fallback = p;
                        fallbackFound = true;
                    }
                }
            }
        }

        if (found)
        {
            return true;
        }
        if (fallbackFound)
        {
            goal = fallback;
            return true;
        }
        return false;
    }

    // koszt najtańszej ścieżki
    private static int RemainingCost(GameState state, GridPosition from, GridPosition goal)
    {
        List<GridPosition> path = Pathfinding.FindPath(state, from, goal);
        if (path == null)
        {
            return Unreachable;
        }
        int cost = 0;
        for (int i = 1; i < path.Count; i++)
        {
            cost += state.Board.GetCell(path[i]).MovementCost;
        }
        return cost;
    }

    // najlepszy atak, jaki jednostka ma z danego pola
    private int BestAttackFrom(GameState state, Unit unit, GridPosition pos)
    {
        int best = 0;

        foreach (Unit target in state.Units)
        {
            if (target.IsDestroyed || target.TeamSide == unit.TeamSide) continue;
            if (!CanHitFrom(state, unit, pos, target.Position)) continue;
            int score = AttackScore(unit, target);
            if (score > best) best = score;
        }

        foreach (CoreStone stone in state.CoreStones)
        {
            if (stone.IsDestroyed || stone.TeamSide == unit.TeamSide) continue;
            if (!CanHitFrom(state, unit, pos, stone.Position)) continue;
            int score = AttackScore(unit, stone);
            if (score > best) best = score;
        }

        return best;
    }

    private static bool CanHitFrom(GameState state, Unit unit, GridPosition pos, GridPosition targetPos)
    {
        if (GridPosition.Manhattan(pos, targetPos) > unit.AttackRange)
        {
            return false;
        }
        return unit.AttackRange == 1 || LineOfSight.HasLine(state.Board, pos, targetPos);
    }

    // suma obrażeń wrogów
    private static int Threat(GameState state, Team myTeam, GridPosition pos)
    {
        int sum = 0;
        foreach (Unit enemy in state.Units)
        {
            if (enemy.IsDestroyed || enemy.TeamSide == myTeam) continue;
            if (GridPosition.Manhattan(enemy.Position, pos) <= enemy.MoveRange + enemy.AttackRange)
            {
                sum += enemy.Damage;
            }
        }
        return sum;
    }

    private static CoreStone GetEnemyStone(GameState state, Team myTeam)
    {
        foreach (CoreStone stone in state.CoreStones)
        {
            if (!stone.IsDestroyed && stone.TeamSide != myTeam) return stone;
        }
        return null;
    }
}