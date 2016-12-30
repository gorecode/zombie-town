using UnityEngine;
using System.Collections;

public class SyncCharacterControllerWithAnimator : MonoBehaviour
{
    private CharacterController controller;
    private Animator avatar;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        avatar = GetComponent<Animator>();
    }

    void OnAnimatorMove()
    {
        controller.Move(avatar.deltaPosition);
        transform.rotation = avatar.rootRotation;
    }
}
