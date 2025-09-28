using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RobotCoder.Core;

namespace Core
{
    public class AdvancedRobotBehaviors : MonoBehaviour
    {
        [Header("Robot Reference")]
        [SerializeField] private RobotController robotController;
        
        [Header("Behavior Settings")]
        [SerializeField] private float scanInterval = 1.0f;
        [SerializeField] private float reactionTime = 0.5f;
        
        private Coroutine _scanCoroutine;
        private bool _isScanning;
        
        private void Start()
        {
            if (robotController == null)
                robotController = RobotController.Instance;
        }
        
        public void StartScanning()
        {
            if (_isScanning) return;
            
            _isScanning = true;
            _scanCoroutine = StartCoroutine(ScanEnvironment());
        }
        
        public void StopScanning()
        {
            if (!_isScanning) return;
            
            _isScanning = false;
            if (_scanCoroutine != null)
            {
                StopCoroutine(_scanCoroutine);
                _scanCoroutine = null;
            }
        }
        
        private IEnumerator ScanEnvironment()
        {
            while (_isScanning)
            {
                ScanSurroundings();
                yield return new WaitForSeconds(scanInterval);
            }
        }
        
        private void ScanSurroundings()
        {
            
            Vector2Int currentPosition = robotController.GetCurrentPosition();
            int currentDirection = robotController.GetCurrentDirection();
            
            for (int i = 0; i < 4; i++)
            {
                Vector2Int directionVector = DirectionToVector(i);
                Vector2Int scanPosition = currentPosition + directionVector;
                
                Debug.Log($"Сканирование позиции {scanPosition} в направлении {i}");
            }
        }
        
        private Vector2Int DirectionToVector(int dir)
        {
            switch (dir)
            {
                case 0: return Vector2Int.up;    // North
                case 1: return Vector2Int.right; // East
                case 2: return Vector2Int.down;  // South
                case 3: return Vector2Int.left;  // West
                default: return Vector2Int.zero;
            }
        }
        
        private void OnDestroy()
        {
            StopScanning();
        }
    }
}