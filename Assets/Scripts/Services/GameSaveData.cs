using System;
using System.Collections.Generic;
using Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace Services
{
    [Serializable]
    public class BallSaveData 
    {
        public Vector3 position;
        public Vector3 velocity;
        public TurnParticipant owner;
    }

    [Serializable]
    public class GameSaveData 
    {
        public List<BallSaveData> balls = new();
        public int scorePlayer;
        public int scoreAI;
        
        public TurnState turnState;
        public TurnParticipant turnParticipant;
        [FormerlySerializedAs("pauseTime")] public float turnPauseTime;
    }
}