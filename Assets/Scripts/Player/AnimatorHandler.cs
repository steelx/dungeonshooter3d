using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    [field: SerializeField]
    public bool CanRotate { get; private set; }
    public bool IsInteracting;
    public Animator anim;
    int horizontal;
    int vertical;

    PlayerLocomotion playerLocomotion;

    private void Awake()
    {
        playerLocomotion = GetComponentInParent<PlayerLocomotion>();
        anim = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    private void OnAnimatorMove()
    {
        if (!IsInteracting) return;
        // this helps reset rootMotion
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        //Vector3 velocity = deltaPosition / Time.deltaTime;
        playerLocomotion.chController.Move(deltaPosition);
    }

    private void Update()
    {
        IsInteracting = anim.GetBool("isInteracting");
    }

    public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
    {
        float delta = Time.deltaTime;
        #region Vertical
        float v = 0;
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            v = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            v = 1f;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            v = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            v = -1f;
        }
        else
        {
            v = 0;
        }
        #endregion

        #region Horizontal
        float h = 0;
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            h = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            h = 1f;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            h = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            h = -1f;
        }
        else
        {
            h = 0;
        }
        #endregion

        if (isSprinting)
        {
            v = 2.2f;
            h = horizontalMovement;
        }

        anim.SetFloat(vertical, v, 0.1f, delta);
        anim.SetFloat(horizontal, h, 0.1f, delta);
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        anim.applyRootMotion = isInteracting;
        anim.SetBool("isInteracting", isInteracting);
        anim.CrossFade(targetAnimation, 0.2f);
    }
}
