using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Config/Game Config")]
    public class GameConfig : ScriptableObject 
    {
        [Header("Gameplay")]
        public float destroyHeight = -10f;
        public int aiBallsCount = 3;
        public int playerBallsCount = 3;
        public float pauseBetweenTurns = 1;
        
        [Header("Ball spawn")]
        public float spawnRadius = 4f;
        public float spawnMinDistance = 1.2f;
        
        [Header("Player")]
        public Material playerMaterial;
        public float playerForce = 10f;
        
        [Header("AI")]
        public Material aiMaterial;
        public float aiForce = 10f;
        public float aiInaccuracyMultiplier = 0.5f;
    }
}