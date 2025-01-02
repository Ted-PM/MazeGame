//using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //[SerializeField]
    //private GameObject _deathScreen;
    //[SerializeField]
    //private Image _blackScreen;
    //bool gameStarted = false;

    private void Awake()
    {
        Instance = this;


        //DontDestroyOnLoad(gameStarted);
        //SceneManager.LoadScene("StartMenu");
    }

    public void Start()
    {
        if (!PlayerPrefs.HasKey("sensitivity"))
        {
            PlayerPrefs.SetFloat("sensitivity", 2.5f);
        }
        //_deathScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    public void WelcomeMenu()
    {
        PlayerPrefs.SetFloat("sensitivity", 1f);
        Cursor.visible = true;
        SceneManager.LoadScene("StartMenu");
    }

    public void StartGame()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("MainGame");
    }

    //public void BeginGame()
    //{

    //}

    public void PlayerDead()
    {
        Cursor.visible = true;
        SceneManager.LoadScene("DeadMenu");
        //_deathScreen.SetActive(true);
        //Fade(false);
    }

    public void PlayerWin()
    {
        Cursor.visible = true;
        SceneManager.LoadScene("WinScene");
    }

    public void StartLobby()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("MainLobby");
    }

    //private IEnumerator Fade(bool fadeOut)
    //{
    //    //_deathScreen.GetComponentInChildren<GameObject>().name = "DeathPanel";
    //    //Image blackScreen = _deathScreen.GetComponentInChildren<Image>();

    //    if (fadeOut)
    //    {
    //        _blackScreen.color = new Color(1, 1, 1, 1);
    //        // loop over 1 second backwards
    //        for (float i = 1; i >= 0; i -= Time.deltaTime)
    //        {
    //            // set color with i as alpha
    //            _blackScreen.color = new Color(1, 1, 1, i);
    //            yield return null;
    //        }
    //    }
    //    // fade from transparent to opaque
    //    else
    //    {
    //        _blackScreen.color = new Color(1, 1, 1, 0);
    //        // loop over 1 second
    //        for (float i = 0; i <= 1; i += Time.deltaTime)
    //        {
    //            // set color with i as alpha
    //            _blackScreen.color = new Color(1, 1, 1, i);
    //            yield return null;
    //        }
    //    }
    //    // loop over 1 second backwards
    //    //for (float i = 1; i >= 0; i -= Time.deltaTime)
    //    //{
    //    //    // set color with i as alpha
    //    //    _blackScreen.color = new Color(1, 1, 1, i);
    //    //    yield return null;
    //    //}
    //    //while (blackScreen.)
    //    //{
    //    //yield return null;
    //    //blackScreen.ChangeAl
    //}
    

    //public void RestartGame()
    //{

    //}
}
