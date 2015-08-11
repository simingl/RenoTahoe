using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;

namespace RTS {

	public class ConfigManager {
		public const string CONFIG_FILENAME = "App.config";

		private ExeConfigurationFileMap configMap;
		private Configuration config;

		private static ConfigManager instance;

		private NameValueCollection appSettings;
		private ConfigManager(){
			configMap = new ExeConfigurationFileMap();
			configMap.ExeConfigFilename = CONFIG_FILENAME;
			config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
		}

		public static ConfigManager getInstance(){
			if (instance == null) {
				instance = new ConfigManager();
			}
			return instance;
		}

		public string ReadSetting(string key){
			return config.AppSettings.Settings[key].Value;
		}
	}
}