using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyMatcher.DataModel {
    public class RowModel {

        private readonly IDictionary<string, RowModel> models = new Dictionary<string, RowModel>();
        private readonly IList<string> columnsByName = new List<string>();
        private readonly IList<DataColumnDefinition> columnsInt;

        public IList<DataColumnDefinition> Columns { get; set; }

        public RowModel GetRowModel(IList<DataColumnDefinition> columns) {
            string code = Encode(columns);
            RowModel model = null;
            models.TryGetValue(code, out model);
            if (model == null) {
                model = new RowModel(columns);
                models.Add(code, model);
            }

            return model;
        }

        private static string Encode(IList<DataColumnDefinition> columns) {
            var buffer = new StringBuilder();
            for (int i = 0; i < columns.Count; i++) {
                if (i > 0) {
                    buffer.Append("_");
                }
                buffer.Append(columns[i].ToString());
            }

            return buffer.ToString();
        }

        private RowModel(IList<DataColumnDefinition> columns) {
            this.columnsInt = columns;
            foreach (var column in columns) {
                this.Columns.Add(column);
                this.columnsByName.Add(column.ColumnName);
            }
        }

        public int GetCellId(DataColumnDefinition cell) {
            int index = this.Columns.IndexOf(cell);
            if (index == -1) {
                throw new Exception(String.Format("Column {0} not provided by data source {1}.", cell.ColumnName, cell.SourceName));
            }

            return index;
        }

        //public int GetCellId(string cell) {
        //    int index = columnsByName.IndexOf(cell);
        //    if (index == -1) {
        //        throw new Exception(String.Format("Column {0} not provided by data source {1}.", cell.ColumnName, cell.SourceName));
        //    }

        //    return index;
        //}
    }
}
