using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_00", menuName = "Asset/Level")]
public class LevelConfig : ScriptableObject
{
    public int Id = 0;

    public float BaseHealth = 100f;

    public float BaseCoins = 10f;

    public float MonsterSpawnRate = 1f;

    public int MaxGunTier = 1;

    public int CompletionReward = 10;

    public List<MonsterTemplate> Monsters = new();

    public int MinScore { get => CompletionReward; }

    public int AverageScore { get => Mathf.RoundToInt(MaxScore / 2); }

    public int MaxScore { get => Monsters.Sum((monster) => monster.Score) + CompletionReward; }
}
