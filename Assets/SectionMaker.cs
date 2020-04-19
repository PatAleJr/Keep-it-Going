using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionMaker : MonoBehaviour
{
    [SerializeField]
    private GameObject sectionPrefab = null;

    public List<GameObject> sections;   //Holds existing sections (4)
    public Transform spawnPoint = null;
    public Vector3 spaceBetweenSections;

    public Transform[] firstSectionPoints;

    public float destroyX = -10f;
    public float startSpeed = 1.5f;
    private float currentSpeed;
    public float columnDelay = 1f;
    public int score = 0;
    public bool isPlaying;
    public bool lost;

    public string firstKey = "a";
    public int sectionScore = 1;

    public float xTapRange = 1f;
    public GameObject currentSection;
    private GameObject previousSection = null;

    private float incrementTimer = 0f;
    public float incrementFrequency = 1f;
    public float incrementBy = 0.2f;

    [SerializeField]
    private SoundManager soundManager;

    [SerializeField]
    private float spaceBetweenColumns;

    //Mouse
    public float inputRange = 80f;
    public float minSpeed = 0.8f;
    public float maxSpeed = 3;
    public float minWaveX = -5f;
    public float maxWaveX = 4f;

    //Tutorial
    [SerializeField]
    private Animator tutorial;


    void Start()
    {
        setGame();
    }

    public void setGame()
    {
        //Resets vars
        score = 0;
        sectionScore = 1;
        lost = false;
        isPlaying = false;
        currentSpeed = startSpeed;
        columnDelay = spaceBetweenColumns / currentSpeed;
        incrementTimer = incrementFrequency;

        //Destroy all existing sections
        foreach (GameObject section in sections)
        {
            Destroy(section);
        }
        sections.Clear();

        //Makes initial sections
        for (int i = 0; i < firstSectionPoints.Length; i++)
        {
            sections.Add(Instantiate(sectionPrefab));
            sections[i].transform.position = firstSectionPoints[i].position;

            Section sectionScript = sections[i].GetComponent<Section>();
            sectionScript.sectionMaker = this;
            sectionScript.columnDelay = columnDelay;
            sectionScript.setMyScore(sectionScore);

            sectionScore++;
        }

        StartCoroutine(setFirstKey());  //Gets key of first section (button which triggers game)
        currentSection = sections[0];

        //Activate tutorial
        tutorial.SetBool("Disable", false);
    }

    IEnumerator setFirstKey()
    {
        yield return new WaitForEndOfFrame();
        firstKey = sections[0].GetComponent<Section>().myButton;
        sections[0].GetComponent<Section>().isMyTurn = true;
    }

    void Update()
    {
        if (lost)
            return;

        if (!isPlaying && Input.GetKeyDown(firstKey))
        {
            isPlaying = true;
            soundManager.gameStart();
        }

        if (isPlaying)
        {
            //Scroll
            //Vector3 moveVector = new Vector2(-currentSpeed * Time.deltaTime, 0);

            float mouseX = Input.mousePosition.x;
            float mouseDistance = Screen.width - mouseX;
            float Xpercent = 100 - (100 * mouseDistance) / Screen.width;

            if (Xpercent < inputRange)
                Xpercent = inputRange;
            Xpercent -= inputRange;

            float inputX = Xpercent * (100/ (100-inputRange));

            float baseScrollSpeed = spaceBetweenColumns / columnDelay;
            float speedMultiplier = ((maxSpeed-minSpeed) / 100f) * inputX + minSpeed;

            //Check if previous section.x + wave Offset is smaller than a value
            if (previousSection != null)
            {
                int columnIndex = previousSection.GetComponent<Section>().columnIndex;
                float waveX = (previousSection.transform.position.x - 3) + (spaceBetweenColumns * columnIndex);

                if (speedMultiplier > 1 && waveX < minWaveX)
                    speedMultiplier = 1;

                if (speedMultiplier < 1 && waveX > maxWaveX)
                    speedMultiplier = 1.1f;
            }

            float spd = baseScrollSpeed * speedMultiplier;

            Vector3 moveVector = new Vector2(-spd * Time.deltaTime, 0);


            //Make these variables changeable in inspector

            //Make vector based on mouse pos here
            foreach (GameObject section in sections)
            {
                section.transform.Translate(moveVector);
            }

            //Check if lost (scrolled too far)
            if (score > 0)
            {
                int previousIndex = sections.IndexOf(currentSection) - 1;
                Section prevSection = sections[previousIndex].GetComponent<Section>();
                if (prevSection.tooLate)  //if finished waiving, its too late
                {
                    lose("You didn't click soon enough");
                }
            }

            //Delete and create if scrolled out of screen
            if (sections[0].transform.position.x <= destroyX)
            {
                destroyAndReplaceSection();
            }

            //Increment speed;
            if (incrementTimer <= 0)
            {
                //Increment
                incrementTimer = incrementFrequency;
                incrementSpeed();
            }
            incrementTimer -= Time.deltaTime;
        }
    }

    void incrementSpeed()
    {
        currentSpeed += incrementBy;
        columnDelay = spaceBetweenColumns / currentSpeed;
        foreach (GameObject section in sections)
        {
            section.GetComponent<Section>().columnDelay = columnDelay;
        }
    }

    public void lose(string reason)
    {
        Debug.Log("you lose");
        isPlaying = false;
        lost = true;
        currentSection.GetComponent<Section>().lose();
        foreach (GameObject section in sections)
        {
            section.GetComponent<Section>().stopClapping();
        }   //Make everyone stop clapping

        Manager.manager.lose(score+1, reason);
    }

    void destroyAndReplaceSection()
    {
        Destroy(sections[0].gameObject);
        sections.RemoveAt(0);

        sectionScore++;

        GameObject newSection = Instantiate(sectionPrefab);
        Vector2 newPos = sections[sections.Count - 1].transform.position + spaceBetweenSections;
        newSection.transform.position = newPos;
        sections.Add(newSection);

        Section sectionScript = newSection.GetComponent<Section>();
        sectionScript.sectionMaker = this;
        sectionScript.columnDelay = columnDelay;
        sectionScript.setMyScore(sectionScore);
    }

    public void tapped(GameObject whichSection)
    {
        int sectionIndex = sections.IndexOf(whichSection);
        Section section = whichSection.GetComponent<Section>();

        bool passed = false;

        if (score == 0)
        {
            passed = true;

            //Deactivate tutorial
            tutorial.SetBool("Disable", true);
        }
        else
        {
            Section prevSection = sections[sectionIndex - 1].GetComponent<Section>();
            if (prevSection.columnIndex > 6)
            {
                passed = true;
            } else {
                passed = false;
            }
        }

        if (passed)
        {
            Debug.Log("passed");
            //Passed
            whichSection.GetComponent<Section>().wave();

            previousSection = currentSection;
            currentSection = sections[sectionIndex + 1];
            StartCoroutine(setNextSectionTurn());   //Delay so doesnt click for next section

            score++;
        }
        else {
            lose("You clicked too soon");
        }
    }

    IEnumerator setNextSectionTurn()
    {
        yield return new WaitForEndOfFrame();
        currentSection.GetComponent<Section>().isMyTurn = true;
    }
}
