using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class Obrera : HormigaGenerica
{

    NavMeshAgent agente;
    Floor hormigueroDentro; //Saber donde empieza el suelo para no meterte dentro del hormiguero cuando exploras
    Outside hormigueroFuera;

    //Atacar
    public int numeroDeSoldadosCerca;
    public bool hayEnemigosCerca;
    EnemigoGenerico[] enemigosCerca;

    //Hacer tuneles
    public int tiempoParaHacerTunel;
    public Vector3 posicionInicialTunel;
    public Vector3 posicionFinalTunel;

    //Curar otras hormigas
    HormigaGenerica hormigaACurar;
    public int tiempoParaCurar;

    //Recoger comida
    public int reservasDeComida;
    public Comida comida;

    //Buscar comida
    public Vector3 siguientePosicionBuscandoComida;
    public Vector3 almacenComida;

    //Ordenes de la reina
    public Reina reina;
    public bool meHanMandadoOrden;
    public enum ordenes {ORDEN1, ORDEN2};

    //Explorar
    public Vector3 siguientePosicionExplorar;

    // Start is called before the first frame update
    void Start()
    {
        agente = this.gameObject.GetComponent<NavMeshAgent>();
        hormigueroDentro = GameObject.FindObjectOfType<Floor>();
        hormigueroFuera = GameObject.FindObjectOfType<Outside>();
        this.vida = 10;
        this.daño = 2;
        siguientePosicionExplorar = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Task]
    public void Explorar()
    {
        if (this.zonaDondeEsta == 1)
        {
            //Debug.Log("Estoy fuera");
            float distanceToTarget = Vector3.Distance(transform.position, siguientePosicionExplorar);
            //Debug.Log(distanceToTarget);
            if (distanceToTarget < 0.2f)
            {
                Vector3 randomDirection;
                NavMeshHit aux;
                do
                {
                    randomDirection = UnityEngine.Random.insideUnitSphere * 10 + this.transform.position;
                } while (!NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas));
                //saliendo = true;
                agente.SetDestination(aux.position);
                siguientePosicionExplorar = aux.position;
            }
            //
        }
        else
        {
            //Debug.Log("Estoy dentro");
            if (this.zonaDondeEsta == 0)
            {
                //Debug.Log("No estoy saliendo");
                Vector3 randomDirection;
                NavMeshHit aux;
                bool aux2;
                do
                {
                    randomDirection = UnityEngine.Random.insideUnitSphere * 100 + this.transform.position;
                    aux2 = NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas);
                } while (aux.position.x > (hormigueroDentro.transform.position.x - (hormigueroDentro.width / 2)) || !aux2);
                //Debug.Log("Salir hacia: " + aux.position);
                //saliendo = true;
                agente.SetDestination(aux.position);
                siguientePosicionExplorar = aux.position;
            }
            else if (this.zonaDondeEsta == 3)
            {
                //Debug.Log("Estoy saliendo");
                if ((transform.position.x - (hormigueroFuera.transform.position.x + hormigueroFuera.width / 2) < 2f))
                {
                    //saliendo = false;
                    //estaDentro = false;
                }
            }
        }
        Task.current.Succeed();
    }
}
