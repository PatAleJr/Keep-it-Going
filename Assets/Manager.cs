using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Manager : MonoBehaviour
{
    public static Manager manager;

    public static int highscore = 0;
    public GameObject endGame;

    public float canvasDelay = 1f;
    public float inputDelay = 2f;
    public float resetGameDelay = 0.3f;

    public bool gameSet = true;
    public bool canReset = false;

    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI reasonText;
    [SerializeField]
    private TextMeshProUGUI highscoreText;

    [SerializeField]
    SectionMaker sectionMaker;

    [SerializeField]
    SoundManager soundManager;

    private void Awake()
    {
        if (Manager.manager == null)
        {
            Manager.manager = this;
        }
        else {
            if (Manager.manager != this)
            {
                Destroy(Manager.manager);
                Manager.manager = this;
            }
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (canReset && !gameSet)
            {
                resetGame();
            }
        }
    }

    void resetGame()
    {
        canReset = false;
        StartCoroutine(resetSections());
        endGame.GetComponent<Animator>().SetTrigger("Reset");
    }

    public void lose(int score, string reason)
    {
        if (score > highscore)
            highscore = score;

        soundManager.Lost();

        StartCoroutine(activateLossCanvas());
        StartCoroutine(activateInput());
        canReset = false;
        gameSet = false;

        scoreText.text = score.ToString();
        reasonText.text = reason;
        highscoreText.text = "Highscore: " + highscore;
    }

    IEnumerator activateLossCanvas()
    {
        yield return new WaitForSeconds(canvasDelay);
        endGame.GetComponent<Animator>().SetTrigger("Lose");
    }
    IEnumerator activateInput()
    {
        yield return new WaitForSeconds(inputDelay);
        canReset = true;
    }
    IEnumerator resetSections()
    {
        yield return new WaitForSeconds(resetGameDelay);
        sectionMaker.setGame();
        gameSet = true;
        soundManager.resetGame();
    }
}
