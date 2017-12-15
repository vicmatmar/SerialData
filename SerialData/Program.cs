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


            if (File.Exists(file_name))
            {
                // Convert file to list and sort by date
                string delimiter = ",";

                string[][] infilearray = File.ReadAllLines(file_name)
                    .Where(l => !string.IsNullOrEmpty(l))
                    .Select(l => l.Split(delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)).Skip(1).ToArray();

                List<DataItem> fileitems = new List<DataItem>();
                foreach (string[] lineitem in infilearray)
                {
                    DataItem item = new DataItem();
                    item.Serial = lineitem[0];
                    item.DateTime = Convert.ToDateTime(lineitem[1]);
                    item.Day = Convert.ToDateTime(lineitem[2]);
                    fileitems.Add(item);
                }
                fileitems.Sort((s1, s2) => s2.DateTime.CompareTo(s1.DateTime));

                // Get items from last date
                //DateTime fromDateTime = Convert.ToDateTime(s.First().ToArray()[1]);
                DateTime fromDateTime = fileitems[0].DateTime + TimeSpan.FromSeconds(1);
                DataItem[] data = DataUtils.GetDataItems(product_id, site_id, fromDateTime);
                DataUtils.DataItemsToCSV(data, "test.csv");

                // Append the rest
                DataUtils.DataItemsToCSV(fileitems.ToArray(), "test.csv", true);
            }
            else
            {
                DataItem[] items = DataUtils.DataItemsToCSV(product_id, site_id, file_name, options.FromDateTime);
            }

            return 0;
        }

    }
}
