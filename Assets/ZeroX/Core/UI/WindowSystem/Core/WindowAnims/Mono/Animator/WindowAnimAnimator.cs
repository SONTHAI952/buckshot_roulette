using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    public class WindowAnimAnimator : WindowAnimMono
    {
        public Animator animator;
        public string stateNameOpen = "Open";
            
        [Tooltip("Tua ngược animation open để Close")]
        public bool mirrorOpenToClose = true;

        [Tooltip("Nếu mirror open to close = true thì sẽ không dùng đến điều này")]
        public string stateNameClose = "Close";
        
        
        
        private Coroutine runnerOpen;
        private Coroutine animationRunnerOpen;

        private Coroutine runnerClose;
        private Coroutine animationRunnerClose;

        private Action currentActionOnFinishedExecuteClose;
        private WindowParameters currentParameters;
        private Window currentWindow;
        
        private float lastNormalized = 0f;
        
        protected void Awake()
        {
            animator.speed = 0f;
        }

        protected void OnDisable()
        {
            if (runnerClose != null || animationRunnerClose != null)
            {
                runnerClose = null;
                animationRunnerClose = null;
                CloseImmediate(currentWindow, currentParameters);
                currentActionOnFinishedExecuteClose.Invoke();
            }
        }

        private void Reset()
        {
            animator = GetComponent<Animator>();
        }

        void StopRunnersOpen()
        {
            if (runnerOpen != null)
            {
                StopCoroutine(runnerOpen);
                runnerOpen = null;
            }

            if (animationRunnerOpen != null)
            {
                StopCoroutine(animationRunnerOpen);
                animationRunnerOpen = null;
            }
        }

        void StopRunnersClose()
        {
            if (runnerClose != null)
            {
                StopCoroutine(runnerClose);
                runnerClose = null;
            }

            if (animationRunnerClose != null)
            {
                StopCoroutine(animationRunnerClose);
                animationRunnerClose = null;
            }
        }

        public override void Open(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            currentWindow = window;
            currentParameters = parameters;
            
            animator.gameObject.SetActive(true);
            animator.enabled = true;
            
            StopRunnersOpen();
            StopRunnersClose();

            runnerOpen = StartCoroutine(RunnerOpen(duration, ignoreTimeScale, onFinished));
        }

        public override void OpenImmediate(Window window, WindowParameters parameters)
        {
            currentWindow = window;
            currentParameters = parameters;
            
            lastNormalized = 1f;
            animator.Play(stateNameOpen, 0, 1f);
            animator.Update(0);
        }

        public override void Close(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            currentWindow = window;
            currentParameters = parameters;
            
            animator.gameObject.SetActive(true);
            animator.enabled = true;

            StopRunnersClose();
            StopRunnersOpen();

            currentActionOnFinishedExecuteClose = onFinished;
            
            runnerClose = StartCoroutine(RunnerClose(duration, ignoreTimeScale, onFinished));
        }

        public override void CloseImmediate(Window window, WindowParameters parameters)
        {
            currentWindow = window;
            currentParameters = parameters;
            
            if (mirrorOpenToClose)
            {
                lastNormalized = 0f;
                animator.Play(stateNameOpen, 0, 0f);
            }
            else
            {
                lastNormalized = 1f;
                animator.Play(stateNameClose, 0, 1f);
            }

            animator.Update(0);
        }

        IEnumerator DoAnimation(string stateName, float endNormalizedTime, float duration, bool ignoreTimeScale, bool reverseCurrentNormalizedTime)
        {
            int stateNameHash = Animator.StringToHash(stateName);
            Animator animator = this.animator;
            var info = animator.GetCurrentAnimatorStateInfo(0);
            //var info = animator.GetCurrentAnimatorStateInfo(0);
            //float normalizedTime = info.normalizedTime;
            float normalizedTime = lastNormalized;
            if (reverseCurrentNormalizedTime)
                normalizedTime = 1 - normalizedTime;
            float time = normalizedTime * duration;
            float endTime = endNormalizedTime * duration;

            if (endTime == time)
                yield break;

            if (endTime > time) //tăng
            {
                if (ignoreTimeScale)
                {
                    for (; time < endTime; time += Time.unscaledDeltaTime)
                    {
                        lastNormalized = time / duration;
                        animator.Play(stateNameHash, 0, lastNormalized);
                        animator.Update(0);
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    for (; time < endTime; time += Time.deltaTime)
                    {
                        lastNormalized = time / duration;
                        animator.Play(stateNameHash, 0, lastNormalized);
                        animator.Update(0);
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
            else //giảm
            {
                if (ignoreTimeScale)
                {
                    for (; time > endTime; time -= Time.unscaledDeltaTime)
                    {
                        lastNormalized = time / duration;
                        animator.Play(stateNameHash, 0, lastNormalized);
                        animator.Update(0);
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    for (; time > endTime; time -= Time.deltaTime)
                    {
                        lastNormalized = time / duration;
                        animator.Play(stateNameHash, 0, lastNormalized);
                        animator.Update(0);
                        yield return new WaitForEndOfFrame();
                    }
                }
            }

            lastNormalized = endNormalizedTime;
            animator.Play(stateNameHash, 0, endNormalizedTime);
            animator.Update(0);
        }
        
        IEnumerator RunnerOpen(float duration, bool ignoreTimeScale, Action onFinished)
        {
            if(mirrorOpenToClose)
                animationRunnerOpen = StartCoroutine(DoAnimation(stateNameOpen, 1f, duration, ignoreTimeScale, false));
            else
                animationRunnerOpen = StartCoroutine(DoAnimation(stateNameOpen, 1f, duration, ignoreTimeScale, true));

            yield return animationRunnerOpen;
            
            onFinished.Invoke();
            StopRunnersOpen();
        }
        
        IEnumerator RunnerClose(float duration, bool ignoreTimeScale, Action onFinished)
        {
            if (mirrorOpenToClose)
                animationRunnerClose = StartCoroutine(DoAnimation(stateNameOpen, 0f, duration, ignoreTimeScale, false));
            else
                animationRunnerClose = StartCoroutine(DoAnimation(stateNameClose, 1f, duration, ignoreTimeScale, true));

            yield return animationRunnerClose;

            currentActionOnFinishedExecuteClose = null;

            runnerClose = null;
            animationRunnerClose = null;
            
            onFinished.Invoke();
        }
    }
}