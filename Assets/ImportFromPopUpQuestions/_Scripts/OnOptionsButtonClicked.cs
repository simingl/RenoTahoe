using UnityEngine;
using System.Collections;

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
		for(int i=0; i<quizManager.tmpColonOptionsButton.Count; ++i)
		{
			if(this.gameObject == quizManager.tmpColonOptionsButton[i])
			{
				//quizManager.questionButtonCounter%quizManager.quizSettings.quiz.question.Count is question counter;
				if (   quizManager.getQuizSettings().quiz.question[(quizManager.questionButtonCounter)%quizManager.getQuizSettings().quiz.question.Count].option.opt[i].name
					== quizManager.getQuizSettings().quiz.question[(quizManager.questionButtonCounter)%quizManager.getQuizSettings().quiz.question.Count].answer)
					quizManager.answerNum=1;
				else 
					quizManager.answerNum=0;
			}
		}
		quizManager.answered=true;
	}
}
