using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class UserDataStorage
    {
        public List<DataSet> dataSets { get; }
        public UserDataStorage()
        {
            this.dataSets = new List<DataSet>();
        }
        public void addDataSet(ValueType valueType, double value)
        {
            this.dataSets.Add(new DataSet(valueType, value, DateTime.Now));
        }
    }

    public struct DataSet
    {
        public ValueType ValueType { get; }
        public double Value { get; }
        public DateTime DateStamp { get; }
        public DataSet(ValueType valueType, double value, DateTime dateStamp)
        {
            ValueType = valueType;
            Value = value;
            DateStamp = dateStamp;
        }
    }
}
