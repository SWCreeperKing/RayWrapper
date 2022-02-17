using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static RayWrapper.GameBox;
using static RayWrapper.GameConsole.CommandLineColor;
using static RayWrapper.GameConsole.GameConsole;
using static RayWrapper.Vars.Logger.Level;

namespace RayWrapper.Vars
{
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
            if (!SaveInit)
                throw new Exception("GameBox.InitSaveSystem() Not called, Save System Not Initialized");
        }

        void LoadString(string data);
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

        public void LoadString(string data) => _t.Set(JsonConvert.DeserializeObject<T>(Decrypt(data)));

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

        public static string Encrypt(string str) => ISave.isCypherValid ? ISave.Cypher.encrypt.Invoke(str) : str;
        public static string Decrypt(string str) => ISave.isCypherValid ? ISave.Cypher.decrypt.Invoke(str) : str;
    }

    public static class SaveExt
    {
        public static void SaveToFile(this ISave t, string path)
        {
            using var sw = File.CreateText($"{path}/{t.FileName()}.RaySaveWrap");
            sw.Write(t.SaveString());
            sw.Close();
        }

        public static void LoadToFile(this ISave t, string path)
        {
            var file = $"{path}/{t.FileName()}.RaySaveWrap";
            if (!File.Exists(file)) return;
            using var sr = new StreamReader(file);
            t.LoadString(sr.ReadToEnd());
            sr.Close();
        }

        public static void SaveItems(this List<ISave> saveList)
        {
            WriteToConsole($"{SKYBLUE}Saving Start @ {DateTime.Now:G}");
            var start = GetTimeMs();
            ISave.IsSaveInitCheck();
            var path = GetSavePath;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            foreach (var t in saveList) t.SaveToFile(path);
            WriteToConsole($"{SKYBLUE}Saved in {new TimeVar(GetTimeMs() - start)}");
        }

        public static void LoadItems(this List<ISave> saveList)
        {
            WriteToConsole($"{SKYBLUE}Loading Start @ {DateTime.Now:G}");
            var start = GetTimeMs();
            ISave.IsSaveInitCheck();
            var path = GetSavePath;
            if (!Directory.Exists(path)) return;
            foreach (var t in saveList) t.LoadToFile(path);
            WriteToConsole($"{SKYBLUE}Loaded in {new TimeVar(GetTimeMs() - start)}");
        }

        public static void DeleteFile(this IEnumerable<ISave> saveList, string name)
        {
            ISave.IsSaveInitCheck();
            var path = GetSavePath;
            if (!Directory.Exists(path)) return;
            var file = $"{path}/{saveList.First(s => s.FileName() == name).FileName()}.RaySaveWrap";
            Console.WriteLine($"> DELETED {file} <");
            if (!File.Exists(file)) return;
            File.Delete(file);
        }

        public static void DeleteAll(this IEnumerable<ISave> saveList)
        {
            ISave.IsSaveInitCheck();
            var path = GetSavePath;
            if (!Directory.Exists(path)) return;
            foreach (var file in saveList.Select(t => $"{path}/{t.FileName()}.RaySaveWrap").Where(File.Exists))
                File.Delete(file);
        }
    }
}