using System;
using System.Text;
using Newtonsoft.Json;
using RayWrapper.Objs;

namespace RayWrapper.Vars
{
    public interface ISave
    {
        void LoadString(string data);
        string SaveString();
        string FileName();
    }

    public class SaveItem<T> : ISave
    {
        private static (Func<string, string> encrypt, Func<string, string> decrypt) _cypher = (null, null);
        private static bool _isCypherValid = false;

        public static (Func<string, string> encrypt, Func<string, string> decrypt) Cypher
        {
            get => _cypher;
            set
            {
                _cypher = value;
                if (_cypher.encrypt is null || _cypher.decrypt is null)
                {
                    _isCypherValid = false;
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
                    _isCypherValid = false;
                    return;
                }
                _isCypherValid = true;
            }
        }

        private T _t;
        private string _fileName;

        public T Reference
        {
            get => _t;
            set => _t = value;
        }

        public SaveItem(T obj, string fileName)
        {
            if (!GameBox.SaveInit)
                throw new Exception("GameBox.InitSaveSystem() Not called, Save System Not Initialized");
            if (obj is null) throw new NullReferenceException();
            _t = obj;
            _fileName = fileName;
            GameBox.SaveList.Add(this);
        }

        public void LoadString(string data) => _t = JsonConvert.DeserializeObject<T>(Decrypt(data));
        public string SaveString() => Encrypt(JsonConvert.SerializeObject(_t));
        public string FileName() => _fileName;

        public static string Encrypt(string str) => _isCypherValid ? _cypher.encrypt.Invoke(str) : str;
        public static string Decrypt(string str) => _isCypherValid ? _cypher.decrypt.Invoke(str) : str;
    }
}