using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LinkPrioridad : MonoBehaviour
{

    public NavMeshAgent agent;
    public OffMeshLink link;
    private float oldLinkCost;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        link = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.currentOffMeshLinkData.valid)
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
            link = agent.currentOffMeshLinkData.offMeshLink;
            oldLinkCost = link.costOverride;
            link.costOverride = 1000.0f;
            //          link.activated = false;
        }
    }

    void ReleaseOffmeshLink()
    {
        if (link != null)
        {
            link.costOverride = oldLinkCost;
            //          link.activated = true;
            link = null;
        }
    }
}
