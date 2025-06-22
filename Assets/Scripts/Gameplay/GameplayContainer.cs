using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    [System.Serializable]
    internal class GameplayContainer
    {
        public event System.Action Changed;

        [SerializeField] private int _levelCount;
        [SerializeField] private string _levelName = "default";
        [SerializeField] private string _platformName = "default";
        [SerializeField] private string _ballName = "default";

        public static GameplayContainer Instance { get; private set; }

        public GameplayContainer()
        {
            Instance = this;
        }

        public int LevelCount
        {
            get => _levelCount;
            set
            {
                _levelCount = value;
                OnChanged();
            }
        }

        public string LevelName
        {
            get => _levelName;
            set
            {
                _levelName = value;
                OnChanged();
            }
        }

        public string PlatformName
        {
            get => _platformName;
            set
            {
                _platformName = value;
                OnChanged();
            }
        }

        public string BallName
        {
            get => _ballName;
            set
            {
                _ballName = value;
                OnChanged();
            }
        }


        private void OnChanged()
        {
            Changed?.Invoke();
        }
    }
}
