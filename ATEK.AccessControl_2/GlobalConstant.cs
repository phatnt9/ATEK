using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2
{
    public static class GlobalConstant
    {
        public static string messageDuplicated = "{0} is duplicated.";
        public static string messageSaveSucced = "Save operation succeeded.";
        public static string messageSaveFail = "Failed to save. Please try again.";
        public static string messageValidate = "{0} is mandatory. Please enter {1}.";
        public static string messageNothingSelected = "Nothing selected.";
        public static string messageDeleteConfirm = "Do you want to delete the selected {0}?";
        public static string messageDeleteSucced = "Delete operation succeeded.";
        public static string messageDeleteFail = "Failed to delete. Please try again.";
        public static string messageDeleteUse = "Can\'t delete {0} because it has been using on {1}.";
        public static string messageValidateNumber = "{0} must be {1} than {2}.";
        public static string messageNoDataSave = "There is no updated data to save.";

        public static string messageTitileInformation = "Information";
        public static string messageTitileError = "Error";
        public static string messageTitileWarning = "Warning";

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("https://console.firebase.google.com"))
                    //using (client.OpenRead("http://google.com/generate_204"))
                    {
                        Console.WriteLine("Connected");
                        return true;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Disconnected");
                return false;
            }
        }
    }
}