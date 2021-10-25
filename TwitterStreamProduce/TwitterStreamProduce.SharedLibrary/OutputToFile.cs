using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TwitterStreamProduce.SharedLibrary.Models;

namespace TwitterStreamProduce.SharedLibrary
{
    public class OutputToFile
    {
        private static readonly object _fileAccess = new object();

        public static void WriteSummaryToFile(OutputEntity entity, string strFileName)
        {
            string strFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strFileName);
            string strJson = SerializeDeserialize.SerializeObject<OutputEntity>(entity);

            try
            {
                bool IsLocked = FileIsLocked(strFullName);

                if (IsLocked) Thread.Sleep(500);

                using (StreamWriter sw = File.AppendText(strFullName))
                {
                    sw.WriteLine(strJson);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static bool FileIsLocked(string strFullFileName)
        {
            bool blnReturn = false;
            FileStream fs;
            try
            {
                fs = File.Open(strFullFileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
                fs.Close();
            }
            catch (IOException ex)
            {
                blnReturn = true;
            }

            return blnReturn;
        }
    }
}

