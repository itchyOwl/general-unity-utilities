// Modified from http://docs.unity3d.com/Manual/nav-CouplingAnimationAndNavigation.html
using UnityEngine;
using ItchyOwl.Extensions;

namespace ItchyOwl.Characters
{
    /// <summary>
    /// NOTE: This is an old script, which might need refactoring.
    /// </summary>
    [RequireComponent(typeof(RootMotionAgent))]
    [RequireComponent(typeof(Animator))]
    public class RootMotionAnimator : MonoBehaviour
    {
        public Transform headTransform;
        public float lookAtCoolTime = 0.2f;
        public float lookAtHeatTime = 0.2f;
        public bool enableLooking = true;
        public float steeringTargetMinDistance = 1;

        protected Vector3 lookAtPosition;
        protected Vector3 lookAtTargetPosition;
        protected float lookAtWeight;

        protected Animator _animator;
        public Animator Animator
        {
            get
            {
                _animator = gameObject.GetReferenceTo(_animator);
                return _animator;
            }
        }

        protected RootMotionAgent _agent;
        public RootMotionAgent RootMotionAgent
        {
            get
            {
                _agent = gameObject.GetReferenceTo(_agent);
                return _agent;
            }
        }

        protected void Awake()
        {
            if (!headTransform)
            {
                Debug.LogWarning("AnimationController: No head transform defined - looking disabled");
                enableLooking = false;
            }
            else
            {
                lookAtTargetPosition = headTransform.position + transform.forward;
                lookAtPosition = lookAtTargetPosition;
            }
        }

        protected void Update()
        {
            Animator.SetFloat("Turn", RootMotionAgent.Turn);
            Animator.SetFloat("Forward", RootMotionAgent.Forward);
            if (enableLooking && RootMotionAgent.Agent.remainingDistance > steeringTargetMinDistance)
            {
                lookAtTargetPosition = RootMotionAgent.Agent.steeringTarget + transform.forward;
            }
        }

        // TODO: tweak the values and promote to variables
        protected void OnAnimatorIK()
        {
            if (!enableLooking) { return; }
            lookAtTargetPosition.y = headTransform.position.y;
            float lookAtTargetWeight = enableLooking ? 1.0f : 0.0f;
            Vector3 curDir = lookAtPosition - headTransform.position;
            Vector3 futDir = lookAtTargetPosition - headTransform.position;
            curDir = Vector3.RotateTowards(curDir, futDir, 6.28f * Time.deltaTime, float.PositiveInfinity);
            lookAtPosition = headTransform.position + curDir;
            float blendTime = lookAtTargetWeight > lookAtWeight ? lookAtHeatTime : lookAtCoolTime;
            lookAtWeight = Mathf.MoveTowards(lookAtWeight, lookAtTargetWeight, Time.deltaTime / blendTime);
            _animator.SetLookAtWeight(lookAtWeight, 0.2f, 0.5f, 0.7f, 0.5f);
            _animator.SetLookAtPosition(lookAtPosition);
        }
    }
}
