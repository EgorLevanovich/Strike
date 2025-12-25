using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public sealed class GDPRService : MonoBehaviour
    {
        [SerializeField] private GameObject _window;
        [SerializeField] private Button _termsButton;
        [SerializeField] private Button _privacyButton;
        [SerializeField] private Button _applyButton;

        [Space] 
        [SerializeField] private string _termsUrl;
        [SerializeField] private string _privacyUrl;

        private const string SaveHash = "GDPR_Consent";
        
        public async UniTask ShowAsync()
        {
            var isDone = false;
            if (PlayerPrefs.HasKey(SaveHash))
            {
                HideWindow();
                return;
            }

            _termsButton.onClick.AddListener(OpenTerms);
            _privacyButton.onClick.AddListener(OpenPrivacy);
            _applyButton.onClick.AddListener(OnApplied);
            _window.SetActive(true);

            await UniTask.WaitUntil(() => isDone);

            void OnApplied()
            {
                HideWindow();
                ApplyConsent();
                PlayerPrefs.SetInt(SaveHash, 1);
                isDone = true;
            }

            void HideWindow()
            {
                _window.SetActive(false);
            }
        }
        
        private void ApplyConsent()
        {
            var gdprMetaData = new MetaData("gdpr");
            gdprMetaData.Set("consent", "true");
            Advertisement.SetMetaData(gdprMetaData);
        }

        private void OpenTerms()
        {
            OpenUrl(_termsUrl);
        }

        private void OpenPrivacy()
        {
            OpenUrl(_privacyUrl);
        }

        private void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }
    }
}