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
    private QuizSettingContainer quizSettings;
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

    public Text ques;
    public Texture2D btnTexture;
    public GUIStyle btnGuiStyle;
    public GameObject RenoFolder;
    public GameObject Questions;
    public GameObject ScoreText;
    public InputField InputNumber;
    public GameObject OptionsButton;
    public GameObject startButton;
    [HideInInspector]
    public bool startQuestion = false;
    private float timeNow = 0f;
    // public GameObject backGround;
    private bool write = true;
    [HideInInspector]
    public int startAnswerTime;
    [HideInInspector]
    public int endAnswerTime;

    public GameObject resultTextPrefab;
 //   public Text resultTextInPrefab;
    public Text resultTextBoard;
    [HideInInspector]
    public bool displayResultBoard;

    public GameObject ResultBoardPanel;

    public GameObject PlayerFolder;
    public GameObject BtnChangeScene;
    public float startQuestionTime = 10.0f;

    void Start()
    {
        startAnswerTime = 0;
        endAnswerTime = 0;
        InputNumber.gameObject.SetActive(false);
        ques.text = "";
        AnswerStateButton.SetActive(false);
        startButton.GetComponentInChildren<Text>().text = "Start";
        resultTextBoard.text = "";
        
        QuizManager.getInstance().displayResultBoard = false;

        BtnChangeScene.SetActive(false);
    }

    void Update()
    {
        GetDronesArea();
        if (QuizManager.getInstance().answered)
        {
            AnswerState(QuizManager.getInstance().answerNum);
        }
        timeNow = Time.realtimeSinceStartup;
        if (timeNow > startQuestionTime && QuizManager.getInstance().write)
        {
          //  GetDronesArea();
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
    private GameObject ColonOptionsButton;
    public List<GameObject> tmpColonOptionsButton = new List<GameObject>();
    //private void InstantiateOptionsButtons()
    //{
    //    ColonOptionsButton = (GameObject)Instantiate(OptionsButton, ques.transform.position + new Vector3(0, -40 * (optionCounter + 1), 0), Quaternion.identity);
    //    ColonOptionsButton.transform.parent = ques.transform;

    //    ColonOptionsButton.GetComponentInChildren<Text>().text = QuizManager.getInstance().quizSettings.quiz.question[QuizManager.getInstance().questionButtonCounter].option.opt[optionCounter].name + ": " + QuizManager.getInstance().quizSettings.quiz.question[QuizManager.getInstance().questionButtonCounter].option.opt[optionCounter].optDescription;
    //    QuizManager.getInstance().tmpColonOptionsButton.Add(ColonOptionsButton);
    //}
    private void InstantiateOptionsButtons()
    {
        Vector3 myVector3 = new Vector3(0, -40 * (optionCounter + 1));
        string str = QuizManager.getInstance().quizSettings.quiz.question[QuizManager.getInstance().questionButtonCounter].option.opt[optionCounter].name + ": " + QuizManager.getInstance().quizSettings.quiz.question[QuizManager.getInstance().questionButtonCounter].option.opt[optionCounter].optDescription;
        ColonPrefabs(OptionsButton, ques.transform, myVector3, str);
    }

    //private void InstantiateButtonsInSquare(int n)
    //{
    //    int num = 0;
    //    for (int i=0; i< n; ++i)
    //    {
    //        for(int j=0; j< n; ++j)
    //        {
    //            ColonOptionsButton = (GameObject)Instantiate(OptionsButton, ques.transform.position + new Vector3(-50*(n-1)+100*j, -50-40 * (i + 1), 0), Quaternion.identity);
    //            ColonOptionsButton.transform.parent = ques.transform;

    //            ColonOptionsButton.GetComponentInChildren<Text>().text = num++.ToString();
    //            QuizManager.getInstance().tmpColonOptionsButton.Add(ColonOptionsButton);
    //        }
    //    }
    //}
    private void InstantiateButtonsInSquare(int n)
    {
        int num = 0;
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                string str = num++.ToString();
                Vector3 myVector3 = new Vector3(-50 * (n - 1) + 100 * j, -50 - 40 * (i + 1), 0);
                ColonPrefabs(OptionsButton, ques.transform, myVector3, str);
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
        for (int i = 0; i < QuizManager.getInstance().tmpColonOptionsButton.Count; ++i)
        {
            Destroy(QuizManager.getInstance().tmpColonOptionsButton[i].gameObject);
        }
        QuizManager.getInstance().tmpColonOptionsButton.Clear();
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
        string timeConsuming = (QuizManager.getInstance().endAnswerTime - QuizManager.getInstance().startAnswerTime).ToString();
        string str;
        str = InputNumber.text;
        QuizManager.getInstance().WriteToXml(str, QuizManager.getInstance().questionButtonCounter, 6);
        QuizManager.getInstance().WriteToXml(timeConsuming, QuizManager.getInstance().questionButtonCounter, 7);
        // quizManager.WriteToXml(str, quizManager.questionButtonCounter % quizManager.getQuizSettings().quiz.question.Count, 6);
        InputNumber.gameObject.SetActive(false);
        QuizManager.getInstance().answered = true;
        QuizManager.getInstance().displayResultBoard = true;
    }

    private void GetDronesArea()  //n is the number of drones
    {
        int droneCount = ConfigManager.getInstance().getSceneDroneCount();

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
                //for (int j = 0; j < droneCount; ++j)
                //{
                //    if (drone.droneNumber.ToString() == QuizManager.getInstance().getQuizSettings().quiz.question[i].droneNumber)
                //        WriteToXml(drone.getDroneArea().ToString());
                //}
            }
        }
    }

    //write to XML file
    public void WriteToXml(string str, int questionCount, int num)
    {
        QuizSettingContainer myContainer = getQuizSettings();
        //Debug.Log(QuizManager.getInstance().getQuizSettings().quiz);
        //Quiz myQuiz = QuizManager.getInstance().getQuizSettings().quiz;

        //Debug.Log(QuizManager.getInstance().quizSettings.quiz);
        //myQuiz.question[0].option.opt[0].optDescription= "2";
        //myQuiz.question[0].userAnswer = "姜美花";

        //Debug.Log(getQuizSettings().quiz.question[0].option.opt[0].optDescription);
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
            myContainer.quiz.question[questionCount].timeConsuming= str;
        }
        QuizSettingContainer.Serialize(myContainer);
    }

    private void OnPopUpQuestionButtonClick()
    {
        //if (QuizManager.getInstance().questionButtonCounter+1 < QuizManager.getInstance().getQuizSettings().quiz.question.Count)
        //{

            if (QuizManager.getInstance().answered || startButton.GetComponentInChildren<Text>().text == "Start")
            {
                BtnChangeScene.SetActive(true);
                QuizManager.getInstance().startAnswerTime = (int)Time.realtimeSinceStartup;
                int droneCount = ConfigManager.getInstance().getSceneDroneCount();
                QuizManager.getInstance().questionButtonCounter = (++QuizManager.getInstance().questionButtonCounter) % QuizManager.getInstance().getQuizSettings().quiz.question.Count; // problem
                showNextQuestion();
                startButton.GetComponentInChildren<Text>().text = "Next";
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
                    //GetDronesArea(droneCount);
                    InstantiateButtonsInSquare((int)Mathf.Sqrt(droneCount));
                }
            //   backGround.SetActive(true);
            BackgroundManager backgroundManager = GameObject.FindGameObjectWithTag("AnswerStateButton").GetComponent<BackgroundManager>();
            //  backgroundManager.reset();
            backgroundManager.initBackground();


            ScoreText.SetActive(false);
            RenoFolder.SetActive(false);
            Questions.SetActive(true);
            PlayerFolder.SetActive(false);

            MinimapManagement mapManagement = GameObject.FindGameObjectWithTag("Camera_minimap").GetComponent<MinimapManagement>();
            CameraMain cameraMain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMain>();
            mapManagement.cameraTurnOff(true);
            cameraMain.turnOff(true);


        }
            QuizManager.getInstance().answered = false;
        QuizManager.getInstance().displayResultBoard = false;
        //}
    }

    private void ColonPrefabs(GameObject colonObjs, Transform parrentTransform, Vector3 offSetVect3, string description)
    {
        ColonOptionsButton = (GameObject)Instantiate(colonObjs, parrentTransform.position + offSetVect3, Quaternion.identity);
        ColonOptionsButton.transform.parent = parrentTransform;

        ColonOptionsButton.GetComponentInChildren<Text>().text = description;
        QuizManager.getInstance().tmpColonOptionsButton.Add(ColonOptionsButton);
    }

    private void DisplayResult()  //Result board.
    {
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
        /*        string str = QuizManager.getInstance().getQuizSettings().quiz.question[questionButtonCounter].description + "@";
                str += "Answer: " + QuizManager.getInstance().getQuizSettings().quiz.question[questionButtonCounter].answer
                         + "  " + "UserAnswer: " + QuizManager.getInstance().getQuizSettings().quiz.question[questionButtonCounter].userAnswer
                         + "  " + "TimeConsuming: " + QuizManager.getInstance().getQuizSettings().quiz.question[questionButtonCounter].timeConsuming;
                str.Replace("@", System.Environment.NewLine);
        */        //  ColonPrefabs(OptionsButton, ques.transform, vect3, str);
        resultTextBoard.text = "Results:\n";
        for (int i = 0; i < QuizManager.getInstance().getQuizSettings().quiz.question.Count; ++i)
        {
            resultTextBoard.text += 
                i+1 + ": " + QuizManager.getInstance().getQuizSettings().quiz.question[i].description + "\n"
                 + "Answer: " + QuizManager.getInstance().getQuizSettings().quiz.question[i].answer
                 + "  " + "UserAnswer: " + QuizManager.getInstance().getQuizSettings().quiz.question[i].userAnswer
                 + "  " + "TimeConsuming: " + QuizManager.getInstance().getQuizSettings().quiz.question[i].timeConsuming + "sec" + "\n";
        }
        //resultTextBoard.text.Replace("@", "@" + System.Environment.NewLine);
        //   resultTextBoard.text = resultTextBoard.text.Replace("@", "@\n");
        //       resultTextInPrefab.text = str;

        //     Vector3 myVector3 = new Vector3(-50 * (n - 1) + 100 * j, -50 - 40 * (i + 1), 0);

    }

    public void ResumeSceneButtonClick()
    {
        BackgroundManager backgroundManager = GameObject.FindGameObjectWithTag("AnswerStateButton").GetComponent<BackgroundManager>();
        backgroundManager.reset();
        BtnChangeScene.SetActive(false);
        startButton.GetComponentInChildren<Text>().text = "Start";
        RenoFolder.SetActive(true);
        Questions.SetActive(false);
        ScoreText.SetActive(true);
        PlayerFolder.SetActive(true);
        // backGround.SetActive(false);
        MinimapManagement mapManagement = GameObject.FindGameObjectWithTag("Camera_minimap").GetComponent<MinimapManagement>();
        CameraMain cameraMain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMain>();
        mapManagement.cameraTurnOff(false);
        cameraMain.turnOff(false);
        QuizManager.getInstance().answered = true;
    }
}