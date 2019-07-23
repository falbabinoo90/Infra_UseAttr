using Interfaces;
namespace DataModel
{
    public class Session: FeatureBase , ISession
    {
        public Session(string istrDir)
        {
            workingDir = istrDir;
        }
        private string workingDir = "";

        string ISession.GetSessionWorkingDirectory() => workingDir;

        double _Distance;
        [PParameter("Distance", Magnitude = Magnitude.Length, HasMin = true, MinSI = 0)]
        public double Distance
        {
            get => GetField(_Distance);
            set => SetField(ref _Distance, value, "Distance");
        }


        string _TxtFile = "" ;
        [PFile("TxtFile", Filter = "*.txt| *.CSV|")]
        public string TxtFile
        {
            get => GetField(_TxtFile);
            set
            {
                SetField(ref _TxtFile, value, "TxtFile");
            }
        }
        
    }

    public class Session_2 : FeatureBase, ISession
    {
        public Session_2(string istrDir)
        {
            workingDir = istrDir;
        }
        private string workingDir = "";

        string ISession.GetSessionWorkingDirectory() => workingDir;

        double _Temperature;
        [PParameter("Temperature", Magnitude = Magnitude.Temperature, HasMin = true, MinSI = 0)]
        public double Temperature
        {
            get => GetField(_Temperature);
            set => SetField(ref _Temperature, value, "Temperature");
        }

        string _AebdFile = "";
        [PFile("AebdFile", Filter = "*.aedb|")]
        public string AebdFile
        {
            get => GetField(_AebdFile);
            set
            {
                SetField(ref _AebdFile, value, "AebdFile");
            }
        }

        double _Surface;
        [PParameter("Surface", Magnitude = Magnitude.Area, HasMin = true, MinSI = 0)]
        public double Surface
        {
            get => GetField(_Surface);
            set => SetField(ref _Surface, value, "Surface");
        }


        string _TxtFile = "";
        [PFile("TxtFile", Filter = "*.txt| *.CSV")]
        public string TxtFile
        {
            get => GetField(_TxtFile);
            set
            {
                SetField(ref _TxtFile, value, "TxtFile");
            }
        }

    }
}
