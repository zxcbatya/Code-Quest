using UnityEngine;
using System.Collections;
using RobotCoder.Core;

namespace Core
{
    public class LoopLogic : MonoBehaviour
    {
        [Header("Robot Reference")]
        [SerializeField] private RobotController robotController;
        
        [Header("Loop Settings")]
        [SerializeField] private int maxLoopIterations = 100; // Safety limit
        [SerializeField] private float loopDelay = 0.1f;
        
        private bool isLooping = false;
        private Coroutine loopCoroutine;
        
        private void Start()
        {
            if (robotController == null)
                robotController = RobotController.Instance;
        }
        
        // Execute a simple repeat loop
        public void ExecuteRepeatLoop(int count, System.Action loopAction)
        {
            if (isLooping || loopAction == null) return;
            
            isLooping = true;
            loopCoroutine = StartCoroutine(RepeatLoop(count, loopAction));
        }
        
        // Coroutine for repeat loop execution
        private IEnumerator RepeatLoop(int count, System.Action loopAction)
        {
            int iterations = 0;
            
            while (iterations < count && iterations < maxLoopIterations)
            {
                // Execute the loop action
                loopAction?.Invoke();
                
                iterations++;
                
                // Wait for a short delay to allow visualization
                if (loopDelay > 0)
                {
                    yield return new WaitForSeconds(loopDelay);
                }
                
                // Wait for robot to finish moving if needed
                if (robotController != null)
                {
                    while (robotController.IsMoving())
                    {
                        yield return null;
                    }
                }
            }
            
            isLooping = false;
            loopCoroutine = null;
        }
        
        // Execute a while loop with condition
        public void ExecuteWhileLoop(System.Func<bool> condition, System.Action loopAction)
        {
            if (isLooping || condition == null || loopAction == null) return;
            
            isLooping = true;
            loopCoroutine = StartCoroutine(WhileLoop(condition, loopAction));
        }
        
        // Coroutine for while loop execution
        private IEnumerator WhileLoop(System.Func<bool> condition, System.Action loopAction)
        {
            int iterations = 0;
            
            while (condition() && iterations < maxLoopIterations)
            {
                // Execute the loop action
                loopAction?.Invoke();
                
                iterations++;
                
                // Wait for a short delay to allow visualization
                if (loopDelay > 0)
                {
                    yield return new WaitForSeconds(loopDelay);
                }
                
                // Wait for robot to finish moving if needed
                if (robotController != null)
                {
                    while (robotController.IsMoving())
                    {
                        yield return null;
                    }
                }
            }
            
            isLooping = false;
            loopCoroutine = null;
        }
        
        // Execute a for loop with counter
        public void ExecuteForLoop(int start, int end, System.Action<int> loopAction)
        {
            if (isLooping || loopAction == null) return;
            
            isLooping = true;
            loopCoroutine = StartCoroutine(ForLoop(start, end, loopAction));
        }
        
        // Coroutine for for loop execution
        private IEnumerator ForLoop(int start, int end, System.Action<int> loopAction)
        {
            int iterations = 0;
            
            for (int i = start; i < end && iterations < maxLoopIterations; i++)
            {
                // Execute the loop action with counter
                loopAction?.Invoke(i);
                
                iterations++;
                
                // Wait for a short delay to allow visualization
                if (loopDelay > 0)
                {
                    yield return new WaitForSeconds(loopDelay);
                }
                
                // Wait for robot to finish moving if needed
                if (robotController != null)
                {
                    while (robotController.IsMoving())
                    {
                        yield return null;
                    }
                }
            }
            
            isLooping = false;
            loopCoroutine = null;
        }
        
        // Stop any currently executing loop
        public void StopLoop()
        {
            if (!isLooping) return;
            
            isLooping = false;
            
            if (loopCoroutine != null)
            {
                StopCoroutine(loopCoroutine);
                loopCoroutine = null;
            }
        }
        
        // Check if a loop is currently executing
        public bool IsLooping()
        {
            return isLooping;
        }
        
        // Set the maximum number of loop iterations (safety limit)
        public void SetMaxLoopIterations(int maxIterations)
        {
            maxLoopIterations = Mathf.Max(1, maxIterations);
        }
        
        // Set the delay between loop iterations
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