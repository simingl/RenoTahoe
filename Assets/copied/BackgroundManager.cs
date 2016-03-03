using UnityEngine;
using System.Collections;
using UnityEngine.UI;

enum BackgroundModes { eNone,eMap, eImage,eColor,eDisableColor };

public class BackgroundManager : MonoBehaviour {

	public GameObject bgPanelImage;
	public GameObject bgPanelForColorTop;
	public GameObject bgPanelForColorBtmFirst;
	public GameObject bgPanelForColorBtmSecond;
    public GameObject questionWhitePanel;


    private BackgroundModes currentBackground;

	// Use this for initialization
	void Start () {

        //bgPanelImage = GameObject.FindGameObjectWithTag ("BgImage");
        //bgPanelForColor = GameObject.FindGameObjectWithTag ("BgPanelColor");

        this.reset();

    }


	public void reset () {

		this.currentBackground = BackgroundModes.eNone;
		this.updateBackground ();
	}

	
	// Update is called once per frame
	void Update () {


	}

    public void initBackground()
    {
        if (this.currentBackground < BackgroundModes.eMap)
        {
            this.enableDisableBackgroundOnClick();
        }



    }


    public void enableDisableBackgroundOnClick () {

		this.currentBackground++;

		if (this.currentBackground > BackgroundModes.eDisableColor) {
			this.currentBackground = BackgroundModes.eMap;
		}

		this.updateBackground ();

	}

	private void updateBackground () {


		MinimapManagement mapManagement = GameObject.FindGameObjectWithTag ("Camera_minimap").GetComponent<MinimapManagement>();
		CameraMain cameraMain = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraMain>();

        //bgPanel.GetComponent <Image>()

        if (this.currentBackground == BackgroundModes.eNone)
        {
            questionWhitePanel.SetActive(false);

            bgPanelImage.SetActive(false);
            bgPanelForColorTop.SetActive(false);
            bgPanelForColorBtmFirst.SetActive(false);
            bgPanelForColorBtmSecond.SetActive(false);
           
            mapManagement.cameraTurnOff(false);
            questionWhitePanel.SetActive(false);

        }
        else if (this.currentBackground == BackgroundModes.eMap) {

			bgPanelImage.SetActive(false);
			bgPanelForColorTop.SetActive(false);
			bgPanelForColorBtmFirst.SetActive(false);
			bgPanelForColorBtmSecond.SetActive(false);
            questionWhitePanel.SetActive(true);
            mapManagement.cameraTurnOff(true);
			

		} else if (this.currentBackground == BackgroundModes.eImage) {
			
			bgPanelImage.SetActive(true);

			bgPanelForColorTop.SetActive(false);
			bgPanelForColorBtmFirst.SetActive(false);
			bgPanelForColorBtmSecond.SetActive(false);
            questionWhitePanel.SetActive(true);
            mapManagement.cameraTurnOff(true);
			

		} else if (this.currentBackground == BackgroundModes.eColor) {
			bgPanelImage.SetActive(false);

			bgPanelForColorTop.SetActive(true);
			bgPanelForColorBtmFirst.SetActive(false);
			bgPanelForColorBtmSecond.SetActive(true);
            questionWhitePanel.SetActive(true);
            bgPanelForColorTop.GetComponent<Image> ().color = Color.white;
			bgPanelForColorBtmSecond.GetComponent<Image> ().color = Color.white;

			mapManagement.cameraTurnOff(false);
			

		} else if (this.currentBackground == BackgroundModes.eDisableColor) {
			bgPanelImage.SetActive(false);

			bgPanelForColorTop.SetActive(true);
			bgPanelForColorBtmFirst.SetActive(true);
			bgPanelForColorBtmSecond.SetActive(true);
            questionWhitePanel.SetActive(true);
            bgPanelForColorTop.GetComponent<Image> ().color = Color.gray;
			bgPanelForColorBtmFirst.GetComponent<Image> ().color = Color.gray;
			bgPanelForColorBtmSecond.GetComponent<Image> ().color = Color.gray;

			mapManagement.cameraTurnOff(true);
			
		}




	}


}
