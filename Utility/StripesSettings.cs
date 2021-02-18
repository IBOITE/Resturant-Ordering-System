using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lokanta.Utility
{
    //birinci stripe sitenden account yapiyorym sonra (appsettings )'de keyler ekliyorym sonra  (utility)'de class olusturdum
    //porplar yaptim sonra (tools->Nuget Package Manager->manage Nuget...)'den(Stripe.net) indirdim
    // (startup)'de bu servese register yapiyorum 
    //bu class online odeme icin 
    // 11v
    public class StripesSettings
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }
}
