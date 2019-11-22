using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DataController : MonoBehaviour
{

    public static DataController dataContoller;

    public bool facil = false;
    public bool medio = true;
    public bool dificil = false;

    public InputField nurses;
    public InputField soldados;
    public InputField obreras;

    public int numNurse = 0;
    public int numObreras = 0;
    public int numSoldados = 0;

    public Nurse nursePrefab;
    public Obrera obreraPrefab;
    //public Soldado soldadoPrefab;
    


    private void Awake()
    {
        if (dataContoller == null)
        {
            dataContoller = this;
            DontDestroyOnLoad(this.gameObject);
        } else if (dataContoller != this)
        {
            Destroy(gameObject);
        }
       
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void f()
    {
        facil = true;
        medio = false;
        dificil = false;
    }

    public void m()
    {
        facil = false;
        medio = true;
        dificil = false;
    }

    public void d()
    {
        facil = false;
        medio = false;
        dificil = true;
    }

    public void boton()
    {
        if (nurses.text != null && nurses.text != "")
        {
            numNurse = int.Parse(nurses.text);
        }
        if (obreras.text != null && obreras.text != "")
        {
            numObreras = int.Parse(obreras.text);
        }
        if (soldados.text != null && soldados.text != "")
        {
            numSoldados = int.Parse(soldados.text);
        }

        //Debug.Log(numNurse + " " + numObreras + " " + numSoldados + " " + facil + " " + medio + " " + dificil);
        SceneManager.LoadScene(1);
    }
}
