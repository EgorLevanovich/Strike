using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private Rigidbody2D[] _players;
    private GameObject[] characters;
    private int index;
    private Rigidbody2D _player;

    private void Start()
    {
        int index = PlayerPrefs.GetInt(PlayerSelect.SkinKey);
        for (int i = 0; i < _players.Length; i++)
        {
            if (i == index)
            {
                _player = _players[i];
                _player.gameObject.SetActive(true);
            }
            else
            {
                _players[i].gameObject.SetActive(false);
            }
        }


    }

    public void SelectLeft()
    {
        characters[index].SetActive(false);
        index--;
        if (index < 0)
        {
            index = characters.Length - 1;
        }
        characters[index].SetActive(true);
    }

    public void SelectRight()
    {
        characters[index].SetActive(false);
        index++;
        if (index == characters.Length)
        {
            index = 0;
        }
        characters[index].SetActive(true);
    }

    public void StartScene()
    {
        PlayerPrefs.SetInt("BallSelect", index);

        SceneManager.LoadScene("Game");
    }
}
