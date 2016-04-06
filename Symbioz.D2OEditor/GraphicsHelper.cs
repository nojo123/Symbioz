using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbioz.D2OEditor
{
    class GraphicsHelper
    {
        public static DataTable ToDataSource(List<string[]> list, string[] columnames, string tablename)
        {
            // New table.
            DataTable table = new DataTable();
            table.TableName = tablename;
            // Get max columns.
            int columns = 0;
            foreach (var array in list)
            {
                if (array.Length > columns)
                {
                    columns = array.Length;
                }
            }

            // Add columns.
            foreach (var colum in columnames)
            {
                table.Columns.Add(colum);
            }

            // Add rows.
            foreach (var array in list)
            {
                if (array.Length == columnames.Count())
                    table.Rows.Add(array);
            }

            return table;
        }
    }
}
