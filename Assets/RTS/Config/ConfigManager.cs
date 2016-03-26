using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace RTS {
	public class ConfigManager {
		private static SettingsContainer settings;
		private static ConfigManager instance = new ConfigManager();

		private XmlDocument doc = new XmlDocument();
		private ConfigManager(){
			if (settings == null) {
				settings = SettingsContainer.readData ();
				//settings.writeData();
			}
		}

        public string studentID;

		public static ConfigManager getInstance(){
			return instance;
		}

		public bool getHUDShowScore(){
			return settings.hud.ShowScore;
		}

		public int getHUDScoreHuman(){
			return settings.hud.ScoreHuman;
		}

		public int getHUDScoreCar(){
			return settings.hud.ScoreCar;
		}

		public int getHUDScoreDroneCrash(){
			return settings.hud.ScoreDroneCrash;
		}
		public int getHUDScoreDroneDead(){
			return settings.hud.ScoreDroneDead;
		}
		public int getHUDScoreHelicopterCrash(){
			return settings.hud.ScoreHelicopterCrash;
		}


		public bool getHUDShowResourceBar(){
			return settings.hud.ShowResourceBar;
		}
		public bool getHUDShowDroneSelectionBar(){
			return settings.hud.ShowDroneSelectionBar;
		}		
		public bool getHUDShowMessageBar(){
			return settings.hud.ShowMessageBar;
		}
		public bool getHUDShowOrderBar(){
			return settings.hud.ShowOrderBar;
		}
		public bool getShowPIPCameraShift(){
			return settings.hud.ShowPIPCameraShift;
		}


		public int getSceneDroneCount(){
			return settings.scene.DroneCount;
		}
		public int getSceneTreeCount(){
			return settings.scene.TreeCount;
		}
		public int getSceneFireCount(){
			return settings.scene.FireCount;
		}
		public int getScenePeopleCount(){
			return settings.scene.PeopleCount;
		}
		public int getSceneCarCount(){
			return settings.scene.CarCount;
		}
		public int getSceneHelicopterCount(){
			return settings.scene.HelicopterCount;
		}
        public int getSceneQuizStartTime()
        {
            return settings.scene.QuizStartTime;
        }
        public int getSceneVerticalButtonsNum()
        {
            return settings.scene.VerticalButtonsNum;
        }
        public int getSceneHorizontalButtonsNum()
        {
            return settings.scene.HorizontalButtonsNum;
        }
    }
}