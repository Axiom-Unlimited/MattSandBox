using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class DialogueSystem : MonoBehaviour {

    [Header("Conditions")]
    [Tooltip("Checks if Characters are Talking")]
    public bool isSpeaking;
    private Animator playerAnim;
    private Rigidbody playerRB;
    [Tooltip("Repeat Whole Conversation")]
    public bool loop;
    [Tooltip("Random Response from NPC's")]
    public bool randomResponse;
    [Tooltip("Checks if Main Character has Questions")]
    public bool hasQuestions;
    //Finds GameManager [Call Exactly as Spelled]
    private GameObject GameManager;
    //Access State Checker
    private StateChecker stateChecker;
    [Tooltip("Current State (!!REFERENCE!!")]
    public int curConditionalState;
    [Tooltip("Max State for Response")]
    public int MaxConditionalState;
    [Tooltip("Min State for Response")]
    public int MinConditionalState;
    [Tooltip("Disables Listed Components whenever True")]
    public bool disableComponents;
    [Tooltip("List of Components to Disable when Talking")]
    public Behaviour[] listOfDisabledComponents;
    //Enable/Disable Mouse Cursor
    public bool mouseEnabled;
    [Tooltip("Lets the Player Know what button to press to start Interaction")]
    public GameObject interactionNoticeCanvas;

    [Header("Dialogue for Continous Conversation")]
    public Dialogue dialogue;
    private bool pauseToSpeak, playerSpeaking;
    private float curConversationTime;
    private int curDialogue;
    AudioSource audSrc;
    BoxCollider col;
    [Space(10)]
    [Header("Dialogue with Question at End")]
    [Tooltip("Increase Stage when Question is Answered")]
    public bool increaseStateOnAnswer;
    [Tooltip("Player has Question")]
    private bool menuActivated;
    public DialogueToQuestion questions;

    [Header("Random Response AI Dialogue")]
    public AudioClip[] randomPhrases;
    private int speakRandom;

    private void Start()
    {
        Default();
    }
    private void FixedUpdate()
    {
        if (!randomResponse)
        {
            curConditionalState = stateChecker.curState;
            if (curConditionalState >= MinConditionalState && curConditionalState <= MaxConditionalState)
            {
                CheckArrayMax();
                UpdateConversation();
                RunConversationTimer();
                if (hasQuestions)
                {
                    CheckQuestionButtons();
                    CheckMenu();
                }
            }
        }
    }

    //Checks for Dialogue
    void Default()
    {
        playerAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        audSrc = GetComponent<AudioSource>();
        audSrc.playOnAwake = false;
        audSrc.loop = false;
        col = GetComponent<BoxCollider>();
        col.size = col.size;
        col.isTrigger = true;
        curDialogue = 0;
        curConversationTime = dialogue.conversationOfMainCharacterTime[curDialogue];
        isSpeaking = false;
        pauseToSpeak = false;
        playerSpeaking = true;
        menuActivated = false;
        mouseEnabled = false;
        interactionNoticeCanvas.SetActive(false);
        CheckMouse();
        CheckGameManager();
        curConditionalState = stateChecker.curState;

        if (randomResponse)
        {
            if(randomPhrases == null)
            {
                Debug.LogError("NO RANDOM PHRASES!!! Add some to Continue");
            }
            else
            {
                speakRandom = Random.Range(0, randomPhrases.Length);
            }
        }

        if (randomResponse)
        {
            hasQuestions = false;
            loop = false;
        }
        else if (loop)
        {
            hasQuestions = false;
            randomResponse = false;
        }
        else if (hasQuestions)
        {
            loop = false;
            randomResponse = false;
        }

        if (!hasQuestions)
        {
            questions.questionMenu = null;
        }
        else if(hasQuestions && questions.questionMenu == null)
        {
            Debug.LogError("No Conversation GUI Canvas");
        }
    }
    void CheckGameManager()
    {
        if(GameObject.Find("GameManager") != true)
        {
            Debug.LogError("DIALOGUE SYSTEM REQUIRES A GAMEOBJECT CALLED 'GameManager' !!!");
        }
        else if (GameObject.Find("GameManager"))
        {
            GameManager = GameObject.Find("GameManager");
        }
        if(GameManager.GetComponent<StateChecker>() != true)
        {
            GameManager.AddComponent<StateChecker>();
        }
        stateChecker = GameManager.GetComponent<StateChecker>();
    }
    void UpdateConversation()
    {
        if (isSpeaking && !pauseToSpeak)
        {
            if (playerSpeaking)
            {
                audSrc.clip = dialogue.conversationOfMainCharacter[curDialogue];
                audSrc.Play();
                pauseToSpeak = true;
            }
            if (!playerSpeaking)
            {
                audSrc.clip = dialogue.conversationOfThisCharacter[curDialogue];
                audSrc.Play();
                curDialogue += 1;
                pauseToSpeak = true;
            }
        }
    }
    void RunConversationTimer()
    {
        if (pauseToSpeak)
        {
            curConversationTime -= Time.deltaTime;
            if (curConversationTime < 0)
            {
                if (playerSpeaking)
                {
                    playerSpeaking = false;
                    curConversationTime = dialogue.conversationOfThisCharacterTime[curDialogue];
                }
                else
                {
                    playerSpeaking = true;
                    curConversationTime = dialogue.conversationOfMainCharacterTime[curDialogue];
                }
                pauseToSpeak = false;
            }
        }
    }
    void CheckArrayMax()
    {
        if(curDialogue >= dialogue.conversationOfThisCharacter.Length && curDialogue >= dialogue.conversationOfMainCharacter.Length - 1)
        {
            if (!loop)
            {
                if (hasQuestions && isSpeaking)
                {
                    menuActivated = true;
                    pauseToSpeak = false;
                    isSpeaking = false;
                }
                else if(!hasQuestions && isSpeaking)
                {
                    disableComponents = false;
                    EnableComponents();
                    pauseToSpeak = false;
                    isSpeaking = false;
                }
            }
            else
            {
                curDialogue = 0;
                pauseToSpeak = true;
                isSpeaking = false;
            }
        }
    }
    void CheckMenu()
    {
        if (menuActivated)
        {
            questions.questionMenu.SetActive(true);
            mouseEnabled = true;
            CheckMouse();
        }
        else
        {
            questions.questionMenu.SetActive(false);
            mouseEnabled = false;
            CheckMouse();
        }
    }
    void DisableComponents()
    {
        if (disableComponents)
        {
            for (int i = 0; i < listOfDisabledComponents.Length; i++)
            {
                listOfDisabledComponents[i].enabled = false;
            }
        }
    }
    void EnableComponents()
    {
        if (!disableComponents)
        {
            for (int i = 0; i < listOfDisabledComponents.Length; i++)
            {
                listOfDisabledComponents[i].enabled = true;
            }
        }
    }
    void CheckMouse()
    {
        if (mouseEnabled)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if(!mouseEnabled)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    //Checks for DialogueToQuestion
    void CheckQuestionButtons()
    {
        if (questions.questionText.Length != questions.questionResponse.Length)
        {
            Debug.LogError("TEXT BUTTON LENGTH DOES NOT MATCH QUESTION RESPONSE LENGTH!!! (Need to make both Question Texts and Question Asked the Same Length)");
        }
        else if (questions.questionText != null)
        {
            if (questions.questionText.Length == 1)
            {
                questions.questionText[0].text = questions.questionResponse[0];
            }
            else if (questions.questionText.Length == 2)
            {
                questions.questionText[0].text = questions.questionResponse[0];
                questions.questionText[1].text = questions.questionResponse[1];
            }
            else if (questions.questionText.Length == 3)
            {
                questions.questionText[0].text = questions.questionResponse[0];
                questions.questionText[1].text = questions.questionResponse[1];
                questions.questionText[2].text = questions.questionResponse[2];
            }
            else if (questions.questionText.Length == 4)
            {
                questions.questionText[0].text = questions.questionResponse[0];
                questions.questionText[1].text = questions.questionResponse[1];
                questions.questionText[2].text = questions.questionResponse[2];
                questions.questionText[3].text = questions.questionResponse[3];
            }
            else if (questions.questionText.Length == 5)
            {
                questions.questionText[0].text = questions.questionResponse[0];
                questions.questionText[1].text = questions.questionResponse[1];
                questions.questionText[2].text = questions.questionResponse[2];
                questions.questionText[3].text = questions.questionResponse[3];
                questions.questionText[4].text = questions.questionResponse[4];
            }
            else if (questions.questionText.Length > 5)
            {
                Debug.LogError("TOO MANY QUESTIONS!!! (Code will need to be Modified)");
            }
            else
            {
                Debug.LogError("QUESTION BOOL ACTIVE!!! (No Questions Set)");
            }
        }
    }

    //Answers for Dialogue Questions
    public void FirstAnswer(int setStateToo)
    {
        if (questions.questionAnswers[0] != null)
        {
            audSrc.clip = questions.questionAnswers[0];
            audSrc.Play();

            if (increaseStateOnAnswer)
            {
                stateChecker.curState = setStateToo;
            }

            menuActivated = false;
            disableComponents = false;
            EnableComponents();
            mouseEnabled = false;
            CheckMouse();
        }
    }
    public void SecondAnswer(int setStateToo)
    {
        if (questions.questionAnswers[1] != null)
        {
            audSrc.clip = questions.questionAnswers[1];
            audSrc.Play();

            if (increaseStateOnAnswer)
            {
                stateChecker.curState = setStateToo;
            }

            menuActivated = false;
            disableComponents = false;
            EnableComponents();
            mouseEnabled = false;
            CheckMouse();
        }
    }
    public void ThirdAnswer(int setStateToo)
    {
        if (questions.questionAnswers[2] != null)
        {
            audSrc.clip = questions.questionAnswers[2];
            audSrc.Play();

            if (increaseStateOnAnswer)
            {
                stateChecker.curState = setStateToo;
            }

            menuActivated = false;
            disableComponents = false;
            EnableComponents();
            mouseEnabled = false;
            CheckMouse();
        }
    }
    public void FourthAnswer(int setStateToo)
    {
        if (questions.questionAnswers[3] != null)
        {
            audSrc.clip = questions.questionAnswers[3];
            audSrc.Play();

            if (increaseStateOnAnswer)
            {
                stateChecker.curState = setStateToo;
            }

            menuActivated = false;
            disableComponents = false;
            EnableComponents();
            mouseEnabled = false;
            CheckMouse();
        }
    }
    public void FithAnswer(int setStateToo)
    {
        if (questions.questionAnswers[4] != null)
        {
            audSrc.clip = questions.questionAnswers[4];
            audSrc.Play();

            if (increaseStateOnAnswer)
            {
                stateChecker.curState = setStateToo;
            }

            menuActivated = false;
            disableComponents = false;
            EnableComponents();
            mouseEnabled = false;
            CheckMouse();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        interactionNoticeCanvas.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        interactionNoticeCanvas.SetActive(false);
    }

    //Check Player Interaction
    private void OnTriggerStay(Collider col)
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (curConditionalState >= MinConditionalState && curConditionalState <= MaxConditionalState)
            {
                if (!randomResponse)
                {
                    isSpeaking = true;
                    disableComponents = true;
                    DisableComponents();
                    playerAnim.SetFloat("Forward", 0);
                    playerAnim.SetFloat("Jump", 0);
                    playerAnim.SetBool("OnGround", true);
                    playerRB.velocity = Vector3.right * 0;
                }
                else
                {
                    speakRandom = Random.Range(0, randomPhrases.Length);
                    audSrc.clip = randomPhrases[speakRandom];
                    audSrc.Play();
                }
            }
        }
    }

}

[System.Serializable]
public class Dialogue
{
    [Tooltip("Name of Character (Used as Reference Only)")]
    public string name;
    [Tooltip("Audio for Player")]
    public AudioClip[] conversationOfMainCharacter;
    [Tooltip("Time it takes to Complete Player Audio")]
    public float[] conversationOfMainCharacterTime;
    [Tooltip("Audio for AI")]
    public AudioClip[] conversationOfThisCharacter;
    [Tooltip("Time it takes to Complete AI Audio")]
    public float[] conversationOfThisCharacterTime;
}
[System.Serializable]
public class DialogueToQuestion
{
    [Tooltip("Menu for Questions")]
    public GameObject questionMenu;
    [Tooltip("Texts located inside Buttons for Questions")]
    public Text[] questionText;
    [Tooltip("Question Responses")]
    public string[] questionResponse;
    [Tooltip("Answers Given for Questions")]
    public AudioClip[] questionAnswers;
}
