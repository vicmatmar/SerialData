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


            if(File.Exists(file_name))
            {
                string delimiter = ",";

                var s = File.ReadAllLines(file_name)
                    .Where(l => !string.IsNullOrEmpty(l))
                    .Select(l => l.Split(delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)).Skip(1).OrderByDescending(f => f[1]) ;

                DateTime fromDateTime = Convert.ToDateTime(s.First().ToArray()[1]);

                DataItem[] data = DataUtils.GetDataItems(product_id, site_id, fromDateTime);

                using (StreamWriter sw = new StreamWriter("testc.csv"))
                {
                    foreach(DataItem item in data)
                    {

                    }
                }
            }

            DataItem[] items = DataUtils.DataItemsToCSV(product_id, site_id, file_name, options.FromDateTime);

            return 0;
        }

    }
}
