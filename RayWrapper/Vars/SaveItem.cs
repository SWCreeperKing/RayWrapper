using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
                var dec = _cypher.decrypt.Invoke(enc);

                var before = Console.ForegroundColor;
                if (str == enc)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("[WARNING] ENCRYPTION RESULTS IN BASE STRING");
                }

                if (str != dec)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] DECRYPTION DOES NOT RESULT THE INPUT TEXT");
                    isCypherValid = false;
                    return;
                }

                Console.ForegroundColor = before;

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

    public class SaveItem<T> : ISave where T : Setable<T>
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
                Console.WriteLine($"ERR: {e.Message} HANDLED");
                Task.Delay(1).GetAwaiter().GetResult();
                return SaveString();
            }
        }

        public string FileName() => _fileName;

        public static string Encrypt(string str) => ISave.isCypherValid ? ISave.Cypher.encrypt.Invoke(str) : str;
        public static string Decrypt(string str) => ISave.isCypherValid ? ISave.Cypher.decrypt.Invoke(str) : str;
    }
}