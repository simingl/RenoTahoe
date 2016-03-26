using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using RTS;

public class LoadingScene: MonoBehaviour {

    public InputField InputNumber;
    public Text errorText;
    public NextSceneButtonScript nextSceneButton;
    // Use this for initialization
    void Start () {
        nextSceneButton = new NextSceneButtonScript();

    }
    public void OnButtonClick()
    {
        ConfigManager.getInstance().studentID = InputNumber.text;
        if (ConfigManager.getInstance().studentID == "")
        {
            errorText.text = "Invalid StudentID";
        }
        else
        {
            nextSceneButton.NextScene();
        }
    }
}
