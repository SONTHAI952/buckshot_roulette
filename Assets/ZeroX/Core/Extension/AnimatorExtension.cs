using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroX.Extensions
{
    public static class AnimatorExtension
    {
        public static bool IsAnimationFinished(this Animator animator, string stateName, float finishAtNormalizedTime = 1f)
        {
            animator.Update(0);
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            bool inTransition = animator.IsInTransition(0);
            return stateInfo.IsName(stateName) && stateInfo.normalizedTime >= finishAtNormalizedTime && !inTransition;
        }
        
        /// <summary>
        /// Nếu thời gian kết thúc state nhỏ hơn min duration thì sẽ gọi callback vs thời gian còn lại
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="coroutineRunner"></param>
        /// <param name="stateName">Tên của state animation để check</param>
        /// <param name="finishAtNormalizedTime"> thời gian animation trôi qua bao nhiêu thì coi là finish. Thích hợp trong việc muốn chừa lại để transition</param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        public static Coroutine OnFinishAnimation(this Animator animator, MonoBehaviour coroutineRunner, int layerIndex, string stateName, float finishAtNormalizedTime, Action onFinished)
        {
            return coroutineRunner.StartCoroutine(Timer());
            IEnumerator Timer()
            {
                animator.Update(0);
                int nameHash = Animator.StringToHash(stateName);
                bool stateFinished = false;
                var waitForEndOfFrame = new WaitForEndOfFrame();
                while (stateFinished == false)
                {
                    var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
                    bool inTransition = animator.IsInTransition(layerIndex);
                    stateFinished = stateInfo.shortNameHash == nameHash && stateInfo.normalizedTime >= finishAtNormalizedTime && !inTransition;
                    yield return waitForEndOfFrame;
                }
                
                onFinished.Invoke();
            }
        }

        public static Coroutine OnFinishAnimation(this Animator animator, MonoBehaviour coroutineRunner, string stateName, float finishAtNormalizedTime, Action onFinished)
        {
            return OnFinishAnimation(animator, coroutineRunner, 0, stateName, finishAtNormalizedTime, onFinished);
        }
        
        /// <summary>
        /// Nếu thời gian kết thúc state nhỏ hơn min duration thì sẽ gọi callback vs thời gian còn lại
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="coroutineRunner"></param>
        /// <param name="stateName">Tên của state animation để check</param>
        /// <param name="durationNeed">Thời gian cần là bao nhiêu, nếu animation state kết thúc trước thời gian này thì trả về thời gian còn lại</param>
        /// <param name="finishAtNormalizedTime"> thời gian animation trôi qua bao nhiêu thì coi là finish. Thích hợp trong việc muốn chừa lại để transition</param>
        /// <param name="onStateFinish"></param>
        /// <returns></returns>
        public static Coroutine OnFinishAnimation(this Animator animator, MonoBehaviour coroutineRunner, string stateName, float durationNeed, float finishAtNormalizedTime, UnityAction<float> onStateFinish)
        {
            return coroutineRunner.StartCoroutine(Timer());
            IEnumerator Timer()
            {
                animator.Update(0);
                float startTime = Time.time;
                int nameHash = Animator.StringToHash(stateName);
                bool stateFinished = false;
                var waitForEndOfFrame = new WaitForEndOfFrame();
                while (stateFinished == false)
                {
                    var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    bool inTransition = animator.IsInTransition(0);
                    stateFinished = stateInfo.shortNameHash == nameHash && stateInfo.normalizedTime >= finishAtNormalizedTime && !inTransition;
                    yield return waitForEndOfFrame;
                }

                float remainingTime = durationNeed - (Time.time - startTime);
                if (remainingTime < 0)
                    remainingTime = 0;
                onStateFinish.Invoke(remainingTime);
            }
        }

        public static void ResetAllTrigger(this Animator animator)
        {
            foreach (var parameter in animator.parameters)
            {
                if(parameter.type == AnimatorControllerParameterType.Trigger)
                    animator.ResetTrigger(parameter.nameHash);
            }
        }

        public static Tweener DoLayerWeight(this Animator animator, int layerIndex, float toWeight, float duration)
        {
            return DOVirtual.Float(animator.GetLayerWeight(layerIndex), toWeight, duration, v =>
            {
                animator.SetLayerWeight(layerIndex, v);
            });
        }
        
        public static Tweener DoLayerWeight(this Animator animator, int layerIndex, float fromWeight, float toWeight, float duration)
        {
            return DOVirtual.Float(fromWeight, toWeight, duration, v =>
            {
                animator.SetLayerWeight(layerIndex, v);
            });
        }
    }
}