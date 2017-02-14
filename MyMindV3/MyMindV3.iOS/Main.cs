using System;
using UIKit;
using System.Diagnostics;

namespace MyMindV3.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            try
            {
                UIApplication.Main(args, null, "AppDelegate");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BANG! {0}--{1}", ex.Message, ex.InnerException);
            }
        }
    }
}
