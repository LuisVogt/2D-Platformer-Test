using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float currentTime;
    private float maxTime;

    public Timer(float _maxTime)
    {
        maxTime = _maxTime;
        currentTime = 0;
    }

    public bool isDone()
    {
        if(currentTime<=0)
        {
            return true;
        }
        return false;
    }

    public void StartTimer()
    {
        currentTime = maxTime;
    }

    public void Update(float deltaTime)
    {
        if(currentTime>0)
        {
            currentTime -= deltaTime;
            if (currentTime < 0)
                currentTime = 0;
        }
    }
}
