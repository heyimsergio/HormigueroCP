using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Hormiga : MonoBehaviour
{

    int i;
   // Start is called before the first frame update
   void Start()
    {
       i = 0;
    }
    [Task]
    public void MoveTo(float x, float z)
    {
        var task = Task.current;
        Vector3 destination = new Vector3(x, 0, z);
        Vector3 delta;
        delta = (destination - transform.position);
        float distance = delta.magnitude;
        this.transform.Translate(delta *Time.deltaTime);
        if (distance < 0.1f)
        {
            transform.position = destination;
            Task.current.Succeed();
        }
    }

    [Task]
    public void Rotate(float a, float v)
    {
        var task = Task.current;
        Debug.Log("Rotar");
        Task.current.Succeed();
    }

    [Task]
    public void debug()
    {
        Debug.Log("Hola");
    }

}

