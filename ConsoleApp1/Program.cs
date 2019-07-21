using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using SpaceClaim.Api.V18;
using DataModel;
using Utilities;
using Interfaces;
namespace ConsoleApp1
{
    class Program 
    {
        static void Main(string[] args)
        {
            #region test spaceclaim
            //StartupOptions options = new StartupOptions(1);
            //options.ExecutableFolder = "C:\\Program Files\\ANSYS Inc\\v195\\scdm";
            //options.ShowApplication = false;
            //options.ShowSplashScreen = false;

            //Session scdm = Session.Start(options);
            //Api.AttachToSession(scdm);

            //Document doc = Document.Create();

            //Part p = doc.MainPart; 


            //doc.Path = "C:\\Users\\bfall\\Desktop\\DocTest.scdoc";
            //doc.Save();
            #endregion
            Session s = new Session("C:\\Users\\bfall\\Desktop\\MySession")
            {
                Distance = 5 
                
            };
            s.TxtFile = "C:\\Users\\bfall\\Desktop\\MySession\\MyFile_Iwanna_import.txt";
            IPAttributes IPA = s as IPAttributes;

            ICollection<PAttribute> l = IPA.GetAttributes(typeof(PAttribute));


            foreach (PAttribute PA in l )
            {
                string displayString =""; 
                PA.GetValueAsStringForDisplay(IPA, ref displayString);
                Console.WriteLine(PA.AttrID + " : " +  displayString);

                PFileAttribute pFile = PA as PFileAttribute;
                if(pFile !=null)
                {
                    string relativePath ; 
                    pFile.ProcessNewFilePath(s as ISession , displayString, out relativePath);
                    Console.WriteLine(PA.AttrID + " Relative path : " + relativePath);
                }
            }

            int toto = -1;
        }
    }
}
