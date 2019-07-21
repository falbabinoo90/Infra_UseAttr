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
