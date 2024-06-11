using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapTurn : MonoBehaviour
{

    private bool isSwapping = false;
    public bool IsSwapping
    {
        get { return isSwapping; }
    }
    void Awake()
    {
        //TimeBar.Instance.animator = GetComponent<Animator>();
    }
    public void OnAnimationEnd()
    {
        TimeBar.Instance.SwapRole();
        TimeBar.Instance.ResetAnimation();
    }
}
