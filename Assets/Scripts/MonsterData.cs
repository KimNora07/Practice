using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Data", menuName = "CreateData/EnemyData")]
public class MonsterData : ScriptableObject
{
    public new string name;

    public Sprite icon;

    public int hp;
    public int maxHp;
    public int atk;
    public float distance;
    public float atkCoolTime;
    public float sleepTime;
}
