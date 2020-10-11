using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TunnelMessage
{
	private string Id;
	private JObject DataContent;
	private JObject data;

	public TunnelMessage(JObject dataContent, string id)
	{
		DataContent = dataContent;
		Id = id;
		
	}

	public string GetMessageId()
    {
		return (string) DataContent["id"];
    }

	public JObject GetDataContent()
    {
		
		return (JObject) DataContent["data"];
    }

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

	public override string ToString()
	{
		dynamic headerData = new
		{
			id = "tunnel/send",
			data = new
			{		
				dest = Id,				
				data = DataContent               
			}
		}; // le trol face :^)

		string header = JsonConvert.SerializeObject(headerData);		
		return header;
	}
}
