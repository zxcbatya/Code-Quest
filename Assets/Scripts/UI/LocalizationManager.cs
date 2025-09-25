using System.Collections.Generic;
using UnityEngine;

namespace RobotCoder.UI
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        [System.Serializable]
        public class LocalizationData
        {
            public string key;
            public string russian;
            public string english;
        }

        [Header("Localization Settings")] [SerializeField]
        private List<LocalizationData> localizationTexts = new List<LocalizationData>();

        private Dictionary<string, LocalizationData> localizationDict;
        private string currentLanguage = "RU";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeLocalization();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeLocalization()
        {
            localizationDict = new Dictionary<string, LocalizationData>();

            AddLocalization("PLAY", "ИГРАТЬ", "PLAY");
            AddLocalization("LEVELS", "УРОВНИ", "LEVELS");
            AddLocalization("SETTINGS", "НАСТРОЙКИ", "SETTINGS");
            AddLocalization("QUIT", "ВЫХОД", "QUIT");
            AddLocalization("BACK", "НАЗАД", "BACK");
            AddLocalization("VOLUME", "ГРОМКОСТЬ", "VOLUME");
            AddLocalization("LANGUAGE", "ЯЗЫК", "LANGUAGE");
            AddLocalization("LEVEL", "УРОВЕНЬ", "LEVEL");
            AddLocalization("START", "СТАРТ", "START");
            AddLocalization("RESET", "СБРОС", "RESET");
            AddLocalization("COMPLETE", "ЗАВЕРШЕНО", "COMPLETE");
            AddLocalization("FAILED", "ПРОВАЛ", "FAILED");
            AddLocalization("MOVE_FORWARD", "ВПЕРЕД", "MOVE FORWARD");
            AddLocalization("TURN_LEFT", "НАЛЕВО", "TURN LEFT");
            AddLocalization("TURN_RIGHT", "НАПРАВО", "TURN RIGHT");
            AddLocalization("JUMP", "ПРЫЖОК", "JUMP");
            AddLocalization("INTERACT", "ДЕЙСТВИЕ", "INTERACT");
            AddLocalization("REPEAT", "ПОВТОР", "REPEAT");
            AddLocalization("IF", "ЕСЛИ", "IF");
            AddLocalization("ELSE", "ИНАЧЕ", "ELSE");

            foreach (var item in localizationTexts)
            {
                if (!localizationDict.ContainsKey(item.key))
                {
                    localizationDict.Add(item.key, item);
                }
            }
        }

        private void AddLocalization(string key, string russian, string english)
        {
            var data = new LocalizationData
            {
                key = key,
                russian = russian,
                english = english
            };

            if (!localizationDict.ContainsKey(key))
            {
                localizationDict.Add(key, data);
            }
        }

        public void SetLanguage(string language)
        {
            currentLanguage = language;
            UpdateAllTexts();
        }

        public string GetText(string key)
        {
            if (localizationDict.TryGetValue(key, out LocalizationData data))
            {
                return currentLanguage == "RU" ? data.russian : data.english;
            }

            return key; 
        }

        private void UpdateAllTexts()
        {
            LocalizedText[] localizedTexts = FindObjectsByType<LocalizedText>(FindObjectsSortMode.None);
            foreach (var localizedText in localizedTexts)
            {
                localizedText.UpdateText();
            }
        }
    }
}