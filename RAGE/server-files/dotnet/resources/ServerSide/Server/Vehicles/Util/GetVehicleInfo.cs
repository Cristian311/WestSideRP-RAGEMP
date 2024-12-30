using GTANetworkAPI;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class GetVehicleInfo : Script
{
    public GetVehicleInfo()
    {

    }

    public static Task<string> MakeAsyncRequest(string url, string contentType)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.ContentType = contentType;
        request.Method = WebRequestMethods.Http.Get;
        request.Timeout = 20000;
        request.Proxy = null;
        request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36";

        Task<WebResponse> task = Task.Factory.FromAsync(
            request.BeginGetResponse,
            asyncResult => request.EndGetResponse(asyncResult),
            (object)null);

        return task.ContinueWith(t => ReadStreamFromResponse(t.Result));
    }


    private static string ReadStreamFromResponse(WebResponse response)
    {
        using (Stream responseStream = response.GetResponseStream())
        using (StreamReader sr = new StreamReader(responseStream))
        {
            //Need to return this response
            string strContent = sr.ReadToEnd();
            return strContent;
        }
    }

    public async Task<List<string>> ReturnVehicleNamesAsync(Player player)
    {
        List<string> tempVehicleStore = new List<string>();

        player.SendChatMessage("Fetching vehicle data... this may take a couple seconds.");

        string html = await MakeAsyncRequest("https://wiki.rage.mp/index.php?title=Vehicles", "text/html");
        string pattern = @"<code[^>]*?>(.*?)</code>";
        MatchCollection matches = Regex.Matches(html, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        if (matches.Count != 0)
        {
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                tempVehicleStore.Add(groups[1].Value);
            }

            return tempVehicleStore;
        }
        else
        {
            player.SendChatMessage("Error matching regex to vehicles..");
        }

        return null;
    }

    public void WriteToFile(string path, string content)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, content);
        }
        catch (IOException e)
        {
            NAPI.Util.ConsoleOutput("Error: ", e.Message);
            NAPI.Util.ConsoleOutput("Stack: ", e.StackTrace);
        }
    }
}

public class VehicleData
{
    public string VehicleName { get; set; }
    public float MaxSpeed { get; set; }
    public float MaxBraking { get; set; }
    public float MaxTraction { get; set; }
    public float MaxAcceleration { get; set; }
    public int MaxNumberOfPassengers { get; set; }
    public int MaxOccupants { get; set; }
    public int VehicleClass { get; set; }
}