using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;


public class QuizManager : MonoBehaviour
{
	private QuizSettingContainer quizSettings;
	private static QuizManager instance = new QuizManager();

	private XmlDocument doc = new XmlDocument();
//	private QuizManager(){}

	public static QuizManager getInstance(){
		return instance;
	}
	public QuizSettingContainer getQuizSettings(){
		if (quizSettings == null) {
			quizSettings = QuizSettingContainer.readData ();
			//quizSettings.writeData();
		}
		return quizSettings;
	}
	public Text ques;
	public Texture2D btnTexture;
	public GUIStyle btnGuiStyle;
	public int juggJ=0;
	public GameObject RenoFolder;
	public GameObject Questions;
    public GameObject ScoreText;
	public InputField InputNumber;
	public GameObject OptionsButton;
	public GameObject startButton;
    private bool displayChoice = false;
    public int counter;
    public GameObject PlayerFolder;
   // public GameObject backGround;

    void Start()
	{
		InputNumber.gameObject.SetActive(false);
		ques.text = "";
		AnswerStateButton.SetActive(false);
		startButton.GetComponentInChildren<Text>().text="Start";
//		RenoFolder = GameObject.FindWithTag("Reno");
//		Questions = GameObject.FindWithTag("Questions");
	}	

	void Update()
	{
		if(QuizManager.getInstance().answered)
		{
			AnswerState(QuizManager.getInstance().answerNum);
		}
	}
	public GameObject AnswerStateButton;
	public int answerNum;
	public void AnswerState(int answerNum)
	{
			destroyOptions();
		AnswerStateButton.SetActive(true);
		if(answerNum==0)
		{
			AnswerStateButton.GetComponentInChildren<Text>().text = "wrong";
		}
		else
		{
			AnswerStateButton.GetComponentInChildren<Text>().text = "right";
		}
	}

	public int optionCounter=0;
	public int questionButtonCounter=-1;
	private GameObject ColonOptionsButton;
	public List<GameObject> tmpColonOptionsButton=new List<GameObject>();
	public void initiateOptionsButtons()
	{
        ColonOptionsButton =(GameObject)Instantiate(OptionsButton, ques.transform.position + new Vector3(0,-40*(optionCounter+1),0), Quaternion.identity);
       
        ColonOptionsButton.transform.parent = ques.transform;
        ColonOptionsButton.GetComponentInChildren<Text>().text = QuizManager.getInstance().quizSettings.quiz.question[QuizManager.getInstance().questionButtonCounter%QuizManager.getInstance().quizSettings.quiz.question.Count].option.opt[optionCounter]+": "+ quizSettings.quiz.question[QuizManager.getInstance().questionButtonCounter%quizSettings.quiz.question.Count].option.desc[optionCounter];
		QuizManager.getInstance().tmpColonOptionsButton.Add(ColonOptionsButton);
	}

	public bool answered=false;

	private void showNextQuestion()
	{
		ques.text = QuizManager.getInstance().getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter%QuizManager.getInstance().getQuizSettings().quiz.question.Count].description;
	}

	private void destroyOptions()
	{
		for (int i=0; i<QuizManager.getInstance().tmpColonOptionsButton.Count;++i)
		{
			Destroy(QuizManager.getInstance().tmpColonOptionsButton[i].gameObject);
		}
		QuizManager.getInstance().tmpColonOptionsButton.Clear();
	}
		
	public void CheckInputNumber()
	{
		if(InputNumber.text == QuizManager.getInstance().getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter%QuizManager.getInstance().quizSettings.quiz.question.Count].answer.ToString())
		{
			QuizManager.getInstance().answerNum=1;
		}
		else
		{
			QuizManager.getInstance().answerNum=0;
		}
		InputNumber.gameObject.SetActive(false);
		QuizManager.getInstance().answered=true;
	}



	public void OnPopUpQuestionButtonClick()
	{
		if(QuizManager.getInstance().answered || startButton.GetComponentInChildren<Text>().text == "Start")
		{
			
			++QuizManager.getInstance().questionButtonCounter;
			showNextQuestion();
			startButton.GetComponentInChildren<Text>().text = "Next";
			AnswerStateButton.SetActive(false);
			if(getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter%QuizManager.getInstance().getQuizSettings().quiz.question.Count].type == QuestionType.Single)
			{
				for (int i=0; i<QuizManager.getInstance().getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter%QuizManager.getInstance().quizSettings.quiz.question.Count].option.opt.Count;++i)
				{
					optionCounter =i;
					initiateOptionsButtons();
				}
			}
			if (getQuizSettings().quiz.question[QuizManager.getInstance().questionButtonCounter%QuizManager.getInstance().quizSettings.quiz.question.Count].type == QuestionType.InputNumber)
			{
				InputNumber.text="";
				InputNumber.gameObject.SetActive(true);
			}
         //   backGround.SetActive(true);
            ScoreText.SetActive(false);
            RenoFolder.SetActive(false);
			Questions.SetActive(true);
            PlayerFolder.SetActive(false);
            
            displayChoice = true;
		}
		QuizManager.getInstance().answered=false;

	}

	public void ResumeSceneButtonClick()
	{
		startButton.GetComponentInChildren<Text>().text = "Start";
		RenoFolder.SetActive(true);
		Questions.SetActive(false);
        ScoreText.SetActive(true);
        PlayerFolder.SetActive(true);
        displayChoice = false;
       // backGround.SetActive(false);
        counter =0;
		QuizManager.getInstance().answered=true;
	}
}