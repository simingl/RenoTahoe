using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;

public enum QuestionType
{
	Single,
	InputNumber,
	PointOutFromMap
}

public class Question
{
	public string description{get;set;}
	public QuestionType type{get;set;}
	public Options option{get;set;}
	public string answer{get;set;}
}
	
public class Options
{
	[XmlElement("opt")]
	public List<string> opt = new List<string>();
	[XmlElement("description")]
	public List<string> desc = new List<string>();
}

public class Student
{
	public string usrName{get;set;}
}

public class Quiz
{
	[XmlElement("question")] 
	public List<Question> question = new List<Question>();
	[XmlElement("student")]
	public List<Student> student = new List<Student>();
	[XmlElement("userAnswer")]
	public List<string> userAnswer = new List<string>();
}

[XmlRoot("QuizSetting")]
public class QuizSettingContainer
{
	
	private const string path = "QuizSetting.xml";

	public Quiz quiz = new Quiz();

	public static QuizSettingContainer readData()
	{
		var serializer = new XmlSerializer(typeof(QuizSettingContainer));
		var stream = new FileStream(path, FileMode.Open);
		QuizSettingContainer container = serializer.Deserialize(stream) as QuizSettingContainer;
		stream.Close();
		return container;
	}

	public void writeData(){
		var serializer = new XmlSerializer(typeof(QuizSettingContainer));

		var encoding = Encoding.GetEncoding("UTF-8");

		using(StreamWriter stream = new StreamWriter( path, false, encoding))
		{
			serializer.Serialize(stream, this);
			stream.Close();
		}
	}
}