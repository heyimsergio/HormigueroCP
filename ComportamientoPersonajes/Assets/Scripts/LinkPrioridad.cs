using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LinkPrioridad : MonoBehaviour
{

    public NavMeshAgent agent;
    public NavMeshLink link;
    private int oldLinkCost;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        link = null;
    }

    // Update is called once per frame
    void Update()
    {
        //if (agent.currentOffMeshLinkData.valid)
        if (agent.isOnOffMeshLink)
        {
            AcquireOffmeshLink();
        }
        else
        {
            ReleaseOffmeshLink();
        }
    }

    void AcquireOffmeshLink()
    {
        if (link == null)
        {
            link = (NavMeshLink) agent.navMeshOwner;
            if (link != null)
            {
                oldLinkCost = link.costModifier;
                link.costModifier = 1000;
            }
            
            //          link.activated = false;
        }
    }

    void ReleaseOffmeshLink()
    {
        if (link != null)
        {
            link.costModifier = oldLinkCost;
            //          link.activated = true;
            link = null;
        }
    }
}
