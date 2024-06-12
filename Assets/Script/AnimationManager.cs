using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator animator;
    void Start()
    {
        Debug.Log(animator.runtimeAnimatorController.name);
    }

    // Update is called once per fram
}
