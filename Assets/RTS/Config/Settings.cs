using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class HUDSettings {
	public bool ShowScore;
	public int ScoreHuman;
	public int ScoreCar;
	public int ScoreDroneCrash;
	public int ScoreDroneDead;
	public int ScoreHelicopterCrash;

	public bool ShowResourceBar;
	public bool ShowDroneSelectionBar;
	public bool ShowMessageBar;
	public bool ShowOrderBar;

	public bool ShowPIPCameraBottom;
	public bool ShowPIPCameraShift;
}

public class SceneSettings{
	public int DroneCount;
	public int PeopleCount;
	public int CarCount;
	public int HelicopterCount;
	public int TreeCount;
	public int FireCount;
}

[XmlRoot("Settings")]
public class SettingsContainer
{
	[XmlArrayItem("HUD")]
	public HUDSettings hud = new HUDSettings();

	[XmlArrayItem("Scene")]
	public SceneSettings scene = new SceneSettings();

	private const string path = "Settings.xml";

	public static SettingsContainer readData(){
		var serializer = new XmlSerializer(typeof(SettingsContainer));
		var stream = new FileStream(path, FileMode.Open);
		SettingsContainer container = serializer.Deserialize(stream) as SettingsContainer;
		stream.Close();
		return container;
	}
	
	public void writeData(){
		var serializer = new XmlSerializer(typeof(SettingsContainer));

		var encoding = Encoding.GetEncoding("UTF-8");
		
		using(StreamWriter stream = new StreamWriter( path, false, encoding))
		{
			serializer.Serialize(stream, this);
			stream.Close();
		}


	}


}