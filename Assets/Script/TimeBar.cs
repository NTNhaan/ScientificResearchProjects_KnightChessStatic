using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    public Slider TimeSliderDemon;
    public Slider TimeSliderHero;
    public float MaxTime = 100f;
    private float LerpSpeed = 10f;
    public bool timeout;

    public void Start()
    {
        TimeSliderHero.value = MaxTime;
        TimeSliderDemon.value = MaxTime;
        timeout = true;
    }

    public void Update()
    {
        if (timeout)
        {
            TimeSliderHero.value -= Time.deltaTime * LerpSpeed;
            if(TimeSliderHero.value <= 0)
            {
                timeout = false;
                TimeSliderHero.value = MaxTime;
            }
        }
        else
        {
            TimeSliderDemon.value -= Time.deltaTime * LerpSpeed;
            if(TimeSliderDemon.value <= 0)
            {
                timeout = true;
                TimeSliderDemon.value = MaxTime;
            }
        } 
    }
    public float TakeTime(float time)
    {
        if (time <= 0)
        {
            return 0;
        }
        else
        {
            return Mathf.Lerp(time, 0, LerpSpeed * Time.deltaTime);
        }
    }
}
