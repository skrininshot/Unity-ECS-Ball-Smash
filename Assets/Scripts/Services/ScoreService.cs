namespace Services
{
    public static class ScoreService 
    {
        public static int Player = 0;
        public static int AI = 0;

        public static void Reset() 
        {
            Player = 0;
            AI = 0;
        }
    }
}