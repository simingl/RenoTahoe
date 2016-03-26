using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;
using RTS;


public class QuizManager : MonoBehaviour
{

  //  static string studentID;
    public Text ques;
    public Texture2D btnTexture;
    public GUIStyle btnGuiStyle;
    public GameObject RenoFolder;
    public GameObject Questions;
    public GameObject ScoreText;
    public InputField InputNumber;
    public GameObject OptionsButton;
    public GameObject StartButton;
    public GameObject ResumeButton;
    public GameObject PlayerFolder;
    [HideInInspector]
    public bool startQuestion = false;
    private float timeNow = 0f;
    // public GameObject backGround;
    private bool write = true;
    [HideInInspector]
    public int startAnswerTime;
    [HideInInspector]
    public int endAnswerTime;
    //minimapPosition---
    private Vector3 minimapPosition;
    private Camera minimapCam;
    public GameObject ButtonsOnMiniMap;
    private int HButtonsNum;
    private int VButtonsNum;
    private Vector2 minimapSize;
    public GameObject ButtonsOnMiniMapFolder;
    //minimapPosition---


    public GameObject resultTextPrefab;
    //   public Text resultTextInPrefab;
    public Text resultTextBoard;
    public GameObject resultBoard;
    [HideInInspector]
    public bool displayResultBoard;
    private bool startPopupQuestion = true;
    //    public GameObject btnChangeScene;
    private ConfigManager configManager;
    private QuizSettingContainer quizSettings;
    private QuizSettingContainer writeToStudentID;


    void Start()
    {
        startAnswerTime = 0;
        endAnswerTime = 0;
        InputNumber.gameObject.SetActive(false);
        ques.text = "";
        AnswerStateButton.SetActive(false);
        StartButton.GetComponentInChildren<Text>().text = "Start";
        resultTextBoard.text = "";
        QuizManager.getInstance().displayResultBoard = false;
        resultBoard.gameObject.SetActive(false);
        //       btnChangeScene.SetActive(false);
        configManager = ConfigManager.getInstance();
        HButtonsNum = configManager.getSceneHorizontalButtonsNum();
        VButtonsNum = configManager.getSceneVerticalButtonsNum();
        minimapCam = GameObject.FindGameObjectWithTag("Camera_minimap").GetComponent<Camera>();
        minimapPosition = new Vector3(minimapCam.pixelRect.x, minimapCam.pixelRect.yMax, 0);
        minimapSize = GameObject.FindGameObjectWithTag("Camera_minimap").GetComponent<MinimapManagement>().lastMinimapSize;
  //      writeToStudentID = new QuizSettingContainer();
    }

   

    void Update()
    {
        GetDronesArea();
        if (QuizManager.getInstance().answered)
        {
            AnswerState(QuizManager.getInstance().answerNum);
        }
        timeNow = Time.realtimeSinceStartup;
        if (timeNow > getQuizStartTime() && QuizManager.getInstance().write)
        {
            if (QuizManager.getInstance().questionButtonCounter + 1 == QuizManager.getInstance().getQuizSettings().quiz.question.Count)
            {
                QuizManager.getInstance().write = false;
            }
            else
            {
                OnPopUpQuestionButtonClick();
            }
        }
        if (QuizManager.getInstance().questionButtonCounter+1 == QuizManager.getInstance().getQuizSettings().quiz.question.Count 
            && QuizManager.getInstance().displayResultBoard)
        {
            QuizManager.getInstance().displayResultBoard = false;
            DisplayResult();
        }
    }

    private static QuizManager instance = new QuizManager();

    private XmlDocument doc = new XmlDocument();

    public static QuizManager getInstance()
    {
        return instance;
    }
    public QuizSettingContainer getQuizSettings()
    {
        if (quizSettings == null)
        {
            quizSettings = QuizSettingContainer.readData();
            //quizSettings.writeData();
        }
        return quizSettings;
    }

    public void WriteToStudentIDFile()
    {
        

    }


    public GameObject AnswerStateButton;
    [HideInInspector]
    public int answerNum;
    public void AnswerState(int n)
    {
        destroyOptions();
        AnswerStateButton.SetActive(true);
        if (n == 0)
        {
            AnswerStateButton.GetComponentInChildren<Text>().text = "wrong";
        }
        else
        {
            AnswerStateButton.GetComponentInChildren<Text>().text = "right";
        }
    }
    [HideInInspector]
    public int optionCounter = 0;
    [HideInInspector]
    public int questionButtonCounter = -1;
    private GameObject CloneOptionsButton;
    public List<GameObject> tmpCloneOptionsButton = new List<GameObject>();

    private void InstantiateOptionsButtons()
    {
        Vector3 myVector3 = new Vector3(0, -40 * (optionCounter + 1));
        string str = QuizManager.getInstance().quizSettings.quiz.question[QuizManager.getInstance().questionButtonCounter].option.opt[optionCounter].name + ": " + QuizManager.getInstance().quizSettings.quiz.question[QuizManager.getInstance().questionButtonCounter].option.opt[optionCounter].optDescription;
        ClonePrefabs(OptionsButton, ques.transform, myVector3, str);
    }

    private int getQuizStartTime()
    {
        return configManager.getSceneQuizStartTime();
    }

    private void InstantiateButtonsInSquare(int n)
    {
        int num = 0;
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                string str = num++.ToString();
                Vector3 myVector3 = new Vector3(-50 * (n - 1) + 100 * j, -50 - 40 * (i + 1), 0);
                ClonePrefabs(OptionsButton, ques.transform, myVector3, str);
            }
        }
    }

    private void InstantiateButtonsInMiniMap()
    {
        float buttonWidth = minimapSize.x / HButtonsNum;
        float buttonHeight = minimapSize.y / VButtonsNum;
        int num = 0;
        for (int i = 0; i < VButtonsNum; ++i)
        {
            for (int j = 0; j < HButtonsNum; ++j)
            {
                string str = num++.ToString();
                Vector3 myVector3 = minimapPosition + new Vector3(buttonWidth * j, -buttonHeight * (i+1), 0);
                //          Vector3 myVector3 = new Vector3(buttonWidth * j, -buttonHeight * i, 0);
                //Debug.Log ("minimapPosition is: "+minimapPosition );
                //Debug.Log("myvector3 is: "+ myVector3);
                ClonePrefabs(ButtonsOnMiniMap, ButtonsOnMiniMapFolder.transform, myVector3, str);
            }
        }
    }
    public bool answered = false;

    private void showNextQuestion()
    {
        int sizeOfStr;
        sizeOfStr = (int)QuizManager.getInstance().getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].description.Length;
        ques.rectTransform.sizeDelta = new Vector2(ques.GetComponent<Text>().fontSize * sizeOfStr, 30);
        ques.text = QuizManager.getInstance().getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].description;

    }

    private void destroyOptions()
    {
        for (int i = 0; i < QuizManager.getInstance().tmpCloneOptionsButton.Count; ++i)
        {
            Destroy(QuizManager.getInstance().tmpCloneOptionsButton[i].gameObject);
        }
        QuizManager.getInstance().tmpCloneOptionsButton.Clear();
    }

    private void CheckInputNumber()
    {
        if (InputNumber.text == QuizManager.getInstance().getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].answer.ToString())
        {
            QuizManager.getInstance().answerNum = 1;
        }
        else
        {
            QuizManager.getInstance().answerNum = 0;
        }
        QuizManager.getInstance().endAnswerTime =(int) Time.realtimeSinceStartup;
        string timeConsumed = (QuizManager.getInstance().endAnswerTime - QuizManager.getInstance().startAnswerTime).ToString();
        string str;
        str = InputNumber.text;
        QuizManager.getInstance().WriteToXml(str, QuizManager.getInstance().questionButtonCounter, 6);
        QuizManager.getInstance().WriteToXml(timeConsumed, QuizManager.getInstance().questionButtonCounter, 7);
        // quizManager.WriteToXml(str, quizManager.questionButtonCounter % quizManager.getQuizSettings().quiz.question.Count, 6);
        InputNumber.gameObject.SetActive(false);
        QuizManager.getInstance().answered = true;
        QuizManager.getInstance().displayResultBoard = true;
    }

    private void GetDronesArea()  //n is the number of drones
    {
        int droneCount = configManager.getSceneDroneCount();

        GameObject[] writeTheNumberOfDroneToXML = GameObject.FindGameObjectsWithTag("Drone");

        //var drone = writeTheNumberOfDroneToXML.GetComponent<Drone>();
        for (int i = 0; i < QuizManager.getInstance().getQuizSettings().quiz.question.Count; ++i)
        {
            if (QuizManager.getInstance().getQuizSettings().quiz.question[i].type == QuestionType.Area)
            {
                foreach (GameObject obj in writeTheNumberOfDroneToXML)
                {
                    if (obj.GetComponent<Drone>().droneNumber.ToString() == QuizManager.getInstance().getQuizSettings().quiz.question[i].droneNumber)
                    {
                        WriteToXml(obj.GetComponent<Drone>().getDroneArea().ToString(), i, 5);
                    }
                }
            }
        }
    }

    //write to XML file
    public void WriteToXml(string str, int questionCount, int num)
    {
        QuizSettingContainer myContainer = QuizManager.getInstance().getQuizSettings();
        if (num == 5)   //writeToXML answer;
        {
            myContainer.quiz.question[questionCount].answer = str;
        }
        if (num == 6) //writeToXML userAnswer
        {
            myContainer.quiz.question[questionCount].userAnswer = str;
        }
        if (num == 7) //writeToXML timeConsuming
        {
            myContainer.quiz.question[questionCount].timeConsumed= str;
        }
        QuizSettingContainer.Serialize(myContainer, ConfigManager.getInstance().studentID);
        //QuizSettingContainer.WriteData(myContainer, configManager.studentID);
    }

    public void OnPopUpQuestionButtonClick()
    {
        //if (QuizManager.getInstance().questionButtonCounter+1 < QuizManager.getInstance().getQuizSettings().quiz.question.Count)
        //{
            if (QuizManager.getInstance().answered || startPopupQuestion)
            {                
                QuizManager.getInstance().startAnswerTime = (int)Time.realtimeSinceStartup;
            int droneCount = configManager.getSceneDroneCount();
            QuizManager.getInstance().questionButtonCounter = (++QuizManager.getInstance().questionButtonCounter) % QuizManager.getInstance().getQuizSettings().quiz.question.Count; // problem
                showNextQuestion();
                StartButton.GetComponentInChildren<Text>().text = "Next";
                AnswerStateButton.SetActive(false);
                if (getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.Single)
                {
                    for (int i = 0; i < QuizManager.getInstance().getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].option.opt.Count; ++i)
                    {
                        optionCounter = i;
                        InstantiateOptionsButtons();
                    }
                }
                if (getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.InputNumber)
                {
                    InputNumber.text = "";
                    InputNumber.gameObject.SetActive(true);
                }
                if (getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter].type == QuestionType.Area)
                {
               //     GetDronesArea(droneCount);
              //      InstantiateButtonsInSquare((int)Mathf.Sqrt(droneCount));
                  InstantiateButtonsInMiniMap();
                }
            //   backGround.SetActive(true);
            /*       ScoreText.SetActive(false);
                   RenoFolder.SetActive(false);
                   Questions.SetActive(true);
                   PlayerFolder.SetActive(false);
                   */
     //       btnChangeScene.SetActive(true);
            BackgroundManager backgroundManager = GameObject.FindGameObjectWithTag("AnswerStateButton").GetComponent<BackgroundManager>();
            backgroundManager.initBackground();

            //   backGround.SetActive(true);
            ScoreText.SetActive(false);
            //RenoFolder.SetActive(false);
            Questions.SetActive(true);
            PlayerFolder.SetActive(false);
            //hideShowGameObjects
            MinimapManagement mapManagement = GameObject.FindGameObjectWithTag("Camera_minimap").GetComponent<MinimapManagement>();
            CameraMain cameraMain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMain>();
            //   mapManagement.cameraTurnOff(true);
            mapManagement.cameraTurnOff(false);
            cameraMain.turnOff(true);
            //   backGround.SetActive(true);

        }
        startPopupQuestion = false;
        QuizManager.getInstance().answered = false;
        QuizManager.getInstance().displayResultBoard = false;
        //}
    }

    private void ClonePrefabs(GameObject CloneObjs, Transform parrentTransform, Vector3 offSetVect3, string description)
    {
        CloneOptionsButton = (GameObject)Instantiate(CloneObjs, parrentTransform.position + offSetVect3, Quaternion.identity);
        CloneOptionsButton.transform.parent = parrentTransform;

        CloneOptionsButton.GetComponentInChildren<Text>().text = description;
        QuizManager.getInstance().tmpCloneOptionsButton.Add(CloneOptionsButton);
    }

    private void DisplayResult()  //Result board.
    {
        resultBoard.gameObject.SetActive(true);
        int max = 0;
        for (int i = 0; i < QuizManager.getInstance().getQuizSettings().quiz.question.Count; ++i)
        {
            if (max < QuizManager.getInstance().getQuizSettings().quiz.question[i].description.Length)
            {
                max = QuizManager.getInstance().getQuizSettings().quiz.question[i].description.Length;
            }
        }
        int maxLength = max * 12;
        //       resultTextInPrefab.rectTransform.sizeDelta = new Vector2(maxLength, 25);
        resultTextBoard.rectTransform.sizeDelta = new Vector2(maxLength, 25 * 3 * QuizManager.getInstance().getQuizSettings().quiz.question.Count);
        resultTextBoard.text = "Results:\n";
        for (int i = 0; i < QuizManager.getInstance().getQuizSettings().quiz.question.Count; ++i)
        {
            resultTextBoard.text += 
                i+1 + ": " + QuizManager.getInstance().getQuizSettings().quiz.question[i].description + "\n"
                 + "Answer: " + QuizManager.getInstance().getQuizSettings().quiz.question[i].answer
                 + "  " + "UserAnswer: " + QuizManager.getInstance().getQuizSettings().quiz.question[i].userAnswer
                 + "  " + "TimeConsuming: " + QuizManager.getInstance().getQuizSettings().quiz.question[i].timeConsumed + "sec" + "\n";
        }
    }

    public void ResumeSceneButtonClick()
    {
        StartButton.GetComponentInChildren<Text>().text = "Start";
        resultBoard.gameObject.SetActive(false);
        /*    RenoFolder.SetActive(true);
            Questions.SetActive(false);
            ScoreText.SetActive(true);
            PlayerFolder.SetActive(true);
            // backGround.SetActive(false);*/

        BackgroundManager backgroundManager = GameObject.FindGameObjectWithTag("AnswerStateButton").GetComponent<BackgroundManager>();
        backgroundManager.reset();

        //RenoFolder.SetActive(true);
   //     btnChangeScene.SetActive(false);
        Questions.SetActive(false);
        ScoreText.SetActive(true);
        PlayerFolder.SetActive(true);

        // backGround.SetActive(false);
        //Camera_minimap
        MinimapManagement mapManagement = GameObject.FindGameObjectWithTag("Camera_minimap").GetComponent<MinimapManagement>();
        CameraMain cameraMain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMain>();
        mapManagement.cameraTurnOff(false);
        cameraMain.turnOff(false);

        QuizManager.getInstance().answered = true;
    }
}