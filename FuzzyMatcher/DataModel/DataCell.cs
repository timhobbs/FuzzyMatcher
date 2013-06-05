using System;

namespace FuzzyMatcher.DataModel {

    public class DataCell {

        public object Value { get; set; }

        public int Type { get; set; }

        public DataCell(int type, Object value) {
            this.Type = type;
            this.Value = value;
        }

        //public int CompareTo(object o) {
        //    DataCell data = (DataCell)o;
        //    if (this.Type != data.Type) {
        //        throw new ArgumentException("Compared DataCells have to be the same type.");
        //    }

        //    if (this.Type == DataColumnDefinition.TYPE_NUMERIC) {
        //        double n1 = (double)this.Value;
        //        double n2 = (double)data.Value;
        //        if (n1 > n2) {
        //            return 1;
        //        } else if (n1 < n2) {
        //            return -1;
        //        } else {
        //            return 0;
        //        }
        //    } else if (this.Type == DataColumnDefinition.TYPE_STRING) {
        //        return ((string)this.Value).ToLower()
        //            .CompareTo(((string)data.Value).ToLower());
        //    } else {
        //        throw new ArgumentException("Not supported currently!");
        //    }
        //}

        //public bool Equals(object obj) {
        //    DataCell data = obj as DataCell;

        //    if (data == null) {
        //        return false;
        //    } else {
        //        return this.Value.Equals(data.Value);
        //    }
        //}

        //public int HashCode() {
        //    return this.Value.GetHashCode();
        //}

        //public override string ToString() {
        //    return String.Format("Cell[value='{0}']", this.Value);
        //}

        //public bool IsEmpty(DataColumnDefinition columnType) {
        //    if (String.IsNullOrEmpty((this.Value ?? String.Empty).ToString())) {
        //        return true;
        //    }

        //    string[] emptys = columnType.EmptyValues;
        //    if (emptys != null) {
        //        for (int i = 0; i < emptys.Length; i++) {
        //            if (this.Value.ToString().Equals(emptys[i])) {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}
    }
}