using System;
using Newtonsoft.Json.Linq;

public class TunnelMessage
{
	private string Id;
	private JObject DataContent;

	public TunnelMessage(JObject dataContent, string id)
	{
		DataContent = dataContent;
		Id = id;
	}

	public string getMessageId()
    {
		return (string) DataContent["id"];
    }

	public JObject getDataContent()
    {
		return (JObject) DataContent["data"];
    }

	public override string ToString()
    {
		string header = "{\"id\" : \"tunnel/send\", \"data\" : {\"dest\" : \"" + Id + "\", \"data\" : ";
		string ending = "}}";

		return header + DataContent.ToString() + ending;
	}
}
