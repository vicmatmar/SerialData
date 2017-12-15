using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialData
{
    public class DataItem
    {
        //public DataItem(string serial, DateTime datetime, DateTime day)
        //{
        //    Serial = serial;
        //    DateTime = datetime;
        //    Day = day;
        //}
        public string Serial { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime Day { get; set; }
    }
}
