using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGamePresentationController : MonoBehaviour
{
    public GameObject WinMeter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SmallWinCelebration ()
    {
        var animator = WinMeter.GetComponent<Animator>();
        StartCoroutine(AnimatorBool(animator, 2.5f, "SmallWinCelebration"));
    }

    public void BigWinCelebration()
    {
        var animator = WinMeter.GetComponent<Animator>();
        StartCoroutine(AnimatorBool(animator, 3f, "BigWinCelebration"));
    }

    public IEnumerator AnimatorBool(Animator animator, float seconds, string name)
    {
        animator.SetBool(name, true);
        yield return new WaitForSeconds(seconds);
        animator.SetBool(name, false);
        
        yield return null;
    }
}
