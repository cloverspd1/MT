using System.Configuration;

namespace BEL.MaterialTrackingWorkflow.Common
{
    public class BundleHelper
    {
        public static string StyleVersion
        {
            get
            {
                return "<link href=\"{0}?v=" + ConfigurationManager.AppSettings["version"] + "\" rel=\"stylesheet\"/>";
            }
        }
        public static string ScriptVersion
        {
            get
            {
                return "<script src=\"{0}?v=" + ConfigurationManager.AppSettings["version"] + "\"></script>";
            }
        }  
    }
}