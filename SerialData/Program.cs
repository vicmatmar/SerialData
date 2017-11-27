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
        static int Main(string[] args)
        {
            var options = new Options();
            var parser = new CommandLine.Parser(p => { p.MutuallyExclusive = true; });

            var isValid = parser.ParseArguments(args, options);
            if (!isValid)
            {
                Console.WriteLine(CommandLine.Text.HelpText.AutoBuild(options).ToString());
                return -1;
            }

            int product_id = options.ProductId;
            if(product_id < 0)
            {
                product_id = DataUtils.GetProductId(options.ProductSku);
            }
            int site_id = options.SiteId;
            string out_filename = options.OutputFile;

            string sku = DataUtils.GetProductSku(product_id);

            if(out_filename == null || out_filename == "")
                out_filename = string.Format("{0}.csv", sku);
            string file_name = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), out_filename);

            String line = string.Format("Creating {0} for product/site {1}/{2}, sku {3}", 
                out_filename, product_id, site_id, sku);
            Console.WriteLine(line);

            DataItem[] items = DataUtils.DataItemsToCSV(product_id, site_id, file_name);

            return 0;
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
