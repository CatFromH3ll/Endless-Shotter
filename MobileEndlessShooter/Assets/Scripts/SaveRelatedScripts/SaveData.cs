    using UnityEngine;
    [System.Serializable]
    public class SaveData
    {
        [Header("Player State")]
        public float maxHealth;
        public float currentHealth;
        public int score;
        public int boatModelIndex;
        public Vector3 position;

        [Header("World State")]
        public int selectedDifficulty;    
        public int currentWaveIndex;      
        public float stageMult;
        
        [Header("Timer State")]
        public float savedTime;
    }
