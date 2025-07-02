using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    [Serializable]
    public class BallSaveData 
    {
        public Vector3 position;
        public Vector3 velocity;
        public bool isPlayer;
    }

    [Serializable]
    public class GameSaveData 
    {
        public List<BallSaveData> balls = new();
        public int scorePlayer;
        public int scoreAI;
    }

}