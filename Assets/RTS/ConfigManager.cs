using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;

namespace RTS {
	public class ConfigManager {
		private static ConfigManager instance;

		private NameValueCollection appSettings;
		private ConfigManager(){
		}

		public static ConfigManager getInstance(){
			if (instance == null) {
				instance = new ConfigManager();
			}
			return instance;
		}

		public string ReadSetting(string key){
			return this.appSettings[key];
		}

	}
}