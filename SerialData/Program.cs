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

            int product_id = DataUtils.GetProductId(options.ProductSku);
            int site_id = DataUtils.GetSiteId(options.Site);

            string out_filename = options.OutputFile;
            if(out_filename == null || out_filename == "")
                out_filename = string.Format("{0}.csv", options.ProductSku);

            String info = string.Format("outfile={0}, product/site={1}({2})/{3}({4})",
                out_filename, options.ProductSku, product_id, options.Site, site_id);

            if (File.Exists(out_filename))
            {
                Console.WriteLine("Processing existing file. " + info);

                // Convert file to list and sort by date
                string delimiter = ",";

                string[][] infilearray = File.ReadAllLines(out_filename)
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
                Console.WriteLine("Processed length = {0}", fileitems.Count());

                // Get item from last date
                //DateTime fromDateTime = Convert.ToDateTime(s.First().ToArray()[1]);
                DateTime fromDateTime = fileitems[0].DateTime + TimeSpan.FromSeconds(1);
                DataItem[] data = DataUtils.GetDataItems(product_id, site_id, fromDateTime);

                Console.WriteLine("Update length = {0}", data.Length);
                if (data.Length > 0)
                {
                    fileitems.AddRange(data);

                    // Remove duplicates
                    // Asume data already ordered by date
                    // We group by serial and pick first in the group
                    data = fileitems.GroupBy(f => f.Serial).Select(f => f.FirstOrDefault()).OrderByDescending(f=>f.DateTime).OrderByDescending(f=>f.DateTime).ToArray();

                    string tempfile = "temp_" + options.ProductSku + ".csv";
                    DataUtils.DataItemsToCSV(data, tempfile);

                    // Write to file
                    DataUtils.DataItemsToCSV(fileitems.ToArray(), tempfile, append:false);

                    File.Delete(out_filename);
                    File.Move(tempfile, out_filename);
                    //File.Copy(tempfile, out_filename);
                }
            }
            else
            {
                Console.WriteLine("Crating file. " + info);
                DataItem[] items = DataUtils.DataItemsToCSV(product_id, site_id, out_filename, options.FromDateTime);
                Console.WriteLine("Data length = {0}", items.Count());
            }

            return 0;
        }

    }
}
