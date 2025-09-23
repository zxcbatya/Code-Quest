using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private int currentLevel = 1;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public int GetCurrentLevel()
        {
            return currentLevel;
        }

        public void SetCurrentLevel(int level)
        {
            currentLevel = Mathf.Max(1, level);
        }
    }
}
