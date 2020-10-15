using System;
using Newtonsoft.Json;
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

	//Get a specific data content from a JSON file
	public JObject GetDataContent()
    {
		
		return (JObject) DataContent["data"];
    }

	//Send data in the correct form to the server
	public string SendDataPacket(dynamic packet)
	{
		dynamic headerData = new
		{
			id = "tunnel/send",
			data = new
			{
				dest = Id,
				data = packet
			}
		};

		string header = JsonConvert.SerializeObject(headerData);
		return header;
	}
}
