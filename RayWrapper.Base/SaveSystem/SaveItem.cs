using System.Text;
using Newtonsoft.Json;
using RayWrapper.Base.Extras;
using RayWrapper.Base.GameBox;
using static RayWrapper.Base.GameBox.Logger.Level;

namespace RayWrapper.Base.SaveSystem;

public interface ISave
{
    private static (Func<string, string> encrypt, Func<string, string> decrypt) _cypher = (null, null);
    public static bool isCypherValid { get; private set; }

    public static (Func<string, string> encrypt, Func<string, string> decrypt) Cypher
    {
        get => _cypher;
        set
        {
            _cypher = value;
            isCypherValid = true;
            if (_cypher.encrypt is null || _cypher.decrypt is null) isCypherValid = false;
            if (!isCypherValid)
            {
                if (_cypher.encrypt is null) Logger.Log(Error, "Encryption is Null");
                if (_cypher.decrypt is null) Logger.Log(Error, "Decryption is Null");
                return;
            }

            StringBuilder sb = new();
            Random r = new();
            var charStop = r.Next(100, 151);

            for (var i = 0; i < charStop; i++) sb.Append((char) r.Next(0, 256));

            var str = sb.ToString();
            var enc = _cypher.encrypt!.Invoke(str);
            var dec = _cypher.decrypt!.Invoke(enc);
            var flawless = str != enc;

            if (!flawless) Logger.Log(Warning, "ENCRYPTION RESULTS IN BASE STRING");
            isCypherValid = str == dec;
            Logger.Log(isCypherValid ? Info : Error,
                isCypherValid
                    ? $"Encryption Analysis Completed {(flawless ? "Flawlessly!" : "And Resulted In NO Change!")}"
                    : "DECRYPTION DOES NOT RESULT THE INPUT TEXT");
        }
    }

    public static void IsSaveInitCheck()
    {
        if (!SaveExt.SaveInit)
            throw new Exception("GameBox.InitSaveSystem() Not called, Save System Not Initialized");
    }

    void LoadString(string data, string file);
    string SaveString();
    string FileName();
}

public class SaveItem<T> : ISave
{
    private T _t;
    private string _fileName;

    public SaveItem(T obj, string fileName)
    {
        ISave.IsSaveInitCheck();
        _t = obj ?? throw new NullReferenceException();
        _fileName = fileName;
    }

    public void LoadString(string data, string file)
    {
        try
        {
            _t.Set(JsonConvert.DeserializeObject<T>(Decrypt(data)));
        }
        catch (JsonSerializationException)
        {
            Logger.Log(Warning, $"CORRUPTED SAVE DATA IN: {file}!!");
        }
    }

    public string SaveString()
    {
        try
        {
            return Encrypt(JsonConvert.SerializeObject(_t));
        }
        catch (InvalidOperationException e)
        {
            Logger.Log(Warning, $"ERR: {e.Message} HANDLED");
            Task.Delay(1).GetAwaiter().GetResult();
            return SaveString();
        }
    }

    public string FileName() => _fileName;

    public static string Encrypt(string str) => ISave.isCypherValid ? ISave.Cypher.encrypt(str) : str;
    public static string Decrypt(string str) => ISave.isCypherValid ? ISave.Cypher.decrypt(str) : str;
}