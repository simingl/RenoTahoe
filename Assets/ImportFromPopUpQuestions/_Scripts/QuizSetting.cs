using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;

public enum QuestionType
{
    Single,
    InputNumber,
    Area
}

public class Question
{
    public string description { get; set; }
    public QuestionType type { get; set; }
    public string droneNumber { get; set; }
    public Options option { get; set; }
    public string answer { get; set; }
    public string userAnswer { get; set; }
    public string timeConsuming { get; set; }
}

public class Opt
{
    public string name;
    public string optDescription;
}

public class Options
{
    [XmlElement("opt")]
    public List<Opt> opt = new List<Opt>();
}

public class Quiz
{
    [XmlElement("question")]
    public List<Question> question = new List<Question>();
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

    //public void writeData(){
    //	var serializer = new XmlSerializer(typeof(QuizSettingContainer));

    //	var encoding = Encoding.GetEncoding("UTF-8");

    //	using(StreamWriter stream = new StreamWriter( path, false, encoding))
    //	{
    //		serializer.Serialize(stream, this);
    //		stream.Close();
    //	}
    //}

    static public void Serialize(QuizSettingContainer details)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(QuizSettingContainer));
        var encoding = Encoding.GetEncoding("UTF-8");
        using (TextWriter writer = new StreamWriter(path, false, encoding))
        {
            serializer.Serialize(writer, details);
        }
    }
}