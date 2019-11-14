using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;

public class EnemigoGenerico : PersonajeGenerico
{

    //Agente Navmesh
    NavMeshAgent agente;
    PandaBehaviour pb;

    protected int numeroHormigasCerca;
    public HormigaGenerica hormigaCerca;
    protected int tiempoParaIrse;

    Floor hormigueroDentro;

    // Start is called before the first frame update
    void Start()
    {
        this.zonaDondeEsta = 1;
        this.vida = 10;
        this.daño = 2;
        pb = this.gameObject.GetComponent<PandaBehaviour>();
        agente = this.gameObject.GetComponent<NavMeshAgent>();
        this.siguientePosicion = this.transform.position;
        hormigueroDentro = GameObject.FindObjectOfType<Floor>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colision enemigo con algo ");
        if (other.gameObject.tag == "Hormiga")
        {
            Debug.Log("Colision con hormiga");
        }
    }

    [Task]
    public void HayHormigasCerca()
    {
        if (hormigaCerca == null)
        {
            Task.current.Fail();
        } else
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void Atacar()
    {
        if (hormigaCerca != null)
        {
            agente.SetDestination(hormigaCerca.transform.position);
            float distanceToTarget = Vector3.Distance(transform.position, hormigaCerca.transform.position);
            if (distanceToTarget < 1.2f)
            {
                float random = Random.Range(0, 10);
                if (random < 9f)
                {
                    if (hormigaCerca.vida - this.daño <= 0)
                    {
                        hormigaCerca.quitarVida(this.daño);
                        hormigaCerca = null;
                    }
                    else
                    {
                        hormigaCerca.quitarVida(this.daño);
                    }
                } else
                {
                    Debug.Log("Ataque fallido");
                }
                
            }
        }
        Task.current.Succeed();
    }

    public bool quitarVida(int damage)
    {
        Debug.Log("Enemigo perdiendo vida");
        this.vida -= damage;
        if (vida <= 0)
        {
            Destroy(this.gameObject);
            return true;
        }
        return false;
    }

    [Task]
    public void Explorar()
    {
        //Debug.Log("Explorar");
        float distanceToTarget = Vector3.Distance(transform.position, this.siguientePosicion);
        //Debug.Log(distanceToTarget);
        if (distanceToTarget < 0.2f)
        {
            Vector3 randomDirection;
            NavMeshHit aux;
            bool aux2;
            do
            {
                randomDirection = UnityEngine.Random.insideUnitSphere * 50 + this.transform.position;
                aux2 = NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas);
            } while (aux.position.x > (hormigueroDentro.transform.position.x - (hormigueroDentro.width / 2)-1) || !aux2);

            //Debug.Log("Salir hacia: " + aux.position);
            //saliendo = true;
            agente.SetDestination(aux.position);
            siguientePosicion = aux.position;
            /*
            Vector3 randomDirection;
            NavMeshHit aux;
            do
            {
                randomDirection = UnityEngine.Random.insideUnitSphere * 50 + this.transform.position;
            } while (!NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas));


            agente.SetDestination(aux.position);
            this.siguientePosicion = aux.position;*/
        } else
        {
            agente.SetDestination(siguientePosicion);
        }
        Task.current.Succeed();
        //
    }

    
}
