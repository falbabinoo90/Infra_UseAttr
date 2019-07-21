
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Resources;

namespace SPEOS.Services.CSharp
{
    public static class UtilResources
    {
        public static ResourceManager GetResxManager(Assembly Asm, string ResourcesFileName = "Resources")
        {
            string ResourceFileName = Asm.GetName().Name + ".Properties." + ResourcesFileName;

            ResourceManager RM = null;
            try
            {
                RM = new ResourceManager(ResourceFileName, Asm);
            }
            catch
            {
            }

            return RM;
        }

        public static string GetTextNoException(this ResourceManager RM, string Key)
        {
            string Text = null;
            try
            {
                Text = RM?.GetString(Key);
            }
            catch (MissingManifestResourceException MMREx)
            {
                Debug.WriteLine(MMREx.Message);
            }
            catch
            {
                
            }

            return Text;
        }
    }
}
