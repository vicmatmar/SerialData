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
        [Option('k', "ProductSku", Required = true,
            HelpText = "ProductSku")]
        public string ProductSku { get; set; }

        [Option('s', "Site", Required = true,
            HelpText = "Site")]
        public string Site { get; set; }

        [Option('o', "OutputFile", Required = false,
            HelpText = "OutputFile (Defaults to SKU.csv)")]
        public string OutputFile { get; set; }

        [Option('f', "FromDateTime", Required = false,
            HelpText = "FromDateTime")]
        public DateTime FromDateTime { get; set; }

    }
}
