using Chronicle.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chronicle.Web.Services
{
    public class KeyGeneratorService
    {
        public ChronicleContext chronicleContext { get; }
        public KeyGeneratorService(ChronicleContext chronicleContext)
        {
            this.chronicleContext = chronicleContext;
        }
        public string generateKey()
        {
            bool loop = true;
            String start = "";
            do
            {
                start = "TRAVEL";
                for (int i = 0; i < 3; i++)
                {
                    start = start +"-"+ RandomString(4);
                }
                var keyUnique = chronicleContext.Users.Where(k => k.Key == start).FirstOrDefault();
                if(keyUnique == null)
                {
                    loop = false;
                }
            } while (loop);

            return start;
        }
        public string generateFamilyKey()
        {
            bool loop = true;
            String start = "";
            do
            {
                start = "#";
                start = start + RandomString(6);
                var keyUnique = chronicleContext.Families.Where(k => k.JoinKey == start).FirstOrDefault();
                if (keyUnique == null)
                {
                    loop = false;
                }
            } while (loop);

            return start;
        }
        public string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
