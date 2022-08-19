using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionOnStart : MonoBehaviour
{
    [SerializeField] Animator transitionAnimator;
    void Start()
    {
        transitionAnimator.SetTrigger("End");
    }
}
