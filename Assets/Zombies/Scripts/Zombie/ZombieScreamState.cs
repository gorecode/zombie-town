using UnityEngine;
using System.Collections;

public class ZombieScreamState : StateMachineBehaviour
{
    public RandomSoundDescriptor sounds;

    private ZombieControllerPt2 zombie;
    private Transform scream;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioClip clip = sounds.audio[Random.Range(0, sounds.audio.Length)];

        zombie = animator.gameObject.GetComponent<ZombieControllerPt2>();
        zombie.PlaySound(clip);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
