using System;
namespace AutomatedApiTestingCsharp
{
    public class Contents
    {
        public string translation { get; set; }
        public string text { get; set; }
        public string translated { get; set; }
    }

    public class Root
    {
        public Success success { get; set; }
        public Contents contents { get; set; }
    }

    public class Success
    {
        public int total { get; set; }
    }
}

