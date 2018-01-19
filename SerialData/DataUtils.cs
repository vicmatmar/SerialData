using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;



namespace SerialData
{
    public class DataUtils
    {
        public enum Product_table_Stlye { Normal, Lowes };

        static public Product_table_Stlye GetStyle(int product_id)
        {
            Product_table_Stlye style = Product_table_Stlye.Normal;
            switch (product_id)
            {
                case 46:
                    style = Product_table_Stlye.Lowes;
                    break;
                default:
                    style = Product_table_Stlye.Normal;
                    break;

            }
            return style;
        }

        static public DataItem[] GetDataItems(int product_id, int site_id, DateTime fromDateTime)
        {
            DataItem[] items;

            Product_table_Stlye table_style = GetStyle(product_id);
            switch (table_style)
            {
                case Product_table_Stlye.Normal:
                    items = GetNormalDataItems(product_id, site_id, fromDateTime);
                    break;
                case Product_table_Stlye.Lowes:
                    items = GetLowesDataItems(site_id, fromDateTime);
                    break;
                default:
                    items = GetNormalDataItems(product_id, site_id, fromDateTime);
                    break;

            }

            return items.ToArray();
        }


        static public DataItem[] GetNormalDataItems(int product_id, int site_id, DateTime fromDateTime)
        {
            List<DataItem> items = new List<DataItem>();
            using (ManufacturingStore_v2Entities cx = new ManufacturingStore_v2Entities())
            {
                var serials = cx.SerialNumbers.
                    Where(s => s.ProductId == product_id && 
                        s.EuiList.ProductionSiteId == site_id &&
                        (s.CreateDate > fromDateTime || s.UpdateDate >= fromDateTime)).
                    OrderBy(s => s.SerialNumber1);

                var serial_array = serials.Select(s => new { s.SerialNumber1, s.CreateDate, s.UpdateDate });

                foreach (var s in serial_array)
                {
                    DataItem item = new DataItem();
                    item.Serial = s.SerialNumber1.ToString();
                    if (s.UpdateDate.HasValue)
                        item.DateTime = s.UpdateDate.Value;
                    else
                        item.DateTime = s.CreateDate;
                    items.Add(item);
                }
            }

            items.Sort((s1, s2) => s2.DateTime.CompareTo(s1.DateTime));

            return items.ToArray();
        }

        static public DataItem[] GetLowesDataItems(int site_id, DateTime fromDateTime)
        {
            List<DataItem> items = new List<DataItem>();
            using (ManufacturingStore_v2Entities cx = new ManufacturingStore_v2Entities())
            {
                var serials = cx.LowesHubs.
                    Where(s => s.date >= fromDateTime).
                    GroupBy(s => s.smt_serial).
                    Select(s => s.OrderByDescending(x => x.date).FirstOrDefault()).
                    Select(s => new { s.smt_serial, s.date });

                foreach (var s in serials)
                {
                    DataItem item = new DataItem();
                    item.Serial = s.smt_serial;
                    item.DateTime = s.date;
                    items.Add(item);
                }
            }

            items.Sort((s1, s2) => s2.DateTime.CompareTo(s1.DateTime));

            return items.ToArray();
        }

        static public DataItem[] DataItemsToCSV(int product_id, int site_id, string file_path, DateTime fromDatetIme)
        {
            DataItem[] items = GetDataItems(product_id, site_id, fromDatetIme);

            DataItemsToCSV(items, file_path);

            return items;
        }

        static public void DataItemsToCSV(DataItem[] items, string file_path, bool append = false, string header = "Serial,DateTime,Day")
        {
            string folder = Path.GetDirectoryName(file_path);
            if (folder.Length > 0 && !Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            using (StreamWriter sw = new StreamWriter(file_path, append))
            {
                string line = "";
                if (!String.IsNullOrEmpty(header) && !append)
                {
                    line = string.Format(header);
                    sw.WriteLine(line);
                }
                foreach (var item in items)
                {
                    DateTime day = new DateTime(item.DateTime.Year, item.DateTime.Month, item.DateTime.Day);
                    line = string.Format("{0},{1},{2}", item.Serial, item.DateTime, day.ToShortDateString());
                    sw.WriteLine(line);
                }
                sw.Close();
            }
        }

        static public string GetProductSku(int product_id)
        {
            string sku;
            using (ManufacturingStore_v2Entities cx = new ManufacturingStore_v2Entities())
            {
                sku = cx.Products.Where(s => s.Id == product_id).Single().SKU;
            }
            return sku;
        }

        static public int GetProductId(string product_sku)
        {
            using (ManufacturingStore_v2Entities cx = new ManufacturingStore_v2Entities())
            {
                return cx.Products.Where(s => s.SKU == product_sku).Single().Id;
            }
        }

        static public int GetSiteId(string site)
        {
            using (ManufacturingStore_v2Entities cx = new ManufacturingStore_v2Entities())
            {
                return cx.ProductionSites.Where(p => p.Name == site).Single().Id;
            }
        }

    }
}
