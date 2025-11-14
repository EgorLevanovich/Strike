namespace MoreMountains.Feedbacks
{
    public class GlobalMMFPlayer : MMF_Player
    {
        public static GlobalMMFPlayer Instance { get; private set; }

        protected override void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}