using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Idel : StateMachineBehaviour
{
    [SerializeField] Boss boss;
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Boss>();
       if (boss == null){
        Debug.LogError("Không tìm thấy Boss!");
       }
       
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       boss.IdelState();
    }

   
}
