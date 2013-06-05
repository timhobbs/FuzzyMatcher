using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyMatcher.DataModel {

    [Serializable]
    public class DataColumnDefinition {

        public const int TYPE_STRING = 1;
        public const int TYPE_DATE = 2;
        public const int TYPE_NUMERIC = 3;

        public string ColumnName { get; set; }

        public int Type { get; set; }

        public string SourceName { get; set; }

        public int Hash { get; set; }

        public string[] EmptyValues { get; set; }

        public bool IsKey { get; set; }
    }
}
