using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.Utils
{
    public class RoleTranslate
    {
        private IDictionary<string, string> dict;

        public RoleTranslate()
        {
            dict = new Dictionary<string, string>
            {
                {"Admin", "Admin" },
                {"Client", "Klient" },
                {"Staff", "Obsługa" },
                {"Klient", "Client" },
                {"Obsługa", "Staff" }
            };
        }

        public string GetValue(string input)
        {
            return dict[input];
        }
    }

}