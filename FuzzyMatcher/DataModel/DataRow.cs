using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyMatcher.DataModel {
    public class DataRow {

        public string SourceName { get; set; }

        public int RecordId { get; set; }

        public IList<DataCell> Cells { get; set; }

        public RowModel Model { get; set; }

        public IDictionary<string, object> Properties { get; set; }

        public DataCell GetData(DataColumnDefinition cell) {
            //if (cell instanceof PropertyBasedColumn) {
            //    return new DataCell(DataColumnDefinition.TYPE_STRING, getProperty(((PropertyBasedColumn)cell).getColumnName()));
            //}
            return Cells[Model.GetCellId(cell)];
        }
    }
}
