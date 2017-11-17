using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace SerialData
{
    class Program
    {
        static void Main(string[] args)
        {

            int product_id = 46;
            int site_id = 7;
            string out_filename = "";

            string sku = DataUtils.GetProductSku(product_id);

            if(out_filename == "")
                out_filename = string.Format("{0}.csv", sku);
            string file_name = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), out_filename);

            String line = string.Format("Creating {0} for product {1}, {2}", out_filename, product_id, sku);
            Console.WriteLine(line);

            DataItem[] items = DataUtils.DataItemsToCSV(product_id, site_id, file_name);


        }

        static DataItem[] getDBData(int product_id, int site_id, string file_name)
        {
            DataItem[] items = DataUtils.DataItemsToCSV(product_id, site_id, file_name);
            foreach (var item in items)
            {
                string line = string.Format("{0},{1}", item.Serial, item.DateTime);
                Console.WriteLine(line);
            }
            return items;
        }

    }
}
