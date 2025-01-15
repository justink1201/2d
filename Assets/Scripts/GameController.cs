using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    public GameObject _gameOverScreen, _winScreen, _resetTextGO, _resetTextWin, _hudCanvas;
    bool _anyKeyToReset;
    public int _score;
    int _maxScore;
    public TextMeshProUGUI _scoreText, _maxScoreText, _finalScoreText, _finalScoreMaxText;
    public static GameController instance;
    
    // Start is called before the first frame update

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GameObject[] _countPickups = GameObject.FindGameObjectsWithTag("Pickup");
        _maxScore = _countPickups.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if(_anyKeyToReset && Input.anyKeyDown)
                {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        //Set score text boxes.
        _scoreText.text = _score.ToString();
        _maxScoreText.text = _maxScore.ToString();
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    public void WinScreen()
    {
        StartCoroutine(WinRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        Debug.Log("Game Over Routine Started");
        yield return new WaitForSeconds(0.9f);
        Debug.Log("Activiating Game Over Screen");
        _gameOverScreen.SetActive(true);
        _hudCanvas.SetActive(false);
        yield return new WaitForSeconds(0.85f);
        _resetTextGO.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        _anyKeyToReset = true;
    }

    IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        _winScreen.SetActive(true);
        _hudCanvas.SetActive(false);
        _finalScoreText.text = _score.ToString();
        _finalScoreMaxText.text = _maxScore.ToString();
        yield return new WaitForSeconds(1.8f);
        _resetTextWin.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        _anyKeyToReset = true;
    }
}
