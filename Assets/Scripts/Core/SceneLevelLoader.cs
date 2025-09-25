using UnityEngine;
using RobotCoder.Core;

namespace Core
{
    public class SceneLevelLoader : MonoBehaviour
    {
        [Header("Level Settings")]
        [SerializeField] private LevelData levelToLoad;
        [SerializeField] private bool loadOnStart = true;
        
        [Header("References")]
        [SerializeField] private RobotController robotController;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private LevelManager levelManager;
        
        private void Start()
        {
            if (loadOnStart)
            {
                LoadLevel();
            }
        }
        
        public void LoadLevel()
        {
            if (levelToLoad == null) return;
            
            // If we have a level manager, use it
            if (levelManager != null)
            {
                // Find the level index in the manager
                LevelData[] allLevels = levelManager.GetAllLevels();
                for (int i = 0; i < allLevels.Length; i++)
                {
                    if (allLevels[i] == levelToLoad)
                    {
                        levelManager.LoadLevel(i);
                        return;
                    }
                }
            }
            
            // Otherwise, load directly
            if (gridManager != null)
            {
                gridManager.InitializeGrid(levelToLoad);
            }
            
            if (robotController != null)
            {
                robotController.Initialize(levelToLoad);
            }
            
            Debug.Log($"Уровень загружен: {levelToLoad.levelName}");
        }
        
        public void SetLevelToLoad(LevelData level)
        {
            levelToLoad = level;
        }
    }
}