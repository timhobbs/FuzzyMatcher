using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuzzyMatcher.DataModel;

namespace FuzzyMatcher {
    public class Deduplicate {

        public Deduplicate() {

        }

        //private void IdentifyDuplicates(IEnumerable<string> source, IEnumerable<string> saver, object sortedKey, object otherKey) {
        //    DataRow example = null;
        //    var buffer = new List<DataRow>();
        //    example = source.getNextRow();
        //    buffer.Add(example);
        //    while (true) {
        //        if (example == null) {
        //            break;
        //        }
        //        buffer.add(example);
        //        DataRow row;
        //        while (IsTheSameKey(row = source.getNextRow(), example, sortedKey) && row != null) {
        //            buffer.add(row);
        //        }
        //        solveGroup(saver, buffer, sortedKey, otherKey);
        //        buffer.clear();
        //        example = row;
        //    }
        //}

        //private bool IsTheSameKey(DataRow r1, DataRow r2, DataColumnDefinition[] sortedKey) {
        //    if (r1 == null || r2 == null) {
        //        return false;
        //    }

        //    for (int i = 0; i < sortedKey.Length; i++) {
        //        if (!r1.GetData(sortedKey[i]).Value.Equals(r2.GetData(sortedKey[i]).Value)) {
        //            return false;
        //        }
        //    }

        //    return true;
        //}
    }
}
