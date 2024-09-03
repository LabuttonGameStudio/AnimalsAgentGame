using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAnimator : MonoBehaviour
{
    public static MeleeAnimator Instance;
    Animator animator;
    SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private TrailRenderer[] trailRenderersLeft;
    [SerializeField] private TrailRenderer[] trailRenderersRight;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        skinnedMeshRenderer = animator.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        skinnedMeshRenderer.enabled = false;
        Instance = this;
        foreach (TrailRenderer trailRenderer in trailRenderersLeft)
        {
            trailRenderer.enabled = false;
        }
        foreach (TrailRenderer trailRenderer in trailRenderersRight)
        {
            trailRenderer.enabled = false;
        }
    }
    private int lastMeleeAnimation;
    private bool lastMeleeDirection;
    public void PlayRandomMeleeAnimation()
    {
        if(disableVisual_Ref != null)
        {
            return;
        }

        string meleeAnimation= "Melee";

        int currentAnimation;
        currentAnimation = Random.Range(0, 3);
        while (lastMeleeAnimation == currentAnimation)
        {
            currentAnimation = Random.Range(0, 3);
        }
        lastMeleeAnimation = currentAnimation;
        meleeAnimation += "_"+currentAnimation;

        bool currentDirection;
        currentDirection = 1 ==Random.Range(0, 2);
        if (lastMeleeDirection == currentDirection)
        {
            currentDirection = !currentDirection;
        }
        lastMeleeDirection = currentDirection;
        meleeAnimation += currentDirection? "L":"R";
        skinnedMeshRenderer.enabled = true;
        if (currentDirection)
        {
            foreach (TrailRenderer trailRenderer in trailRenderersLeft)
            {
                trailRenderer.enabled = true;
            }
        }
        else
        {
            foreach (TrailRenderer trailRenderer in trailRenderersRight)
            {
                trailRenderer.enabled = true;
            }
        }
        animator.CrossFade(meleeAnimation, 0.2f);

        disableVisual_Ref = StartCoroutine(DisableVisual_Coroutine(0.469f));
    }
    private Coroutine disableVisual_Ref;
    private IEnumerator DisableVisual_Coroutine(float timer)
    {
        yield return new WaitForSeconds(timer);
        skinnedMeshRenderer.enabled = false;
        disableVisual_Ref = null;
        foreach (TrailRenderer trailRenderer in trailRenderersLeft)
        {
            trailRenderer.enabled = false;
        }
        foreach (TrailRenderer trailRenderer in trailRenderersRight)
        {
            trailRenderer.enabled = false;
        }
    }


    private bool lastTakedownDirection;

    public void PlayTakedownAnimation()
    {
        if (disableVisual_Ref != null)
        {
            return;
        }
        string takedownAnimation = "Takedown_0";
        bool currentDirection;
        currentDirection = 1 == Random.Range(0, 2);
        if (lastTakedownDirection == currentDirection)
        {
            currentDirection = !currentDirection;
        }
        lastTakedownDirection = currentDirection;
        takedownAnimation += currentDirection ? "L" : "R";

        skinnedMeshRenderer.enabled = true;
        animator.CrossFade(takedownAnimation, 0.2f);
        disableVisual_Ref = StartCoroutine(DisableVisual_Coroutine(1.406f));
    }
}
