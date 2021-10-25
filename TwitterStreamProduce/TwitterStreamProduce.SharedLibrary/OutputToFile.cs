using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

            lock (_fileAccess)
            {
                using (StreamWriter sw = File.AppendText(strFullName))
                {
                    sw.WriteLine(strJson);
                }
            }
        }
    }
}

