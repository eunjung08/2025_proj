using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour
{
    [SerializeField]
    public UnitInfo unitInfo;
    public Transform Leader;
    public Vector3 formOffset;
    public Vector3 DesiredVelocity;
    public float attTimer;
    public Transform currentTarget;
    public List<Unit> tempTeam = new List<Unit>(20);
    public NavMeshAgent navMeshAgent;
    GameManager gameManager;
    public GameObject[] effectPrefab;
    public float[] skillCoolDown = new float[] { 0, 0, 0, 0, 0 };
    public Debuf debuf = Debuf.none;
    public float stunTimer = 2.0f;
    public float stunTiming = 2.0f;
    PotionInfo potionInfo = new PotionInfo();

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        unitInfo = GetComponent<UnitInfo>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    private void FixedUpdate()
    {
        if (Leader == null)
        {
            return;
        }
        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            return;
        }
        if (DesiredVelocity.sqrMagnitude > 0.01f)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(DesiredVelocity);
        }
        else
        {
            navMeshAgent.isStopped = true;
        }
    }
    void Update()
    {
        if(debuf == Debuf.stun)
        {
            stunTiming -= Time.deltaTime;
            if(stunTimer > 0)
            {
                return;
            }
            else
            {
                stunTiming = stunTimer;
                debuf = Debuf.none;
            }
        }
        
        //유닛이 죽었는지 확인
        if(unitInfo.status == Status.Dead)
        {
            return;
        }
        //현재 타겟이 없다면 새로운 타겟 검색
        if (currentTarget == null || IsVaildTarget(currentTarget))
        {
            currentTarget = minDistanceTarget();
        }
        if(currentTarget != null)
        {

            //거리가 사거리에 들어왔는지 확인
            float dist = Vector3.Distance(transform.position, currentTarget.position);
            if (dist <=  unitInfo.attackRange)
            {
                unitInfo.status = Status.Attack;
            }
            else
            {
                unitInfo.status = Status.Chase;
            }
        }
        else
        {
            if(Leader != null)
            {
                unitInfo.status = Status.Follow;
            }
            else
            {
                unitInfo.status = Status.Idle;
            }
        }

        switch (unitInfo.status)
        {
            case Status.Attack:
                DesiredVelocity = Vector3.zero;
                Attack(currentTarget);
                break;
            case Status.Chase:
                Vector3 dir = currentTarget.position - transform.position;
                dir.y = 0;
                if(dir.sqrMagnitude < 0.1f)
                {
                    DesiredVelocity = Vector3.zero;
                }
                else
                {
                    DesiredVelocity = currentTarget.position;   
                    //DesiredVelocity = dir.normalized * unitInfo.moveSpeed;
                }
                break;
            case Status.Follow:
                Vector3 target = Leader.position + formOffset;
                //아군 회피
                //target += AvoidTeam();
                DesiredVelocity = target;
                break;
        }

    }

    bool IsVaildTarget(Transform tr)
    {
        //null이 들어온게 아닌지 확인
        if(tr == null)
        {
            return false;
        }
        Unit unit = tr.GetComponent<Unit>();
        //유닛인지 확인
        if(unit == null)
        {
            return false;
        }
        //체력 확인
        if(unit.unitInfo.curHP <= 0f)
        {
            return false;
        }
        //같은팀인지 확인
        if(unit.unitInfo.team == this.unitInfo.team)
        {
            return false;
        }
        return true;
    }

    Transform minDistanceTarget()
    {
        //근처 탐지
        Collider[] col = Physics.OverlapSphere(transform.position, unitInfo.detectRadius, LayerMask.GetMask("Unit"));
        //타겟 설정
        Transform choice = null;

        float bestSqr = float.MaxValue;
        for(int i = 0; i < col.Length; i++)
        {
            Unit unit = col[i].GetComponent<Unit>();
            //if (unitInfo.team == Team.Player)
            //{
            //    Debug.Log(col[i].name);
            //}
            //유닛이 아닌지, 아군인지 확인
            if (unit == null || unit.unitInfo.team == this.unitInfo.team)
            {
                continue;
            }
            //magnitude => sqrMagnitude : 제곱근 사용하지 않음, 비교 연산이라 제곱근 없는 거리
            float sq = (unit.transform.position - transform.position).sqrMagnitude;
            float distance = Vector3.Distance(unit.transform.position, transform.position);
            if(distance < bestSqr)
            {
                bestSqr = distance;
                choice = unit.transform;
            }
        }
        //if (choice != null && unitInfo.team == Team.Player)
        //{
        //    Debug.Log(choice.name);
        //}
        return choice;
    }

    void Attack(Transform target)
    {
        attTimer -= Time.deltaTime;
        //쿨타임
        if(attTimer > 0)
        {
            return;
        }
        //타겟이 없다면
        if(target == null)
        {
            return;
        }
        //타겟 유효성 체크
        if (!IsVaildTarget(target))
        {
            return;
        }
        Unit targetUnit = target.GetComponent<Unit>();
        if(unitInfo.team == Team.Player)
        {
            //GameObject effect = Instantiate(effectPrefab[(int)unitInfo.playerType], target.transform.position + new Vector3(0, 1.0f, 0), effectPrefab[(int)unitInfo.playerType].transform.rotation);


            if (targetUnit.unitInfo.curHP - unitInfo.attackDamage > 0)
            {
                unitInfo.exp += targetUnit.unitInfo.GetExp(unitInfo.attackDamage);
            }
            else
            {
                unitInfo.exp += targetUnit.unitInfo.GetExp(unitInfo.attackDamage);
                unitInfo.exp += targetUnit.unitInfo.GetBonusExp();
            }
            LvUP();
        }
        targetUnit.Damage(unitInfo.attackDamage);
        attTimer = unitInfo.attackRate;
    }

    public void LvUP()
    {
        if(unitInfo.exp > unitInfo.RequireExp())
        {
            unitInfo.exp -= unitInfo.RequireExp();
            unitInfo.curLV ++;
            unitInfo.SetInitPlayerStatus(unitInfo.playerType);

            gameManager.SetUI((int)unitInfo.playerType, unitInfo);
            if(unitInfo.curLV % 2 == 1)
            {
                Time.timeScale = 0;
                gameManager.SetSkillUI((int)unitInfo.playerType, unitInfo);
            }
        }
        gameManager.SetUI((int)unitInfo.playerType, unitInfo);
    }

    public void Damage(float dmg)
    {
        if (unitInfo.curHP <= 0f)
        {
            return; //이미 사망
        }
        unitInfo.curHP -= dmg;
        //Debug.Log(unitInfo.curHP);

        if (unitInfo.curHP <= 0f)
        {
            unitInfo.curHP = 0f;
            GameObject manager = GameObject.Find("Manager");
            LeaderManager leaderManager = manager.GetComponent<LeaderManager>();
            GameManager gameManager = manager.GetComponent<GameManager>();
            if(leaderManager.currentLeader == this)
            {
                leaderManager.units.Remove(this);
                leaderManager.currentLeaderIndex = 0;
                leaderManager.ChangePlayerLeader(0);
                //gameManager.SetEnemyLeader();
            }
            else
            {
                leaderManager.units.Remove(this);
                gameManager.enemys.Remove(this);
            }
            if(unitInfo.team == Team.Enemy)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }

        }
    }
}
