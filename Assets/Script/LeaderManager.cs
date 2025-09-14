using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LeaderManager : MonoBehaviour
{
    public List<Unit> units = new List<Unit>();
    public int currentLeaderIndex = 0;
    public Unit currentLeader;
    GameManager gameManager;
    //Cam cam;
    
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        //cam = GameObject.Find("Main Camera").GetComponent<Cam>();
        //SetPlayerLeader();

    }

    public void SetPlayerLeader()
    {
        if(units.Count > 0)
        {
            ChangePlayerLeader(currentLeaderIndex);
        }
        else
        {
            Unit[] temp = GameObject.FindObjectsOfType<Unit>();
            foreach(Unit unit in temp)
            {
                units.Add(unit);
            }
            units.Sort((x,y) => x.unitInfo.playerType.CompareTo(y.unitInfo.playerType));
            ChangePlayerLeader(currentLeaderIndex);
        }
    }

    public void ChangePlayerLeader(int index)
    {
        if(units.Count == 0)
        {
            return;
        }
        //기존 리더에서 LeaderController 제거
        if(currentLeader != null)
        {
            LeaderController oldLCtrl = currentLeader.GetComponent<LeaderController>();
            if (oldLCtrl != null)
            {
                Destroy(oldLCtrl);
            }
            CharacterController oldCCtrl = currentLeader.GetComponent<CharacterController>();
            if (oldCCtrl != null)
            {
                Destroy(oldCCtrl);
            }
            currentLeader.Leader = units[currentLeaderIndex].transform;
        }
        //새로운 리더 지정
        currentLeaderIndex = index;
        currentLeader = units[currentLeaderIndex];
        if(gameManager == null)
        {
            gameManager = GetComponent<GameManager>();
        }
        gameManager.SetUILeader(currentLeaderIndex, units[currentLeaderIndex].unitInfo);
        //LeaderController 부착
        if (currentLeader.GetComponent<LeaderController>() == null)
        {
            currentLeader.gameObject.AddComponent<LeaderController>();
        }
        if (currentLeader.GetComponent<CharacterController>() == null)
        {
            currentLeader.gameObject.AddComponent<CharacterController>();
            currentLeader.GetComponent<CharacterController>().center = new Vector3(0, 1.0f, 0);
        }
        //cam.Player = currentLeader.transform;
        for(int i = 0; i < units.Count; i++)
        {
            if(i != currentLeaderIndex)
            {
                units[i].Leader = currentLeader.transform;
                units[i].formOffset = units[i].unitInfo.formOffset[i];
                units[i].navMeshAgent.isStopped = false;
                //units[i].navMeshAgent.enabled = true;
            }
            else
            {
                units[i].Leader = null;
                units[i].formOffset = Vector3.zero;
                units[i].navMeshAgent.isStopped = true;
            }
        }
        Debug.Log("리더 변경 : " + currentLeader.gameObject.name);
    }
}
