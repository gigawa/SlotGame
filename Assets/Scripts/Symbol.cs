using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol : MonoBehaviour
{
    private Animator animator;

    public AnimationClip WinCelebration;
    public AnimationClip Idle;

    public int minRng;
    public int maxRng;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        SetAnimations();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetAnimations()
    {
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        
        overrideController["Idle"] = Idle;
        overrideController["Celebration"] = WinCelebration;

        animator.runtimeAnimatorController = overrideController;
    }

    public void PlayWin()
    {
        animator.SetBool("WinCelebration", true);
    }

    public void StopWin()
    {
        animator.SetBool("WinCelebration", false);
    }
}
