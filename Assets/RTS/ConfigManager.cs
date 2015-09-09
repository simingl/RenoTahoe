using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;

namespace RTS {
	public class ConfigManager {
		public const string CONFIG_FILENAME = "App.config";

		public const string HUD_SHOW_SCORE = "HUD_show_score";
		public const string HUD_SCORE_HUMAN = "HUD_score_human";
		public const string HUD_SCORE_CAR   = "HUD_score_car";
		public const string HUD_SCORE_DRONE_CRASH = "HUD_score_drone_crash";
		public const string HUD_SCORE_DRONE_DEAD = "HUD_score_drone_dead";
		public const string HUD_SCORE_HELICOPTER_CRASH = "HUD_score_helicopter_crash";

		public const string HUD_SHOW_RESOURCE_BAR = "HUD_ShowResourceBar";
		public const string HUD_SHOW_DRONE_SELECTION_BAR = "HUD_ShowDroneSelectionBar";
		public const string HUD_SHOW_MESSAGE_BAR = "HUD_ShowMessageBar" ;
		public const string HUD_SHOW_ORDER_BAR = "HUD_ShowOrderBar";

		public const string HUD_SHOW_PIP_CAMERA_BOTTOM = "ShowPIPCameraBottom" ;
		public const string HUD_SHOW_PIP_CAMERA_SHIFT = "ShowPIPCameraShift";

		public const string SCENE_DRONE_COUNT = "Scene_Drone_Count";
		public const string SCENE_TREE_COUNT = "Scene_Tree_Count";
		public const string SCENE_FIRE_COUNT = "Scene_Fire_Count";
		public const string SCENE_PEOPLE_COUNT = "Scene_People_Count";
		public const string SCENE_CAR_COUNT = "Scene_Car_Count";
		public const string SCENE_HELICOPTER_COUNT = "Scene_Helicopter_Count";

		private ExeConfigurationFileMap configMap;
		private Configuration config;

		private static ConfigManager instance;

		private AppSettingsSection appSettings;
		private ConfigManager(){
			configMap = new ExeConfigurationFileMap();
			configMap.ExeConfigFilename = CONFIG_FILENAME;
			config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
			appSettings = config.AppSettings;
		}

		public static ConfigManager getInstance(){
			if (instance == null) {
				instance = new ConfigManager();
			}
			return instance;
		}

		public string ReadSetting(string key){
			return appSettings.Settings[key].Value;
		}

		public bool getHUDShowScore(){
			string value = this.ReadSetting (ConfigManager.HUD_SHOW_SCORE);
			return bool.Parse (value);
		}

		public int getHUDScoreHuman(){
			string value = this.ReadSetting (ConfigManager.HUD_SCORE_HUMAN);
			return int.Parse(value);
		}

		public int getHUDScoreCar(){
			string value = this.ReadSetting (ConfigManager.HUD_SCORE_CAR);
			return int.Parse(value);
		}

		public int getHUDScoreDroneCrash(){
			string value = this.ReadSetting (ConfigManager.HUD_SCORE_DRONE_CRASH);
			return int.Parse(value);
		}
		public int getHUDScoreDroneDead(){
			string value = this.ReadSetting (ConfigManager.HUD_SCORE_DRONE_DEAD);
			return int.Parse(value);
		}
		public int getHUDScoreHelicopterCrash(){
			string value = this.ReadSetting (ConfigManager.HUD_SCORE_HELICOPTER_CRASH);
			return int.Parse(value);
		}




		public bool getHUDShowResourceBar(){
			string value = this.ReadSetting (ConfigManager.HUD_SHOW_RESOURCE_BAR);
			return bool.Parse (value);
		}
		public bool getHUDShowDroneSelectionBar(){
			string value = this.ReadSetting (ConfigManager.HUD_SHOW_DRONE_SELECTION_BAR);
			return bool.Parse (value);
		}		
		public bool getHUDShowMessageBar(){
			string value = this.ReadSetting (ConfigManager.HUD_SHOW_MESSAGE_BAR);
			return bool.Parse (value);
		}
		public bool getHUDShowOrderBar(){
			string value = this.ReadSetting (ConfigManager.HUD_SHOW_ORDER_BAR);
			return bool.Parse (value);
		}

		public bool getShowPIPCameraBottom(){
			string value = this.ReadSetting (ConfigManager.HUD_SHOW_ORDER_BAR);
			return bool.Parse (value);
		}




		public int getSceneDroneCount(){
			string value = this.ReadSetting (ConfigManager.SCENE_DRONE_COUNT);
			return int.Parse (value);
		}
		public int getSceneTreeCount(){
			string value = this.ReadSetting (ConfigManager.SCENE_TREE_COUNT);
			return int.Parse (value);
		}
		public int getSceneFireCount(){
			string value = this.ReadSetting (ConfigManager.SCENE_FIRE_COUNT);
			return int.Parse (value);
		}
		public int getScenePeopleCount(){
			string value = this.ReadSetting (ConfigManager.SCENE_PEOPLE_COUNT);
			return int.Parse (value);
		}
		public int getSceneCarCount(){
			string value = this.ReadSetting (ConfigManager.SCENE_CAR_COUNT);
			return int.Parse (value);
		}
		public int getSceneHelicopterCount(){
			string value = this.ReadSetting (ConfigManager.SCENE_HELICOPTER_COUNT);
			return int.Parse (value);
		}
	}
}