using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnswerPanelController : MonoBehaviour {


    Question question;
    //Option Panel
    public  Dropdown optionDropdown;
    //Input Panel
    public InputField inputPanelText;
    //Area Panel
    public Text areaPanelDetails;
    public Button submitButton;


    // Use this for initialization
	void Start () {


        submitButton.GetComponentInChildren<Text>().text = "Submit";
    }
	
	// Update is called once per frame
	void Update () {
	
	}


    public void setQuestion(Question inQuestion)
    {
      //  Debug.Log("answer panel got question");
         question = inQuestion;
        fillAnswers();
    }

    void fillOptions()
    {
        //Debug.Log("answer panel called fillOptions");

        optionDropdown.ClearOptions();
        foreach(Opt option in question.option.opt)
        {

            optionDropdown.options.Add(new Dropdown.OptionData(option.optDescription));
            
        }


        optionDropdown.value = 0;
        optionDropdown.RefreshShownValue();
    }


    void fillAnswers()
    {
        //Debug.Log("answer panel called fillAnswers");

        switch  (question.type)
        {
            case QuestionType.Single:
 

                optionDropdown.gameObject.SetActive(true);
                inputPanelText.gameObject.SetActive(false);
                areaPanelDetails.gameObject.SetActive(false);




                //Debug.Log("answer panel called QuestionType.Single");

                fillOptions();
                break;

            case QuestionType.InputNumber:
                //Debug.Log("answer panel called QuestionType.InputNumber");
 
                optionDropdown.gameObject.SetActive(false);
                inputPanelText.gameObject.SetActive(true);
                areaPanelDetails.gameObject.SetActive(false);

                inputPanelText.text = "";

                    break;

            case QuestionType.Area:
                //Debug.Log("answer panel called QuestionType.Area");

                optionDropdown.gameObject.SetActive(false);
                inputPanelText.gameObject.SetActive(false);
                areaPanelDetails.gameObject.SetActive(true);

                areaPanelDetails.text = "Please choose an area from map";
                break;

            default:
                Debug.Log("not found");
                break;
        }

    }

    
    public void SubmitButtonClicked()
    {

        switch (question.type)
        {
            case QuestionType.Single:
                question.userAnswer = optionDropdown.captionText.text;
                break;

            case QuestionType.InputNumber:
                question.userAnswer = inputPanelText.text;
                break;

            case QuestionType.Area:
                
                break;

            default:
                

                break;
        }

        QuizManager quizManager;
        quizManager = QuizManager.getInstance();

        if (question.userAnswer == question.answer)
        {
            quizManager.answerNum = 1;
        }
        else
        {
            quizManager.answerNum = 0;
        }
        string str = question.userAnswer;
        quizManager.endAnswerTime = (int)Time.realtimeSinceStartup;
        string timeConsumed = (quizManager.endAnswerTime - quizManager.startAnswerTime).ToString();
        quizManager.WriteToXml(str, quizManager.questionButtonCounter, 6);
        quizManager.WriteToXml(timeConsumed, quizManager.questionButtonCounter, 7);
        quizManager.answered = true;
        quizManager.displayResultBoard = false;

    }



}
