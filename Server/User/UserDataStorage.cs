using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class UserDataStorage
    {
        public List<DataSet> dataSets { get; set; }

        private Dictionary<ValueType, DateTime> lastUpdate;

        public UserDataStorage()
        {
            this.dataSets = new List<DataSet>();
            this.lastUpdate = new Dictionary<ValueType, DateTime>();
        }

        /// <summary>
        /// Method to add the updates to the historical data
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="value"></param>
        public void addDataSet(ValueType valueType, double value)
        {
            DateTime lastUpdateForThisValueType;
            // If the valuetype has a value
            if(lastUpdate.TryGetValue(valueType,out lastUpdateForThisValueType))
            {
                // Check if the lastUpdateForThisValueType is longer then a second ago
                if (DateTime.Now.CompareTo(lastUpdateForThisValueType.AddSeconds(1)) >= 0)
                {
                    // The lastUpdateForThisValueType is longer ago or equal to 1 second
                    lastUpdate.Add(valueType, DateTime.Now);

                    // Add it to the historical data
                    this.dataSets.Add(new DataSet(valueType, value, DateTime.Now));
                }
            }
            else
            {
                // ValueType has not been added yet
                lastUpdate.Add(valueType, DateTime.Now);

                // Add it to the historical data
                this.dataSets.Add(new DataSet(valueType, value, DateTime.Now));
            }

        }
    }

    public class DataSet
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
