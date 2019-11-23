using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeChanger : MonoBehaviour
{

    public void normalTime()
    {
        Time.timeScale = 1;
    }

    public void speedTime()
    {
        Time.timeScale = 1.5f;
    }

    public void maxSpeed()
    {
        Time.timeScale = 2f;
    }

    public void pause()
    {
        Time.timeScale = 0;
    }

    
}
