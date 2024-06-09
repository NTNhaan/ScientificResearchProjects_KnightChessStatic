using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    public enum Role
    {
        Player,
        Demon
    }
    public Slider TimeSliderDemon;
    public Slider TimeSliderHero;
    public float MaxTime = 100;
    public Role role;
    public void Awake()
    {
        TimeSliderHero.value = MaxTime;
        TimeSliderDemon.value = MaxTime;
    }
    public void Start()
    {
        role = Role.Player;
    }
    public void SwapRole()
    {
        if (role == Role.Player)
        {
            role = Role.Demon;
            // Debug.Log("Role: " + role);
            TimeSliderHero.value = MaxTime;
        }
        else if (role == Role.Demon)
        {
            role = Role.Player;
            // Debug.Log("Role: " + role);
            TimeSliderDemon.value = MaxTime;
        }
    }
    public void Update()
    {
        if (role == Role.Player)
        {
            TimeSliderHero.value -= Time.deltaTime * 10;
            if (TimeSliderHero.value <= 0)
            {
                SwapRole();
            }
        }
        else if (role == Role.Demon)
        {
            TimeSliderDemon.value -= Time.deltaTime * 10;
            if (TimeSliderDemon.value <= 0)
            {
                SwapRole();
            }
        }
    }

}