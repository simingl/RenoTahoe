using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OnOptionsButtonClicked : MonoBehaviour {
	private QuizManager quizManager;

	void Start()
	{
//		quizManager = GetComponent<QuizManager>();
//		quizManager = (QuizManager)FindObjectOfType(typeof(QuizManager));
		quizManager = QuizManager.getInstance();
	}

	public void ButtonClicked()
	{
        int uswerAnswer = 0;
        string str = "";
        for (int i=0; i<quizManager.tmpCloneOptionsButton.Count; ++i)
		{          
            if (this.gameObject == quizManager.tmpCloneOptionsButton[i])
			{
                uswerAnswer = i;
                if (quizManager.getQuizSettings().quiz.question[quizManager.questionButtonCounter].type == QuestionType.Area)
                {
                    if (quizManager.getQuizSettings().quiz.question[quizManager.questionButtonCounter].answer == i.ToString())
                    {
                        quizManager.answerNum = 1;
                    }
                    else
                        quizManager.answerNum = 0;
                    str = quizManager.tmpCloneOptionsButton[uswerAnswer].GetComponentInChildren<Text>().text;
                }

                //quizManager.questionButtonCounter%quizManager.quizSettings.quiz.question.Count is question counter;
                 else
                {
                    if (quizManager.getQuizSettings().quiz.question[quizManager.questionButtonCounter].option.opt[i].name
                        == quizManager.getQuizSettings().quiz.question[quizManager.questionButtonCounter].answer)
                        quizManager.answerNum = 1;
                    else
                        quizManager.answerNum = 0;
                    str = quizManager.getQuizSettings().quiz.question[quizManager.questionButtonCounter].option.opt[i].name;
                }                
			}
		}
        
        
        quizManager.endAnswerTime = (int)Time.realtimeSinceStartup;
        string timeConsumed = (quizManager.endAnswerTime - quizManager.startAnswerTime).ToString();
        quizManager.WriteToXml(str, quizManager.questionButtonCounter, 6);
        quizManager.WriteToXml(timeConsumed, quizManager.questionButtonCounter, 7);
        quizManager.answered=true;
        quizManager.displayResultBoard = true;
	}
}
