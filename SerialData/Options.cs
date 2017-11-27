using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;


namespace SerialData
{
    class Options
    {
        [Option('p', "ProductId", Required = false, DefaultValue = -1, MutuallyExclusiveSet = "product",
            HelpText = "ProductId")]
        public int ProductId { get; set; }

        [Option('k', "ProductSku", Required = false, MutuallyExclusiveSet = "product",
            HelpText = "ProductSku")]
        public string ProductSku { get; set; }

        [Option('s', "SiteId", Required = true,
            HelpText = "SiteId")]
        public int SiteId { get; set; }

        [Option('o', "OutputFile", Required = false,
            HelpText = "OutputFile (Defaults to SKU.csv)")]
        public string OutputFile { get; set; }
    }
}
