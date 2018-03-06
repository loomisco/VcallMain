using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Vcall.Helpers
{
    public class Log
    {
        public string Header;
        public string Message;

        private void CreateLogFolder()
        {
            string Dt = DateTime.Now.ToString("yyyyMMdd");
            string Folder;


            Folder = @"C:\APiLog";

            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }

        }
    


    public void writeToLog()
    {
      

        string Dt = DateTime.Now.ToString("yyyyMMdd");
        string Folder;
        string Fle;

        Folder = @"C:\APiLog";
        Fle = Folder + @"\" + Dt + ".txt";


        using (System.IO.StreamWriter file =
       new System.IO.StreamWriter(Fle, true))
        {
            file.WriteLine(Header);
            file.WriteLine(Message);
        }

    

    }

}
    }