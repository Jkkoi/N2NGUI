using System.IO;

namespace N2NGUI;

using Google.Protobuf;

public static class Config
{
    private static string ConfigFileName = "data.conf";

    public static string GenerateN2NArguments()
    {
        var data = LoadConfig();
        var arguments = "";
        if (data.Server != string.Empty)
        {
            arguments += $" -l {data.Server}";
        }

        if (data.Community != string.Empty)
        {
            arguments += $" -c {data.Community}";
        }
        
        if (data.Password != string.Empty)
        {
            arguments += $" -k {data.Password}";
        }

        arguments += " -E";

        return arguments;
        return $"-l {data.Server} -c {data.Community} -k {data.Password} -E";
        return string.Empty;
    }

    public static void SaveConfig(string server, string lanAddress, string community, Data.Types.EnyptType type,
        string password)
    {
        Data data = new Data
        {
            Server = server,
            LanAddress = lanAddress,
            Community = community,
            Type = type,
            Password = password
        };
        using var output = File.Create(ConfigFileName);
        data.WriteTo(output);
    }

    public static Data LoadConfig()
    {
        using var input = new FileStream(ConfigFileName, FileMode.OpenOrCreate, FileAccess.Read);
        // using var input = File.OpenRead(ConfigFileName);
        return Data.Parser.ParseFrom(input);
    }
}