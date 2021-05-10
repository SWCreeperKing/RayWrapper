using System;
using System.Text;
using Newtonsoft.Json;
using RayWrapper.Objs;

namespace RayWrapper.Vars
{
    public interface ISave
    {
        private static (Func<string, string> encrypt, Func<string, string> decrypt) _cypher = (null, null);
        public static bool isCypherValid { get; private set; } = false;

        public static (Func<string, string> encrypt, Func<string, string> decrypt) Cypher
        {
            get => _cypher;
            set
            {
                _cypher = value;
                if (_cypher.encrypt is null || _cypher.decrypt is null)
                {
                    isCypherValid = false;
                    return;
                }

                StringBuilder sb = new();
                Random r = new();
                var charStop = r.Next(100, 151);
                for (var i = 0; i < charStop; i++) sb.Append((char) r.Next(0, 256));
                var str = sb.ToString();
                var enc = _cypher.encrypt.Invoke(str);
                var dec = _cypher.decrypt.Invoke(str);
                if (str == enc)
                {
                    var before = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("[WARNING] ENCRYPTION RESULTS IN BASE STRING");
                    Console.ForegroundColor = before;
                }

                if (str != dec)
                {
                    var before = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] DECRYPTION DOES NOT RESULT THE INPUT TEXT");
                    Console.ForegroundColor = before;
                    isCypherValid = false;
                    return;
                }

                isCypherValid = true;
            }
        }

        public static void IsSaveInitCheck()
        {
            if (!GameBox.SaveInit)
                throw new Exception("GameBox.InitSaveSystem() Not called, Save System Not Initialized");
        }
        
        void LoadString(string data);
        string SaveString();
        string FileName();
    }

    public class SaveItem<T> : ISave where T : ISetable
    {
        private T _t;
        private string _fileName;

        public SaveItem(T obj, string fileName)
        {
            ISave.IsSaveInitCheck();
            if (obj is null) throw new NullReferenceException();
            _t = obj;
            _fileName = fileName;
        }

        public void LoadString(string data) => _t.Set(JsonConvert.DeserializeObject<T>(Decrypt(data)));
        public string SaveString() => Encrypt(JsonConvert.SerializeObject(_t));
        public string FileName() => _fileName;

        public static string Encrypt(string str) => ISave.isCypherValid ? ISave.Cypher.encrypt.Invoke(str) : str;
        public static string Decrypt(string str) => ISave.isCypherValid ? ISave.Cypher.decrypt.Invoke(str) : str;
    }
}