using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;

public class OpenAIChat : MonoBehaviour
{
    private string apiKey = "sk-proj-yHqTkhd4zhFMuliDmVvwT3BlbkFJpwGEAos37BgRSKQmrpzE";

    public IEnumerator SendMessageToOpenAI(string message, System.Action<string> callback)
    {
        string url = "https://api.openai.com/v1/chat/completions";
        var requestData = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = "You are a helpful assistant." },
                new { role = "user", content = message }
            },
            max_tokens = 150,
            temperature = 0.7
        };

        string jsonData = JsonConvert.SerializeObject(requestData);
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("HTTP Error: " + request.responseCode + " - " + request.error);
        }
        else
        {
            var response = request.downloadHandler.text;
            Debug.Log(response);
            var jsonResponse = JsonConvert.DeserializeObject<OpenAIResponse>(response);
            callback(jsonResponse.choices[0].message.content);
        }
    }
}

[System.Serializable]
public class OpenAIResponse
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

[System.Serializable]
public class Message
{
    public string content;
}
