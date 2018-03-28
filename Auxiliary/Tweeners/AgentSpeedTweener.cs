using UnityEngine;
using UnityEngine.AI;
using ItchyOwl.Extensions;

namespace ItchyOwl.Auxiliary
{
    public class AgentSpeedTweener : Tweener
    {
        [SerializeField]
        private NavMeshAgent _agent;
        public NavMeshAgent Agent
        {
            get
            {
                _agent = gameObject.GetReferenceTo(_agent, seekChildren: true, includeInactive: true);
                return _agent;
            }
            set
            {
                _agent = value;
            }
        }

        public void TweenSpeed(float to, float? from = null, float duration = 1)
        {
            TweenTo(0, to, from, duration, updateCallback: value => Agent.speed = to);
        }
    }
}
