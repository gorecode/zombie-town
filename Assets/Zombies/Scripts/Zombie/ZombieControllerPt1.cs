/// <summary>
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections;
using RootMotion.FinalIK;

[RequireComponent(typeof(Animator))]  

//Name of class must be name of file as well

public class ZombieControllerPt1 : MonoBehaviour
{
    public enum State
    {
        IDLE,
        HUNTING
    }

    public Transform target = null;
    public float distanceToSee = 30f;
    public float distanceToFollow = 1;
    public float distanceToStartLookAt = 4;
    public float turnAroundSpeed = 40;
    protected Animator avatar;
    protected CharacterController controller;
    public float distanceForMeleeAttack = 1f;
    public float distanceForRangeAttack = 1.5f;
    public float distanceForJumpAttack = 2f;
    public float delayBetweenRangeAttacks = 4f;
    public float delayBetweenJumpAttacks = 10f;
    public float maxSpeed = 1f;
    public float lastRangeAttackTime;
    public float lastJumpAttackTime;
    public float minIdleChangeDelay = 10;
    public float maxIdleChangeDelay = 20;
	
    private LookAtIK ik;
    private float SpeedDampTime = .1f;
    private float DirectionDampTime = .25f;
    private LTDescr lookAtWeightLTD;
    private Transform head;
    private State state = State.IDLE;
    private bool forceHunt;
    private float idleType;

    public void ForceHunt()
    {
        forceHunt = true;
    }

    public State GetState()
    {
        return state;
    }

    void Start()
    {
        avatar = GetComponent<Animator>();
        ik = GetComponent<LookAtIK>();
        head = avatar.GetBoneTransform(HumanBodyBones.Head);
        avatar.speed = 1 + UnityEngine.Random.Range(-0.3f, 0.8f);

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            ik.solver.target = target.Find("Head");
        }

        float newSpeed = UnityEngine.Random.Range(1f, maxSpeed + 0.1f);

        ChangeIdleAnim();

        if (newSpeed >= 2f)
        {
            maxSpeed = 2f;
        }
        else
        {
            maxSpeed = 1f;
        }
    }

    void ChangeIdleAnim()
    {
        idleType = (int)UnityEngine.Random.Range(0, 5);

        Invoke("ChangeIdleAnim", UnityEngine.Random.Range(minIdleChangeDelay, maxIdleChangeDelay));
    }

    void Update()
    {
        if (avatar && target)
        {
            Vector3 curentDir = avatar.rootRotation * Vector3.forward;
            Vector3 wantedDir = (target.position - avatar.rootPosition).normalized;

            float distance = Vector3.Distance(target.position, avatar.rootPosition);

            if (withinSight(target, 60) && distance <= distanceToSee || forceHunt)
            {
                forceHunt = false;

                state = State.HUNTING;

                CancelInvoke("BecomeIdle");
            }
            else
            {
                if (!IsInvoking("BecomeIdle"))
                {
                    Invoke("BecomeIdle", 3f);
                }
            }

            if (state == State.HUNTING)
            {
                float direction = Vector3.Cross(curentDir, wantedDir).y;

                transform.Rotate(0, direction * turnAroundSpeed * Time.deltaTime * avatar.speed, 0);

                float targetLookAtWeight = 0f;

                if (distance < distanceToStartLookAt)
                {
                    targetLookAtWeight = 1f;
                }

                if (lookAtWeightLTD == null)
                {
                    float lastWeight = ik.solver.IKPositionWeight;

                    lookAtWeightLTD = LeanTween.value(gameObject, lastWeight, targetLookAtWeight, 1f).setOnUpdate((float val) =>
                        {
                            ik.solver.IKPositionWeight = val;
                        }).setOnComplete((object o) =>
                        {
                            lookAtWeightLTD = null;
                        });
                }

                if (distance > distanceToFollow)
                {
                    avatar.SetFloat("Speed", maxSpeed, SpeedDampTime, Time.deltaTime);
                }
                else
                {
                    avatar.SetFloat("Speed", 0, SpeedDampTime, Time.deltaTime);
                }

                if (distance < distanceForRangeAttack && Time.time - lastRangeAttackTime > delayBetweenRangeAttacks)
                {
                    lastRangeAttackTime = Time.time;
                    avatar.SetTrigger("Scream");
                }
                else if (distance < distanceForJumpAttack && Time.time - lastJumpAttackTime > delayBetweenJumpAttacks && maxSpeed >= 2f)
                {
                    lastJumpAttackTime = Time.time;
                    avatar.SetTrigger("JumpAttack");
                }
                else if (distance < distanceForMeleeAttack)
                {
                    avatar.SetTrigger("MeleeAttack");
                }
            }
            else if (state == State.IDLE)
            {
                avatar.SetFloat("Speed", 0, SpeedDampTime, Time.deltaTime);
                avatar.SetFloat("IdleType", idleType, 1f, Time.deltaTime);
            }

        }
    }

    public void BecomeIdle()
    {
        state = State.IDLE;
    }
    // Returns true if targetTransform is within sight of current transform
    public bool withinSight(Transform targetTransform, float fieldOfViewAngle)
    {
        Vector3 direction = targetTransform.position - head.transform.position;
        // An object is within sight if the angle is less than field of view
        return Vector3.Angle(direction, head.transform.forward) < fieldOfViewAngle;
    }
}
