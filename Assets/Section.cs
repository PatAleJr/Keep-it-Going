using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Section : MonoBehaviour
{
    public Animator[] columns;
    private Animator animator;

    public bool isWaving = false;
    public bool waved = false;
    public int columnIndex = 0;
    public bool tooLate = false;

    private float timer;
    public float columnDelay = 0.4f;
    public float acceptanceTime = 3;

    public string[] possibleKeys;
    public int newKeysScore = 8;
    public string myButton;
    [SerializeField]
    private TextMeshProUGUI myButtonText = null;
    [SerializeField]
    private TextMeshProUGUI myScoreText = null;
    public int myScoreNum;

    public bool isMyTurn = false;
    public SectionMaker sectionMaker;

    private void Start()
    {
        myButton = possibleKeys[Random.Range(0, 3)];
        myButtonText.text = myButton;
        animator = GetComponent<Animator>();

        if (myScoreNum > newKeysScore)
        {
            myButton = possibleKeys[Random.Range(0, possibleKeys.Length)];
            myButtonText.text = myButton;
        }

        foreach (Animator animator in columns)  //Clap
        {
            animator.SetBool("Clap", true);
            animator.SetInteger("ClapIndex", Random.Range(0, 2));
        }//Clap
    }

    public void setMyScore(int myScore)
    {
        myScoreNum = myScore;
        myScoreText.text = myScoreNum.ToString();

        if (myScore > newKeysScore)
        {
            myButton = possibleKeys[Random.Range(0, possibleKeys.Length)];
            myButtonText.text = myButton;
        }
    }

    void Update()
    {
        if (!Manager.manager.gameSet)
            return;

        if (isMyTurn && !isWaving && !waved)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(myButton))
                { 
                    sectionMaker.tapped(this.gameObject);
                }
                else
                {
                    if (myScoreNum == 1)
                        return;
                    //Finds if is a button you can press
                    bool lost = false;
                    foreach (string key in possibleKeys)
                    {
                        if (Input.GetKeyDown(key))
                        {
                            lost = true;
                            break;
                        }
                    }
                    if (lost)
                    {
                        sectionMaker.lose("You tapped the wrong button");
                    }
                }
            }
        }

        if (isWaving)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else {

                //If did last column
                if (columnIndex == columns.Length - 1)
                {
                    isWaving = false;
                    waved = true;
                    StartCoroutine(tooFar());
                }

                columns[columnIndex].SetTrigger("Wave");
                timer = columnDelay;
                columnIndex++;
            }
        }
    }

    IEnumerator tooFar()
    {
        yield return new WaitForSeconds(columnDelay*acceptanceTime);
        tooLate = true;
    }

    public void stopClapping()
    {
        foreach (Animator animator in columns)  //the crowds
        {
            animator.SetBool("Clap", false);
        }
    }

    public void lose()
    {
        animator.SetTrigger("Fail");    //the button
        foreach (Animator animator in columns)  //the crowds
        {
            animator.SetBool("Clap", false);

            int failAnim = Random.Range(0, 4);

            switch (failAnim)
            {
                case 0:
                    break;
                case 1:
                    animator.SetTrigger("Fail01");
                    break;
                case 2:
                    animator.SetTrigger("Fail02");
                    break;
                case 3:
                    animator.SetTrigger("Fail03");
                    break;
            }
        }
    }

    public void wave()
    {
        animator.SetTrigger("Tap");
        columnIndex = 0;
        timer = 0;
        isWaving = true;
        isMyTurn = false;
    }
}
