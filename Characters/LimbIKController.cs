using UnityEngine;
using System;
using ItchyOwl.Auxiliary;

namespace ItchyOwl.Characters
{
    [RequireComponent(typeof(Animator))]
    public class LimbIKController : Tweener
    {
        /// <summary>
        /// Do not change ikGoal in runtime after initialization, does not work!
        /// </summary>
        public AvatarIKGoal ikGoal;
        /// <summary>
        /// Enable if you need rudimentary smoothing. Useful especially when changing the targets and no other smoothing is used.
        /// </summary>
        public bool positionSmoothing = false;
        /// <summary>
        /// Enable rudimentary linear weight smoothing. For better tweening, use the tweener. Should be disabled if tweening is used.
        /// </summary>
        public bool weightSmoothing = false;
        /// <summary>
        /// Used only if positionSmoothing = true.
        /// </summary>
        public float positionalSmoothingSpeed = 5;
        /// <summary>
        /// Used only if weightSmoothing = true.
        /// </summary>
        public float weightSmoothingSpeed = 1;
        /// <summary>
        /// Used in Tweener functions for changing the ik weight.
        /// </summary>
        public Tween.EasingMode tweenMode = Tween.EasingMode.Smoother;

        public bool rotate;
        [SerializeField]
        private Vector3 _rotation;  // Original rotation is not stored if the value is accessed via inspector. For debugging/testing only!
        private Vector3 Rotation
        {
            get
            {
                if (_rotation == Vector3.zero)
                {
                    _rotation = originalRotation;
                }
                return _rotation;
            }
            set { _rotation = value; }
        }
        private Vector3 originalRotation;

        [SerializeField]
        [Range(0, 1)]
        private float targetWeight;
        [SerializeField]
        private Transform target;

        private int tweenId = 0;
        private float currentWeight;
        private Vector3 defaultPos;
        private Vector3 currentPos;
        private Vector3 targetPos;

        private Animator animator;
        private Animator Animator
        {
            get
            {
                if (animator == null)
                {
                    animator = GetComponent<Animator>();
                }
                return animator;
            }
        }

        [SerializeField]
        private Transform limb;
        public Transform Limb
        {
            get
            {
                if (limb == null)
                {
                    switch (ikGoal)
                    {
                        case AvatarIKGoal.LeftFoot:
                            limb = Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                            break;
                        case AvatarIKGoal.LeftHand:
                            limb = Animator.GetBoneTransform(HumanBodyBones.LeftHand);
                            break;
                        case AvatarIKGoal.RightFoot:
                            limb = Animator.GetBoneTransform(HumanBodyBones.RightFoot);
                            break;
                        case AvatarIKGoal.RightHand:
                            limb = Animator.GetBoneTransform(HumanBodyBones.RightHand);
                            break;
                        default:
                            throw new NotImplementedException(ikGoal.ToString());
                    }
                    if (limb == null)
                    {
                        Debug.LogWarning("[LimbIKController] Cannot find the hand transform, please define manually.");
                        enabled = false;
                    }
                    else
                    {
                        defaultPos = limb.position;
                        targetPos = defaultPos;
                        currentPos = defaultPos;
                    }
                }
                return limb;
            }
        }

        void Start()
        {
            if (!InitReady) { Init(); }
        }

        public bool InitReady { get; private set; }
        public void Init()
        {
            limb = null;
            Debug.Log("[LimbIKController] Init " + Limb);
            InitReady = true;
        }

        #region For testing
        [SerializeField]
        private bool reset;
        [SerializeField]
        private bool reach;
        private bool isReaching;
        #endregion

        private void Update()
        {
            #region For testing
            if (reset)
            {
                ResetIK();
                reset = false;
            }
            if (reach && !isReaching)
            {
                ReachTowards(target);
            }
            if (!reach && isReaching)
            {
                StopReaching(disable: false);
            }
            #endregion

            // Update the target position in case that the target moves
            if (target != null && isReaching)
            {
                targetPos = target.position;
            }
        }

        private void OnAnimatorIK()
        {
            currentPos = positionSmoothing ? Vector3.MoveTowards(currentPos, targetPos, Time.deltaTime * positionalSmoothingSpeed) : targetPos;
            currentWeight = weightSmoothing ? Mathf.MoveTowards(currentWeight, targetWeight, Time.deltaTime * weightSmoothingSpeed) : targetWeight;
            Animator.SetIKPosition(ikGoal, currentPos);
            Animator.SetIKPositionWeight(ikGoal, currentWeight);
            if (rotate)
            {
                Animator.SetBoneLocalRotation(GetBone(), Quaternion.Euler(Rotation));
            }
        }

        public void RotateTo(Vector3 eulers, float tweenDuration = 1)
        {
            if (InitReady) { Init(); }
            enabled = true;
            rotate = true;
            originalRotation = Animator.GetBoneTransform(GetBone()).localEulerAngles;
            Rotation = originalRotation;
            if (tweenDuration > 0)
            {
                TweenRotX(eulers.x, tweenDuration);
                TweenRotY(eulers.y, tweenDuration);
                TweenRotZ(eulers.z, tweenDuration);
            }
            else
            {
                Rotation = eulers;
            }
        }

        public void RotateBy(Vector3 eulers, float tweenDuration = 1)
        {
            if (InitReady) { Init(); }
            enabled = true;
            rotate = true;
            originalRotation = Animator.GetBoneTransform(GetBone()).localEulerAngles;
            Rotation = originalRotation;
            if (tweenDuration > 0)
            {
                TweenRotX(originalRotation.x + eulers.x, tweenDuration);
                TweenRotY(originalRotation.y + eulers.y, tweenDuration);
                TweenRotZ(originalRotation.z + eulers.z, tweenDuration);
            }
            else
            {
                Rotation = originalRotation + eulers;
            }
        }

        public void StopRotating(float tweenDuration = 1, bool disable = false)
        {
            if (InitReady) { Init(); }
            if (!rotate) { return; }
            Action callback = null;
            if (disable)
            {
                callback = () => ResetAndDisable();
            }
            else
            {
                callback = () => rotate = false;
            }
            TweenRotX(originalRotation.x, tweenDuration, callback);
            TweenRotY(originalRotation.y, tweenDuration, callback);
            TweenRotZ(originalRotation.z, tweenDuration, callback);
        }

        public void ReachTowards(Transform target, float weight = 1, float tweenDuration = 1)
        {
            this.target = target;
            ReachTowards(target.position, weight, tweenDuration);
        }

        /// <summary>
        /// Can be used for static positions. For moving targets, use ReachTowards(Transform target).
        /// </summary>
        public void ReachTowards(Vector3 target, float weight = 1, float tweenDuration = 1)
        {
            if (InitReady) { Init(); }
            enabled = true;
            reach = true;
            isReaching = true;
            currentPos = Limb.position;
            targetPos = target;
            if (tweenDuration > 0)
            {
                weightSmoothing = false;
                TweenTo(tweenId, to: weight, duration: tweenDuration, easing: tweenMode, updateCallback: value => targetWeight = value);
            }
            else
            {
                weightSmoothing = true;
                targetWeight = weight;
            }
        }

        public void StopReaching(float tweenDuration = 1, bool disable = false)
        {
            if (InitReady) { Init(); }
            if (!isReaching) { return; }
            isReaching = false;
            reach = false;
            Action callback = null;
            if (disable)
            {
                callback = () => ResetAndDisable();
            }
            // If this is the first time we tween, "from" value has to be provided, because by default it's 0, which is not what we want in this case.
            if (Tweens.ContainsKey(tweenId))
            {
                TweenTo(tweenId, to: 0, duration: tweenDuration, easing: tweenMode, updateCallback: value => targetWeight = value, readyCallback: callback);
            }
            else
            {
                TweenTo(tweenId, to: 0, from: targetWeight, duration: tweenDuration, easing: tweenMode, updateCallback: value => targetWeight = value, readyCallback: callback);
            }
        }

        public void ResetIK()
        {
            if (InitReady) { Init(); }
            StopAll();
            target = null;
            targetPos = defaultPos;
            isReaching = false;
            reach = false;
            rotate = false;
            _rotation = Vector3.zero;
            originalRotation = Vector3.zero;
        }

        public void ResetAndDisable()
        {
            ResetIK();
            enabled = false;
        }

        private HumanBodyBones GetBone()
        {
            switch (ikGoal)
            {
                case AvatarIKGoal.LeftHand:
                    return HumanBodyBones.LeftHand;
                case AvatarIKGoal.RightHand:
                    return HumanBodyBones.RightHand;
                case AvatarIKGoal.LeftFoot:
                    return HumanBodyBones.LeftFoot;
                case AvatarIKGoal.RightFoot:
                    return HumanBodyBones.RightFoot;
                default:
                    throw new NotImplementedException(ikGoal.ToString());
            }
        }

        private void TweenRotX(float x, float tweenDuration, Action callback = null)
        {
            TweenTo(tweenId + 1, to: x, from: Rotation.x, duration: tweenDuration, easing: tweenMode, updateCallback: value => Rotation = new Vector3(value, Rotation.y, Rotation.z), readyCallback: callback);
        }

        private void TweenRotY(float y, float tweenDuration, Action callback = null)
        {
            TweenTo(tweenId + 2, to: y, from: Rotation.y, duration: tweenDuration, easing: tweenMode, updateCallback: value => Rotation = new Vector3(Rotation.x, value, Rotation.z), readyCallback: callback);
        }

        private void TweenRotZ(float z, float tweenDuration, Action callback = null)
        {
            TweenTo(tweenId + 3, to: z, from: Rotation.z, duration: tweenDuration, easing: tweenMode, updateCallback: value => Rotation = new Vector3(Rotation.x, Rotation.y, value), readyCallback: callback);
        }
    }
}
