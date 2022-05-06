using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class IntParameterRandomSetter : StateMachineBehaviour
{
    //public string randomIdleState;

    public int stateCount = 0;

    int randomInt;

    bool valueIncreasing;

    float increment;

    public AnimationClip idle;

    public AnimationClip[] idleAnimations;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        valueIncreasing = true;

        randomInt = Random.Range(0, 4);

        animator.SetInteger("IdleIndex", randomInt);
    }

    /*public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        *//*if (randomInt >= stateCount)
        {
            valueIncreasing = false;
        }
        if(randomInt <= 1)
        {
            valueIncreasing = true;
        }

        if (valueIncreasing)
        {
            increment = 0.001f;
        }

        if(!valueIncreasing)
        {
            increment = -0.001f;
        }

        randomInt += increment;

        

        animator.SetFloat("IdleIndex", randomInt);*//*
    }*/


}
