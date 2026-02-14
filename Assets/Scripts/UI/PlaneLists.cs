using System.Collections.Generic;

public static class PlaneLists
{
    // Lock object for thread safety
    private static readonly object _lock = new object();
    
    // Cached lists - will be populated from config on first access
    private static bool _initialized = false;
    private static List<string> _ruPlanes;
    private static List<string> _ukPlanes;
    private static List<string> _frPlanes;
    private static List<string> _usPlanes;
    private static List<string> _gerPlanes;
    private static List<string> _itaPlanes;

    // Fallback hardcoded lists (used if config is not available)
    private static readonly List<string> _ruPlanesFallback = new List<string>()
    {
        //note: sorting is working, no need to order here
        "I-16 type 24",
        "Il-2 mod.1941",
        "Il-2 mod.1942",
        "Il-2 mod.1943",
        "La-5 FN ser.2",
        "La-5 ser.8",
        "La-5 F ser.38",
        "LaGG-3 ser.29",
        "Li-2",
        "MiG-3 ser.24",
        "Pe-2 ser.35",
        "Pe-2 ser.87",
        "U-2VS",
        "Yak-1 ser.69",
        "Yak-1 ser.127",
        "Yak-7B ser.36",
        "Yak-9 ser.1",
        "Yak-9T ser.1"
    };

    private static readonly List<string> _ukPlanesFallback = new List<string>()
    {
        "Hurricane Mk.II",
        "Mosquito F.B. Mk.VI ser.2",
        "Spitfire Mk.Vb",
        "Spitfire Mk.IXc",
        "Spitfire Mk.IXe",
        "Spitfire Mk.XIV",
        "Spitfire Mk.XIVe",
        "Tempest Mk.V ser.2",
        "Typhoon Mk.Ib",

        "Airco De Haviland 4",
        "Bristol F.2B Falcon II",
        "Bristol F.2B Falcon III",
        "Handley Page O/400",
        "S.E.5a",
        "Sopwith Camel",
        "Sopwith Dolphin",
        "Sopwith Snipe",
        "Sopwith Triplane"
    };

    private static readonly List<string> _frPlanesFallback = new List<string>()
    {
        "Breguet type 14 B.2",
        "SPAD VII.C1 150HP",
        "SPAD VII.C1 180HP",
        "SPAD XIII.C1"
    };

    private static readonly List<string> _usPlanesFallback = new List<string>()
    {
        "A-20B",
        "C-47A",
        "P-38J-25",
        "P-39L-1",
        "P-40E-1",
        "P-47D-22",
        "P-47D-28",
        "P-51B-5",
        "P-51D-15"

    };

    private static readonly List<string> _gerPlanesFallback = new List<string>()
    {
        "Ar 234 B-2",
        "Bf 109 E-7",
        "Bf 109 F-2",
        "Bf 109 F-4",
        "Bf 109 G-2",
        "Bf 109 G-4",
        "Bf 109 G-6",
        "Bf 109 G-6AS",
        "Bf 109 G-6 Late",
        "Bf 109 G-14",
        "Bf 109 K-4",
        "Bf-110 E2",
        "Bf-110 G2",
        "FW 190 A3",
        "FW 190 A5",
        "FW 190 A6",
        "FW 190 A8",
        "FW 190 D9",
        "He 111 H-6",
        "He 111 H-16",
        "Hs 129 B-2",
        "I.A.R. 80-A",
        "I.A.R. 80-B",
        "Ju-52/3m g4e",
        "Ju-87 D3",
        "Ju-88 A4",
        "Ju-88 C6",
        "Me 262 A",
        "Me 410 A-1",
        "Ta 152 H1",

        "Albatros D.Va",
        "DFW C.V",
        "Fokker D.VII",
        "Fokker D.VIIF",
        "Fokker D.VIII",
        "Fokker Dr.I",
        "Gotha G.V",
        "Halberstadt CL.II D.IIIa",
        "Halberstadt CL.II D.IIIau",
        "Nieuport 28.C1",
        "Pfalz D.IIIa",
        "Pfalz D.XII",
        "Schuckert D.IV"
    };

    private static readonly List<string> _itaPlanesFallback = new List<string>()
    {
        "MC 202 s8"
    };

    // Initialize lists from config on first access
    private static void Initialize()
    {
        lock (_lock)
        {
            // Double-check after acquiring lock
            if (_initialized) return;
            
            // Try to load from config
            if (ConfigLoader.HasConfigData())
            {
                var planesByCountry = ConfigLoader.GetPlanesByCountry();
                
                _ruPlanes = planesByCountry.ContainsKey(Country.RU) ? planesByCountry[Country.RU] : new List<string>();
                _ukPlanes = planesByCountry.ContainsKey(Country.UK) ? planesByCountry[Country.UK] : new List<string>();
                _frPlanes = planesByCountry.ContainsKey(Country.FR) ? planesByCountry[Country.FR] : new List<string>();
                _usPlanes = planesByCountry.ContainsKey(Country.US) ? planesByCountry[Country.US] : new List<string>();
                _gerPlanes = planesByCountry.ContainsKey(Country.GER) ? planesByCountry[Country.GER] : new List<string>();
                _itaPlanes = planesByCountry.ContainsKey(Country.ITA) ? planesByCountry[Country.ITA] : new List<string>();
                
                UnityEngine.Debug.Log("[PlaneLists] Initialized from config");
            }
            else
            {
                // Use fallback hardcoded lists
                _ruPlanes = new List<string>(_ruPlanesFallback);
                _ukPlanes = new List<string>(_ukPlanesFallback);
                _frPlanes = new List<string>(_frPlanesFallback);
                _usPlanes = new List<string>(_usPlanesFallback);
                _gerPlanes = new List<string>(_gerPlanesFallback);
                _itaPlanes = new List<string>(_itaPlanesFallback);
                
                UnityEngine.Debug.Log("[PlaneLists] Initialized from fallback hardcoded lists");
            }
            
            _initialized = true;
        }
    }

    // Public properties with lazy initialization
    public static List<string> RuPlanes
    {
        get
        {
            Initialize();
            return _ruPlanes;
        }
    }

    public static List<string> UkPlanes
    {
        get
        {
            Initialize();
            return _ukPlanes;
        }
    }

    public static List<string> FrPlanes
    {
        get
        {
            Initialize();
            return _frPlanes;
        }
    }

    public static List<string> UsPlanes
    {
        get
        {
            Initialize();
            return _usPlanes;
        }
    }

    public static List<string> GerPlanes
    {
        get
        {
            Initialize();
            return _gerPlanes;
        }
    }

    public static List<string> ItaPlanes
    {
        get
        {
            Initialize();
            return _itaPlanes;
        }
    }

    // Force reload from config (useful after config update)
    // Thread-safe implementation - holds lock through entire operation
    public static void Reload()
    {
        lock (_lock)
        {
            _initialized = false;
            
            // Reinitialize immediately within the same lock to prevent race conditions
            // Try to load from config
            if (ConfigLoader.HasConfigData())
            {
                var planesByCountry = ConfigLoader.GetPlanesByCountry();
                
                _ruPlanes = planesByCountry.ContainsKey(Country.RU) ? planesByCountry[Country.RU] : new List<string>();
                _ukPlanes = planesByCountry.ContainsKey(Country.UK) ? planesByCountry[Country.UK] : new List<string>();
                _frPlanes = planesByCountry.ContainsKey(Country.FR) ? planesByCountry[Country.FR] : new List<string>();
                _usPlanes = planesByCountry.ContainsKey(Country.US) ? planesByCountry[Country.US] : new List<string>();
                _gerPlanes = planesByCountry.ContainsKey(Country.GER) ? planesByCountry[Country.GER] : new List<string>();
                _itaPlanes = planesByCountry.ContainsKey(Country.ITA) ? planesByCountry[Country.ITA] : new List<string>();
                
                UnityEngine.Debug.Log("[PlaneLists] Reloaded from config");
            }
            else
            {
                // Use fallback hardcoded lists
                _ruPlanes = new List<string>(_ruPlanesFallback);
                _ukPlanes = new List<string>(_ukPlanesFallback);
                _frPlanes = new List<string>(_frPlanesFallback);
                _usPlanes = new List<string>(_usPlanesFallback);
                _gerPlanes = new List<string>(_gerPlanesFallback);
                _itaPlanes = new List<string>(_itaPlanesFallback);
                
                UnityEngine.Debug.Log("[PlaneLists] Reloaded from fallback hardcoded lists");
            }
            
            _initialized = true;
        }
    }
}
