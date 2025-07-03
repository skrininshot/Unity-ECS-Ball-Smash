namespace Components
{
    public struct TurnComponent 
    {
        public TurnParticipant Participant;
        public TurnState State;
        public float PauseTime;
    }

    public enum TurnParticipant
    {
        Player,
        AI,
    }

    public enum TurnState
    {
        Move,
        Pause
    }
}