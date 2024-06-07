using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapTurn : MonoBehaviour
{
    public AnimationClip swapAnimation;
    public AnimationClip swapBackAnimation;
    private bool isSwapping = false;
    public bool IsSwapping
    {
        get { return isSwapping; }
    }
    protected TimeBar timeBar;
    void Awake()
    {
        timeBar = GetComponent<TimeBar>();
    }
    void Update()
    {

    }
    public void Swap()
    {
        isSwapping = true;
        StartCoroutine(SwapCoroutine());
    }
    private IEnumerator SwapCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play(swapAnimation.name);
            yield return new WaitForSeconds(swapAnimation.length);
            isSwapping = false;
        }
    }
    public void SwapBack()
    {
        isSwapping = true;
        StartCoroutine(SwapBackCoroutine());
    }
    private IEnumerator SwapBackCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play(swapBackAnimation.name);
            yield return new WaitForSeconds(swapBackAnimation.length);
            isSwapping = false;
        }
    }
}
