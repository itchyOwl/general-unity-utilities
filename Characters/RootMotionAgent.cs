using UnityEngine;
using System;
using ItchyOwl.General;
using UnityEngine.AI;
using ItchyOwl.Extensions;

namespace ItchyOwl.Characters
{
    /// <summary>
    /// This script handles movement so that the navigation agent is used to determine the target position, but the actual movement is handled via animation root motion.
    /// Rotation and movement are calculated, but not applied, unless rotateCharacterTowardAgent and pullCharacterTowardAgent are set true.
    /// By default, the movement control is given to the animator, but the rotation control is not.
    /// The calculations are sent in MovementUpdate event, which clients should use for example for sending the data to the animator.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class RootMotionAgent : MonoBehaviour
    {
        public float Forward { get; protected set; }
        public float Turn { get; protected set; }

        [SerializeField]
        protected float stoppingMargin = 0.2f;
        [SerializeField]
        /// <summary>
        /// Increase, if the agent stutters.
        /// </summary>
        protected float agentStoppingDistance = 0.1f;
        /// <summary>
        /// stoppingMargin + Agent.stoppingDistance
        /// </summary>
        public float TotalStoppingDistance { get { return stoppingMargin + Agent.stoppingDistance; } }

        /// <summary>
        /// If set false, the character is rotated only via root motion. This is applied only when moving via agent.
        /// </summary>
        public bool rotateCharacterTowardAgent = true;
        // Pulls the character towards agent position. Root motion is still used, but this adds to the total translation.
        public bool pullCharacterTowardAgent;
        // How fast the character is pulled toward the agent? Can be used as movement speed, when root motion is not applied
        protected float pullSpeed = 0.5f;
        // Enable to debug the movement with mouse (point and click)
        [SerializeField]
        private bool enableDebugMouseMovement;
        public float maxFacingAngleDeviation = 120;

        // Disable if the agent is following waypoints and if the next is not the last waypoint. Automatically reset to true when a movement is started -> set false after the movement command, if you want to temporarily disable the value.
        public bool autoBreaking = true;
        public bool reverse;

        [SerializeField]
        protected float smoothTime = 0.2f;
        [SerializeField]
        [Range(-10, 0)]
        protected float forwardLerpMin = -3;
        [SerializeField]
        [Range(1, 10)]
        protected float forwardLerpMax = 6;

        private NavMeshAgent _agent;
        public NavMeshAgent Agent
        {
            get
            {
                _agent = gameObject.GetReferenceTo(_agent);
                return _agent;
            }
        }

        protected float maxForwardValue;
        protected float currentTurnVelocity;
        protected float currentForwardVelocity;
        protected float turn;
        protected float forward;

        protected bool stop;
        /// <summary>
        /// Used for temporarily disabling turning when turning is handled differently.
        /// </summary>
        protected bool disableTurnManipulation;

        protected virtual void Start()
        {
            Agent.Warp(transform.position);
            Agent.updatePosition = false;
            Agent.updateRotation = false;
            Agent.stoppingDistance = agentStoppingDistance;
        }

        protected virtual void Update()
        {
            if (enableDebugMouseMovement && Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray.origin, ray.direction, out hit))
                {
                    Agent.destination = hit.point;
                }
            }
            var movementVectorToNextPoint = Agent.nextPosition - transform.position;
            var movementVectorLength = movementVectorToNextPoint.magnitude;
            //var movementVectorToEndTarget = Agent.steeringTarget - transform.position;
            //Debug.DrawRay(transform.position, movementVectorToNextPoint, Color.red);
            bool shouldMove = movementVectorLength > TotalStoppingDistance;
            if (stop) { shouldMove = false; }
            if (!disableTurnManipulation)
            {
                if (shouldMove)
                {
                    var localVector = transform.InverseTransformDirection(movementVectorToNextPoint.normalized);
                    if (reverse)
                    {
                        localVector = -localVector;
                    }
                    //Debug.DrawRay(transform.position, localVector, Color.green);
                    turn = Mathf.Atan2(localVector.x, localVector.z);
                }
                else
                {
                    turn = 0;
                }
            }
            bool isFacing = transform.IsFacingDir(movementVectorToNextPoint, maxFacingAngleDeviation, lockYAxis: true, inverse: reverse);
            if (shouldMove && isFacing)
            {
                if (autoBreaking)
                {
                    float fLerp = Mathf.InverseLerp(forwardLerpMin, forwardLerpMax, movementVectorLength);
                    forward = Mathf.Clamp(Mathf.Lerp(0, 1, fLerp), 0, maxForwardValue);
                }
                else
                {
                    forward = maxForwardValue;
                }
                if (reverse)
                {
                    forward = -forward;
                }
                if (rotateCharacterTowardAgent && !disableTurnManipulation)
                {
                    ApplyRotationFix(movementVectorToNextPoint);
                }
                if (pullCharacterTowardAgent)
                {
                    ApplyTranslationFix(Agent.nextPosition);
                }
            }
            else
            {
                forward = 0;
            }
            Turn = Mathf.SmoothDamp(Turn, turn, ref currentTurnVelocity, smoothTime);
            Forward = Mathf.SmoothDamp(Forward, forward, ref currentForwardVelocity, smoothTime);
        }

        private void ApplyRotationFix(Vector3 movementVector)
        {
            transform.RotateTowardDir(movementVector, Agent.angularSpeed / 100, lockYAxis: true, inverse: reverse);
        }

        private void ApplyTranslationFix(Vector3 targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, pullSpeed * Forward * Time.deltaTime);
        }
    }
}
