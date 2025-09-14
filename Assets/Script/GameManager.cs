using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject prefabUnit;
    public LeaderManager leaderManager;
    public List<Unit> enemys = new List<Unit>();
    public int enemyLv;
    public Transform[] Board;

    public Text[] jobText;
    public Text[] lvText;
    public Text[] expText;
    public Text[] hpText;
    public Text[] mpText;
    public Text[] atkText;

    public Text[] meleSkillText;
    public Text[] rangeSkillText;
    public Text[] magicSkillText;

    public Text[] leaderText;
    public Text[] invText;

    enum eBoard
    {
        Leader,
        Job,
        Lv,
        Exp,
        HP,
        MP,
        Atk,
        Skill,
        Inv
    }

    public GameObject BackLvBox;
    public UnitInfo lvUnitInfo;

    public void Start()
    {
        SetPlayerTeam();
        leaderManager = GetComponent<LeaderManager>();
        leaderManager.SetPlayerLeader();
        enemyLv = 0;
        Invoke("SetEnemyTeam", 5.0f);
    }
    public void SetPlayerTeam()
    {
        for (int i = 0; i < 3; i++)
        {
            Unit unit = Instantiate(prefabUnit).GetComponent<Unit>();
            unit.unitInfo.team = Team.Player;
            unit.unitInfo.playerType = (PlayerType)i;
            unit.unitInfo.SetInitPlayerStatus(unit.unitInfo.playerType);
            unit.gameObject.name = ((PlayerType)i).ToString();
            SetUI(i, unit.unitInfo);
        }
    }

    public void SetUI(int num, UnitInfo unitInfo)
    {
        jobText[num].text = unitInfo.playerType.ToString();
        lvText[num].text = unitInfo.curLV + "/" + unitInfo.maxLV;
        expText[num].text = unitInfo.exp + "/" + unitInfo.RequireExp();
        hpText[num].text = unitInfo.curHP + "/" + unitInfo.maxHP;
        mpText[num].text = unitInfo.curMP + "/" + unitInfo.maxMP;
        atkText[num].text = unitInfo.attackDamage.ToString();
    }

    public void SetSkillUI(int num, UnitInfo unitInfo)
    {
        lvUnitInfo = unitInfo;
        BackLvBox.SetActive(true);
    }

    public void BtnSkill(int num)
    {
        switch (num)
        {
            //직업별 스클 시작
            case 0:
            case 1:
            case 2:
            case 3:
                switch (lvUnitInfo.playerType)
                {
                    case PlayerType.Mele:
                        {
                            int tempNum = -1;
                            //스킬 배운적이 있는지 확인
                            for (int i = 0; i < lvUnitInfo.meleSkill.Length; i++)
                            {
                                if (lvUnitInfo.meleSkill[i] == (MeleSkill)(num + 1))
                                {
                                    tempNum = i;
                                    break;
                                }
                            }
                            //Debug.Log(tempNum);
                            //아직 스킬을 배우적 없음
                            if (tempNum == -1)
                            {
                                //비어있는 스킬칸 확인{
                                for (int i = 0; i < lvUnitInfo.skillLev.Length; i++)
                                {
                                    if (lvUnitInfo.skillLev[i] == 0)
                                    {
                                        tempNum = i;
                                        break;
                                    }
                                }
                                lvUnitInfo.meleSkill[tempNum] = (MeleSkill)(num + 1);
                                Time.timeScale = 1;
                                BackLvBox.SetActive(false);
                                lvUnitInfo.skillLev[tempNum]++;
                                meleSkillText[tempNum].text = ((MeleSkill)(num + 1)).ToString() + "Lv." + lvUnitInfo.skillLev[tempNum];
                                return;
                            }
                            //스킬레벨이 만렙임
                            if (lvUnitInfo.skillLev[tempNum] >= 5)
                            {
                                //다른 거 클릭
                                return;
                            }
                            lvUnitInfo.skillLev[tempNum]++;
                            meleSkillText[tempNum].text = ((MeleSkill)(num + 1)).ToString() + "Lv." + lvUnitInfo.skillLev[tempNum];
                        }
                        break;
                    case PlayerType.Ranged:
                        {
                            int tempNum = -1;
                            //스킬 배운적이 있는지 확인
                            for (int i = 0; i < lvUnitInfo.rangeSkill.Length; i++)
                            {
                                if (lvUnitInfo.rangeSkill[i] == (RangeSkill)(num + 1))
                                {
                                    tempNum = i;
                                    break;
                                }
                            }
                            
                            //아직 스킬을 배우적 없음
                            if (tempNum == -1)
                            {
                                //비어있는 스킬칸 확인{
                                for (int i = 0; i < lvUnitInfo.skillLev.Length; i++)
                                {
                                    if (lvUnitInfo.skillLev[i] == 0)
                                    {
                                        tempNum = i;
                                        break;
                                    }
                                }
                                lvUnitInfo.rangeSkill[tempNum] = (RangeSkill)(num + 1);
                                Time.timeScale = 1;
                                BackLvBox.SetActive(false);
                                lvUnitInfo.skillLev[tempNum]++;
                                rangeSkillText[tempNum].text = ((RangeSkill)(num + 1)).ToString() + "Lv." + lvUnitInfo.skillLev[tempNum];
                                return;
                            }
                            //스킬레벨이 만렙임
                            if (lvUnitInfo.skillLev[tempNum] >= 5)
                            {
                                //다른 거 클릭
                                return;
                            }
                            lvUnitInfo.skillLev[tempNum]++;
                            rangeSkillText[tempNum].text = ((RangeSkill)(num + 1)).ToString() + "Lv." + lvUnitInfo.skillLev[tempNum];
                        }
                        break;
                    case PlayerType.Magic:
                        {
                            int tempNum = -1;
                            //스킬 배운적이 있는지 확인
                            for (int i = 0; i < lvUnitInfo.magicSkill.Length; i++)
                            {
                                if (lvUnitInfo.magicSkill[i] == (MagicSkill)(num + 1))
                                {
                                    tempNum = i;
                                    magicSkillText[tempNum].text = ((MagicSkill)(num + 1)).ToString() + "Lv." + lvUnitInfo.skillLev[tempNum];
                                    break;
                                }
                            }
                            //아직 스킬을 배우적 없음
                            if (tempNum == -1)
                            {
                                //비어있는 스킬칸 확인{
                                for (int i = 0; i < lvUnitInfo.skillLev.Length; i++)
                                {
                                    if (lvUnitInfo.skillLev[i] == 0)
                                    {
                                        tempNum = i;
                                        break;
                                    }
                                }
                                lvUnitInfo.magicSkill[tempNum] = (MagicSkill)(num + 1);
                                Time.timeScale = 1;
                                BackLvBox.SetActive(false);
                                lvUnitInfo.skillLev[tempNum]++;
                                magicSkillText[tempNum].text = ((MagicSkill)(num + 1)).ToString() + "Lv." + lvUnitInfo.skillLev[tempNum];
                                return;
                            }
                            //스킬레벨이 만렙임
                            if (lvUnitInfo.skillLev[tempNum] >= 5)
                            {
                                //다른 거 클릭
                                return;
                            }
                            lvUnitInfo.skillLev[tempNum]++;
                            magicSkillText[tempNum].text = ((MagicSkill)(num + 1)).ToString() + "Lv." + lvUnitInfo.skillLev[tempNum];
                        }
                        break;
                }
                break;
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
                {
                    foreach(Unit u in leaderManager.units)
                    {
                        u.unitInfo.commonSkilLev[(num - 4)]++;
                    }
                }
                break;
        }
        Time.timeScale = 1;
        BackLvBox.SetActive (false);
    }

    public void SetUILeader(int num, UnitInfo unitInfo)
    {
        for(int i = 0; i < leaderText.Length; i++)
        {
            if(i == num)
            {
                leaderText[i].text = "Leader";
            }
            else
            {
                leaderText[i].text = "";
            }
        }
    }

    public void SetEnemyTeam()
    {
        switch (enemyLv)
        {
            case 0:
                //int num = 10;
                int num = UnityEngine.Random.Range(2, 6);
                for(int i = 0; i < num; i++)
                {
                    Unit unit = Instantiate(prefabUnit, new Vector3(5.0f, 0, 0), prefabUnit.transform.rotation).GetComponent<Unit>();
                    unit.unitInfo.team = Team.Enemy;
                    unit.unitInfo.enemyType = EnemyType.mele1;
                    unit.currentTarget = leaderManager.currentLeader.transform;
                    unit.Leader = leaderManager.currentLeader.transform;
                    unit.gameObject.name = EnemyType.mele1.ToString();
                    unit.unitInfo.SetInitEnemyStatus(unit.unitInfo.enemyType);
                    enemys.Add(unit);
                }
                break;
        }
    }



}
