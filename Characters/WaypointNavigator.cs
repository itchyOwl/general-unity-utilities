using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ItchyOwl.Extensions;
using ItchyOwl.Auxiliary;

namespace ItchyOwl.Characters
{
    /// <summary>
    /// This script controls the navigation agent movement with predefined waypoints
    /// NOTE: This is an old script, which might need refactoring.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public class WaypointNavigator : MonoBehaviour
    {
        public List<Transform> waypoints = new List<Transform>();
        public float movementUpdateDelay = 0.1f;
        public float movementMargin = 0.1f;
        public bool movementEnabled = true;
        public bool updatePosition; // Set false if the movement is handled via root motion
        public bool updateRotation; // Set false if the rotation is handled via root motion
        public bool navigateOnStart;
        public bool loop;

        private bool isMoving;
        private bool isRunning;
        private float defaultSpeed;
        private UnityEngine.AI.NavMeshAgent _agent;
        private UnityEngine.AI.NavMeshAgent Agent
        {
            get
            {
                if (_agent == null) { _agent = GetComponent<UnityEngine.AI.NavMeshAgent>(); }
                return _agent;
            }
        }
        private RootMotionAgent _rootMotionAgent;
        private RootMotionAgent RootMotionAgent
        {
            get
            {
                if (_rootMotionAgent == null) { _rootMotionAgent = GetComponent<RootMotionAgent>(); }
                return _rootMotionAgent;
            }
        }
        private IEnumerator<Transform> _iterator;
        private IEnumerator<Transform> Iterator
        {
            get
            {
                if (_iterator == null) { _iterator = waypoints.GetEnumerator(); }
                return _iterator;
            }
        }

        public event Action MovementStarted = () => Debug.Log("WaypointNavigator: Movement started.");
        public event Action MovementContinued = () => Debug.Log("WaypointNavigator: Continuing movement.");
        /// <summary>
        /// Returns true if has reached the target.
        /// </summary>
        public event Action<bool> MovementStopped = value => Debug.Log("WaypointNavigator: Movement stopped. Target reached: " + value);

        // If you want to implement a public event, make a new, because this is overridden each time StartChainedMovement is called.
        // We could use callback instead of a private event here, but it makes the code more complex.
        private event Action TargetReached;

        void Awake()
        {
            PauseController.GamePaused += (sender, args) => movementEnabled = false;
            PauseController.GameContinue += (sender, args) => movementEnabled = true;
        }

        void Start()
        {
            if (navigateOnStart)
            {
                StartChainedMovement(loop: loop);
            }
        }

        public void Disable()
        {
            if (Agent.isOnNavMesh && Agent.isActiveAndEnabled)
            {
                isMoving = false;
                Agent.isStopped = !isMoving;
            }
            Reset();
            Agent.enabled = false;
        }

        public void StartChainedMovement(bool loop, int waypointCount = 0, Action callback = null, float speed = 0, float secondsToWaitAtWP = 0)
        {
            enabled = true;
            Agent.enabled = true;
            Reset();
            this.loop = loop;
            // Override the event so that all the old listeners are cleared
            TargetReached = () => { }; //Debug.Log("WaypointNavigator: Target reached");
                                                   //Debug.Log("WaypointNavigator: Starting chained movement (" + waypointCount + ")");
            if (speed > 0) { Agent.speed = speed; }
            int trueWaypointCount = waypointCount > 0 ? waypointCount : waypoints.Count;
            int waypointsRemaining = trueWaypointCount;
            if (RootMotionAgent != null)
            {
                RootMotionAgent.autoBreaking = trueWaypointCount < 2;
            }
            TargetReached += () =>
            {
                waypointsRemaining--;
                if (waypointsRemaining == 1 && RootMotionAgent != null)
                {
                    // Reset adjustForwardSpeedtoRemainingDistance in order to smooth out the stopping of the character
                    float distance = transform.GetDistanceTo(waypoints[trueWaypointCount - 1]) - (Agent.stoppingDistance + movementMargin);
                    float timeToDestination = distance / Agent.speed;
                    this.DelayedMethod(() => RootMotionAgent.autoBreaking = true, timeToDestination / 2);
                }
                if (waypointsRemaining > 0)
                {
                    MoveToNext(loop, true, secondsToWaitAtWP);
                }
                else
                {
                    //Debug.Log("WaypointNavigator: No more targets.");
                    Stop(true);
                    Agent.speed = defaultSpeed;
                    if (callback != null) { callback(); }
                }
            };
            MoveToNext(loop, true, secondsToWaitAtWP);
        }

        public void Reset()
        {
            Agent.updatePosition = updatePosition;
            Agent.updateRotation = updateRotation;
            StopAllCoroutines();
            _iterator = null;
            isMoving = false;
            isRunning = false;
            defaultSpeed = Agent.speed;
            if (Agent.isActiveAndEnabled && Agent.isOnNavMesh)
            {
                Agent.ResetPath();
            }
        }

        private void MoveToNext(bool loop = false, bool overrideStopping = false, float secondsToWaitAtWP = 0)
        {
            if (isRunning) { StopAllCoroutines(); }
            if (loop)
            {
                var current = Iterator.GetNext().First();
                //Debug.Log("WaypointNavigator: Moving to the next target.");
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(MoveCoroutine(current, overrideStopping, secondsToWaitAtWP));
                }
            }
            else
            {
                if (Iterator.MoveNext())
                {
                    if (gameObject.activeInHierarchy)
                    {
                        //Debug.Log("WaypointNavigator: Moving to the next target.");
                        StartCoroutine(MoveCoroutine(Iterator.Current, overrideStopping, secondsToWaitAtWP));
                    }
                }
                else
                {
                    //Debug.Log("WaypointNavigator: No more targets.");
                    if (overrideStopping)
                    {
                        Stop(true);
                    }
                }
            }
        }

        private void Stop(bool targetReached)
        {
            isMoving = false;
            Agent.isStopped = !isMoving;
            MovementStopped(targetReached);
        }

        private IEnumerator MoveCoroutine(Transform target, bool overrideStopping = false, float secondsToWaitAtWP = 0)
        {
            isRunning = true;
            //Debug.Log("WaypointNavigator: Starting movement towards " + target);
            if (target == null) { throw new Exception("WaypointNavigator: Target null."); }
            bool movementHasStarted = false;
            var wait = new WaitForSeconds(movementUpdateDelay);
            var waitAtWP = new WaitForSeconds(secondsToWaitAtWP);
            while (true)
            {
                bool targetReached = !transform.IsFartherFromThan(target.position, Agent.stoppingDistance + movementMargin);
                while (!movementEnabled)
                {
                    if (isMoving)
                    {
                        Stop(targetReached);
                    }
                    yield return null;
                }
                if (!isMoving)
                {
                    isMoving = true;
                    Agent.isStopped = !isMoving;
                    if (!movementHasStarted)
                    {
                        movementHasStarted = true;
                        MovementStarted();
                    }
                    else
                    {
                        MovementContinued();
                    }
                }
                if (targetReached)
                {
                    isRunning = false;
                    if (overrideStopping == false || secondsToWaitAtWP > 0)
                    {
                        Stop(targetReached: true);
                    }
                    yield return waitAtWP;
                    TargetReached();
                    yield break;
                }
                else
                {
                    Agent.SetDestination(target.position);
                    Debug.DrawLine(transform.position, target.position, Color.white, movementUpdateDelay);
                }
                yield return wait;
            }
        }
    }
}
