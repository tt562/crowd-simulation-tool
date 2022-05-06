using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingAnimationController : StateMachineBehaviour
{
    //public string randomIdleState;

    public float min;

    public float max;

    public FormationPosition.GroupSide groupSide;

    float randomFloat;

    bool valueIncreasing;

    float increment;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        GameObject formationPoint = animator.GetComponent<NPCAI>().closestFreeFormationPoint;

        if (formationPoint != null)
        {
            groupSide = formationPoint.GetComponent<FormationPosition>().groupSide;
        }
        else
        {
            groupSide = FormationPosition.GroupSide.NoSide;
        }

        valueIncreasing = true;

        if (groupSide == FormationPosition.GroupSide.Left)
        {
            min = -1.0f;
            max = 0.0f;
            randomFloat = Random.Range(min, max);
        }
        if(groupSide == FormationPosition.GroupSide.Right)
        {
            min = 0.0f;
            max = 1.0f;
            randomFloat = Random.Range(min, max);
        }

        animator.SetFloat("IdleType", randomFloat);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (randomFloat >= max)
        {
            valueIncreasing = false;
        }
        if (randomFloat <= min)
        {
            valueIncreasing = true;
        }

        if (valueIncreasing)
        {
            increment = 0.005f;
        }

        if (!valueIncreasing)
        {
            increment = -0.005f;
        }

        randomFloat += increment;



        animator.SetFloat("WalkingType", randomFloat);
    }
}
