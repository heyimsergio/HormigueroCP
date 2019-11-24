using Panda;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AntMovement : MonoBehaviour
{

    public Camera cam;

    public NavMeshAgent agent;

    public SpriteRenderer hormiga_sprite;
    public MeshCollider hormiga_colisiones;
    PandaBehaviour pb;

    public bool haEntradoAlCamino;

    float speed;

    public int linkMultiplayer;

    public OffMeshLink link;
    private float oldLinkCost;


    void Start()
    {
        pb = this.gameObject.GetComponent<PandaBehaviour>();
        hormiga_sprite = this.GetComponentInChildren<SpriteRenderer>();
        hormiga_colisiones = this.GetComponentInChildren<MeshCollider>();
        link = FindObjectOfType<OffMeshLink>();
        speed = agent.speed;
        haEntradoAlCamino = false;
        linkMultiplayer = 2;
    }

    // Update is called once per frame
    void Update()
    {

        if (agent.isOnOffMeshLink)
        {
            if (!haEntradoAlCamino)
            {
                PersonajeGenerico a = agent.gameObject.GetComponent<PersonajeGenerico>();
                HormigaGenerica h = this.gameObject.GetComponent<HormigaGenerica>();

                foreach (EnemigoGenerico enem in h.enemigosCerca)
                {
                    enem.hormigasCerca.Remove(h);
                }
                h.enemigosCerca = new List<EnemigoGenerico>();

                foreach (Comida c in h.comidaQueHayCerca)
                {
                    c.hormigasCerca.Remove(h);
                }
                h.comidaQueHayCerca = new List<Comida>();

                haEntradoAlCamino = true;
                if (a.zonaDondeEsta == 0)
                {
                    //a.GetComponent<NavMeshAgent>().autoTraverseOffMeshLink = false;
                    //a.GetComponent<NavMeshAgent>().updatePosition = false;
                    //a.transform.position =  link.startPoint;
                    a.zonaDondeEsta = 3;
                }
                else if (a.zonaDondeEsta == 1)
                {
                    //a.GetComponent<NavMeshAgent>().autoTraverseOffMeshLink = false;
                    //a.GetComponent<NavMeshAgent>().updatePosition = false;
                    //a.transform.position =  link.endPoint;
                    a.zonaDondeEsta = 2;
                }
                //Debug.Log(a.zonaDondeEsta);
            }
            hormiga_sprite.enabled = false;
            hormiga_colisiones.enabled = false;
            Comida aux = agent.gameObject.GetComponentInChildren<Comida>();
            if (aux != null)
            {
                aux.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
            agent.avoidancePriority = 99;
            agent.speed = speed * linkMultiplayer;
        }
        else
        {
            if (haEntradoAlCamino)
            {
                haEntradoAlCamino = false;
                PersonajeGenerico a = agent.gameObject.GetComponent<PersonajeGenerico>();
                if (a.zonaDondeEsta == 3)
                {
                    a.zonaDondeEsta = 1;
                }
                else
                {
                    a.zonaDondeEsta = 0;
                }
                //Debug.Log(a.zonaDondeEsta);
            }
            hormiga_sprite.enabled = true;
            hormiga_colisiones.enabled = true;
            Comida aux = agent.gameObject.GetComponentInChildren<Comida>();
            if (aux != null)
            {
                aux.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
            }
            agent.avoidancePriority = agent.gameObject.GetComponent<HormigaGenerica>().priority;
            agent.speed = speed;
        }
        
        
    }

    /*void AcquireOffmeshLink()
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
    }*/
}