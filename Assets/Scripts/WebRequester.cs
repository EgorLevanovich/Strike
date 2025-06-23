using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public class WebRequester
    {
        public async UniTask<string> GetDataAsync(Color color)
        {
            string url = $"https://www.thecolorapi.com/id?rgb=rgb({color.r},{color.g},{color.b})";
            string result = "default";
            
            using UnityWebRequest request = UnityWebRequest.Get(url);
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log("Ответ от API:\n" + json);
                
                ColorApiResponse response = JsonUtility.FromJson<ColorApiResponse>(json);
                result = response.Name.Value;
            }
            else
            {
                Debug.LogError($"{nameof(GetDataAsync)} {request.error}");
            }

            return result;
        }
    }

    [System.Serializable]
    public struct ColorName
    {
        public string Value;
    }

    [System.Serializable]
    public struct ColorApiResponse
    {
        public ColorName Name;
    }
}