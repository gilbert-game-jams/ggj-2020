using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackManager : MonoBehaviour
{

    [SerializeField]
    Transform[] PatrolPoints;
    [SerializeField]
    Transform CrackParent;
    List<CrackBehaviour> DisabledCrackList = new List<CrackBehaviour>(); 
    List<CrackBehaviour> EnabledCrackList = new List<CrackBehaviour>();

    [SerializeField]
    float CrackMaxDelay =1;
    [SerializeField]
    float CrackMinDelay=5;
    float CrackTimer;

    // Start is called before the first frame update
    void Start()
    {
        //add alla the cracks in the list 
        for(int i=0; i< CrackParent.childCount; i++)
        {
         DisabledCrackList.Add(CrackParent.GetChild(i).GetComponent<CrackBehaviour>());
         DisabledCrackList[i].gameObject.SetActive(false);   
        }
        UpdateTimer();
    }

    // Update is called once per frame
    private void FixedUpdate() 
    {
        for(int i = 0;  i < EnabledCrackList.Count; i++)
        {
            if(!EnabledCrackList[i].gameObject.activeSelf)
            {
                CrackBehaviour crackSwap = EnabledCrackList[i];
                DisabledCrackList.Add(crackSwap);
                EnabledCrackList.Remove(crackSwap);
            }
        }
        if(CrackTimer > Time.time)
        return;
        if(DisabledCrackList.Count == 0)
        {
            UpdateTimer();
            return;
        }
        int randomIndex = Random.Range(0, DisabledCrackList.Count);
        CrackBehaviour crackTemp = DisabledCrackList[randomIndex];
        crackTemp.gameObject.SetActive(true);
       EnabledCrackList.Add(crackTemp);
       DisabledCrackList.Remove(crackTemp);
       UpdateTimer();
    }

    public bool TryGetCrack(out CrackBehaviour _crack)
    {
        _crack = null;
        int randomIndex = Random.Range(0,EnabledCrackList.Count);
        for(int i = 0; i < EnabledCrackList.Count; i++)
        {
            if(EnabledCrackList[randomIndex].CanTake())
            {
                _crack = EnabledCrackList[randomIndex];
                return true;
            }
            randomIndex++;
            randomIndex = randomIndex%EnabledCrackList.Count;
        }
        return false;
    }

    public Vector3 GetPatrolPoint()
    {
        int randomIndex = Random.Range(0,PatrolPoints.Length);
        return PatrolPoints[randomIndex].position;
    }
    void UpdateTimer()
    {
        CrackTimer = Time.time+ Random.Range(CrackMinDelay,CrackMaxDelay);
    }

}
