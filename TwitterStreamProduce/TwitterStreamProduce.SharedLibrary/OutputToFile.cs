using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TwitterStreamProduce.SharedLibrary.Models;

namespace TwitterStreamProduce.SharedLibrary
{
    public class OutputToFile
    {
        public static void WriteSummaryToFile(StreamDataEntity entity, string strFileName)
        {
            string strFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strFileName);
            string strJson = SerializeDeserialize.SerializeObject(entity);

            using (StreamWriter sw = File.AppendText(strFullName))
            {
                sw.WriteLine(strJson);
            }
        }
    }
}

