using UnityEngine;
using System.Collections;
using RobotCoder.Core;

namespace Core
{
    public class LoopLogic : MonoBehaviour
    {
        [Header("Robot Reference")] [SerializeField]
        private RobotController robotController;

        [Header("Loop Settings")] [SerializeField]
        private int maxLoopIterations = 100; 

        [SerializeField] private float loopDelay = 0.1f;

        private bool _isLooping = false;
        private Coroutine _loopCoroutine;

        private void Start()
        {
            if (robotController == null)
                robotController = RobotController.Instance;
        }

        public void ExecuteRepeatLoop(int count, System.Action loopAction)
        {
            if (_isLooping || loopAction == null) return;

            _isLooping = true;
            _loopCoroutine = StartCoroutine(RepeatLoop(count, loopAction));
        }

        private IEnumerator RepeatLoop(int count, System.Action loopAction)
        {
            int iterations = 0;

            while (iterations < count && iterations < maxLoopIterations)
            {
                loopAction?.Invoke();

                iterations++;

                if (loopDelay > 0)
                {
                    yield return new WaitForSeconds(loopDelay);
                }

                if (robotController != null)
                {
                    while (robotController.IsMoving())
                    {
                        yield return null;
                    }
                }
            }

            _isLooping = false;
            _loopCoroutine = null;
        }

        public void ExecuteWhileLoop(System.Func<bool> condition, System.Action loopAction)
        {
            if (_isLooping || condition == null || loopAction == null) return;

            _isLooping = true;
            _loopCoroutine = StartCoroutine(WhileLoop(condition, loopAction));
        }

        private IEnumerator WhileLoop(System.Func<bool> condition, System.Action loopAction)
        {
            int iterations = 0;

            while (condition() && iterations < maxLoopIterations)
            {
                iterations++;

                if (loopDelay > 0)
                {
                    yield return new WaitForSeconds(loopDelay);
                }

                if (robotController != null)
                {
                    while (robotController.IsMoving())
                    {
                        yield return null;
                    }
                }
            }

            _isLooping = false;
            _loopCoroutine = null;
        }

        public void ExecuteForLoop(int start, int end, System.Action<int> loopAction)
        {
            if (_isLooping || loopAction == null) return;

            _isLooping = true;
            _loopCoroutine = StartCoroutine(ForLoop(start, end, loopAction));
        }

        private IEnumerator ForLoop(int start, int end, System.Action<int> loopAction)
        {
            int iterations = 0;

            for (int i = start; i < end && iterations < maxLoopIterations; i++)
            {
                loopAction?.Invoke(i);

                iterations++;

                if (loopDelay > 0)
                {
                    yield return new WaitForSeconds(loopDelay);
                }

                if (robotController != null)
                {
                    while (robotController.IsMoving())
                    {
                        yield return null;
                    }
                }
            }

            _isLooping = false;
            _loopCoroutine = null;
        }

        private void StopLoop()
        {
            if (!_isLooping) return;

            _isLooping = false;

            if (_loopCoroutine != null)
            {
                StopCoroutine(_loopCoroutine);
                _loopCoroutine = null;
            }
        }

        public bool IsLooping()
        {
            return _isLooping;
        }

        public void SetMaxLoopIterations(int maxIterations)
        {
            maxLoopIterations = Mathf.Max(1, maxIterations);
        }

        public void SetLoopDelay(float delay)
        {
            loopDelay = Mathf.Max(0, delay);
        }

        private void OnDestroy()
        {
            StopLoop();
        }
    }
}