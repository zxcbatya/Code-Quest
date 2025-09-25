using Core;
using UnityEngine;
using System.Collections;

namespace RobotCoder.Core
{
    public class RobotController : MonoBehaviour
    {
        public static RobotController Instance { get; private set; }
        
        [SerializeField] private Vector2Int currentPosition = Vector2Int.zero;
        [SerializeField] private int currentDirection = 0;
        [SerializeField] private float moveSpeed = 1f;
        
        private Vector2Int startPosition;
        private int startDirection;
        private bool isMoving = false;
        private GridManager gridManager;
        private LevelData currentLevel;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                startPosition = currentPosition;
                startDirection = currentDirection;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            gridManager = GridManager.Instance;
        }
        
        public void Initialize(LevelData levelData)
        {
            currentLevel = levelData;
            if (levelData != null)
            {
                startPosition = levelData.startPosition;
                startDirection = levelData.startDirection;
                currentPosition = startPosition;
                currentDirection = startDirection;
                
                // Update visual position
                transform.position = GridToWorldPosition(currentPosition);
                transform.rotation = DirectionToRotation(currentDirection);
            }
        }
        
        public bool MoveForward()
        {
            if (isMoving) return false;
            
            Vector2Int targetPosition = GetForwardPosition();
            
            if (IsValidPosition(targetPosition))
            {
                currentPosition = targetPosition;
                StartCoroutine(AnimateMovement());
                return true;
            }
            
            return false;
        }
        
        public bool TurnLeft()
        {
            if (isMoving) return false;
            
            currentDirection = (currentDirection + 3) % 4;
            StartCoroutine(AnimateRotation(-90f));
            return true;
        }
        
        public bool TurnRight()
        {
            if (isMoving) return false;
            
            currentDirection = (currentDirection + 1) % 4;
            StartCoroutine(AnimateRotation(90f));
            return true;
        }
        
        public bool Jump()
        {
            if (isMoving) return false;
            
            Vector2Int targetPosition = GetForwardPosition();
            
            if (IsValidPosition(targetPosition))
            {
                currentPosition = targetPosition;
                StartCoroutine(AnimateJump());
                return true;
            }
            
            return false;
        }
        
        public bool Interact()
        {
            if (isMoving) return false;
            
            Debug.Log($"Робот взаимодействует на позиции {currentPosition}");
            StartCoroutine(AnimateInteraction());
            return true;
        }
        
        public void ResetToStart()
        {
            // Останавливаем все корутины перед сбросом
            StopAllCoroutines();
            isMoving = false;
            
            currentPosition = startPosition;
            currentDirection = startDirection;
            
            transform.position = GridToWorldPosition(currentPosition);
            transform.rotation = DirectionToRotation(currentDirection);
        }
        
        public bool IsPathAhead()
        {
            Vector2Int targetPosition = GetForwardPosition();
            return IsValidPosition(targetPosition);
        }
        
        public bool IsWallAhead()
        {
            return !IsPathAhead();
        }
        
        public bool IsOnGoal()
        {
            if (currentLevel != null)
            {
                return currentLevel.IsGoalPosition(currentPosition);
            }
            return false;
        }
        
        public bool IsItemNearby()
        {
            // Check adjacent positions for items
            for (int i = 0; i < 4; i++)
            {
                Vector2Int direction = DirectionToVector(i);
                Vector2Int checkPosition = currentPosition + direction;
                
                if (gridManager != null)
                {
                    LevelData.TileType tileType = gridManager.GetTileType(checkPosition.x, checkPosition.y);
                    if (tileType == LevelData.TileType.Button || 
                        tileType == LevelData.TileType.Key)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        private Vector2Int GetForwardPosition()
        {
            Vector2Int direction = DirectionToVector(currentDirection);
            return currentPosition + direction;
        }
        
        private Vector2Int DirectionToVector(int dir)
        {
            switch (dir)
            {
                case 0: return Vector2Int.up;
                case 1: return Vector2Int.right;
                case 2: return Vector2Int.down;
                case 3: return Vector2Int.left;
                default: return Vector2Int.up;
            }
        }
        
        private bool IsValidPosition(Vector2Int position)
        {
            if (gridManager != null)
            {
                return gridManager.IsPositionValid(position.x, position.y);
            }
            else
            {
                // Fallback to simple grid bounds checking
                return position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8;
            }
        }
        
        private Vector3 GridToWorldPosition(Vector2Int gridPos)
        {
            if (gridManager != null)
            {
                return gridManager.GridToWorldPosition(gridPos);
            }
            else
            {
                // Fallback to simple positioning
                return new Vector3(gridPos.x, 0, gridPos.y);
            }
        }
        
        private Quaternion DirectionToRotation(int dir)
        {
            return Quaternion.Euler(0, dir * 90f, 0);
        }
        
        private System.Collections.IEnumerator AnimateMovement()
        {
            isMoving = true;
            
            Vector3 startPos = transform.position;
            Vector3 targetPos = GridToWorldPosition(currentPosition);
            float duration = 1f / moveSpeed;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                transform.position = Vector3.Lerp(startPos, targetPos, progress);
                yield return null;
            }
            
            transform.position = targetPos;
            isMoving = false;
            
            AudioManager.Instance?.PlaySound("robot_move");
        }
        
        private System.Collections.IEnumerator AnimateRotation(float angle)
        {
            isMoving = true;
            
            Quaternion startRot = transform.rotation;
            Quaternion targetRot = startRot * Quaternion.Euler(0, angle, 0);
            float duration = 0.5f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                transform.rotation = Quaternion.Lerp(startRot, targetRot, progress);
                yield return null;
            }
            
            transform.rotation = targetRot;
            isMoving = false;
            
            AudioManager.Instance?.PlaySound("robot_turn");
        }
        
        private System.Collections.IEnumerator AnimateJump()
        {
            isMoving = true;
            
            Vector3 startPos = transform.position;
            Vector3 targetPos = GridToWorldPosition(currentPosition);
            Vector3 midPos = (startPos + targetPos) / 2 + Vector3.up * 2f;
            
            float duration = 1f / moveSpeed;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                Vector3 pos1 = Vector3.Lerp(startPos, midPos, progress);
                Vector3 pos2 = Vector3.Lerp(midPos, targetPos, progress);
                transform.position = Vector3.Lerp(pos1, pos2, progress);
                
                yield return null;
            }
            
            transform.position = targetPos;
            isMoving = false;
            
            AudioManager.Instance?.PlaySound("robot_jump");
        }
        
        private System.Collections.IEnumerator AnimateInteraction()
        {
            isMoving = true;
            
            Vector3 originalScale = transform.localScale;
            Vector3 biggerScale = originalScale * 1.2f;
            
            float duration = 0.2f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                transform.localScale = Vector3.Lerp(originalScale, biggerScale, progress);
                yield return null;
            }
            
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                transform.localScale = Vector3.Lerp(biggerScale, originalScale, progress);
                yield return null;
            }
            
            transform.localScale = originalScale;
            isMoving = false;
            
            AudioManager.Instance?.PlaySound("robot_interact");
        }
        
        public Vector2Int GetCurrentPosition()
        {
            return currentPosition;
        }
        
        public int GetCurrentDirection()
        {
            return currentDirection;
        }
        
        public bool IsMoving()
        {
            return isMoving;
        }
        
        private void OnDestroy()
        {
            // Останавливаем все корутины при уничтожении объекта
            StopAllCoroutines();
        }
    }
}