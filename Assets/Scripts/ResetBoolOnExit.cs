using UnityEngine;

public class ResetBoolOnExit : StateMachineBehaviour
{
    // Chamado quando a anima��o termina
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isAttacking", false);
    }
}
