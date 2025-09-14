using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Player, 
    Enemy, 
    Neutral
}

public enum PlayerType
{
    Mele, 
    Ranged,
    Magic,
}

public enum EnemyType
{
    mele1, 
    mele2, 
    mele3, 
    range1,
    range2, 
    range3, 
    boss1, 
    boss2, 
    boss3
}

public enum Boss1Skill
{
    summonMele1, 
    summonMele2
}

public enum Boss2Skill
{
    summonMele2, 
    summonMele3, 
    summonRange2, 
    summonRange3
}

public enum Boss3Skill
{
    summonMele3, 
    summonRange3,
    HideUI, 
    ChaosScreen
}

public enum Status
{
    Idle, 
    Follow, 
    MoveTo, 
    Chase, 
    Attack, 
    Dead
}

public enum MeleSkill
{
    none, 
    mele,
    multi,
    piercing,
    stun
}

public enum RangeSkill
{
    none,
    combo,
    multi, 
    anglePiercing,
    aoe
}

public enum MagicSkill
{
    none, 
    combo, 
    chain, 
    heal, 
    poision
}

public enum CommonSkill
{
    criticalRateUP, 
    attackSpeedUP, 
    HPUP, 
    moveSpeedUP,
    attackDamageUP
}

public enum ItemName
{
    none,
    HP1,
    HP2,
    HP3,
    MP1,
    MP2,
    MP3,
    FlashyBattle,
    MentalFocus,
    Revival
}

public enum Debuf
{
    none,
    stun,
    poison
}

[SerializeField]
public class PotionInfo
{
    public int[] Amount = new int[] { 10, 30, 50 };
    public float Cool = 2;
}
