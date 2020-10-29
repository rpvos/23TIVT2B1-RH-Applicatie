using System;

namespace SharedItems
{
    /// <summary>
    /// Class that stores the update data
    /// </summary>
    public class DataSet
    {
        public UpdateType UpdateType { get; }
        public double Value { get; }
        public DateTime DateStamp { get; }
        public DataSet(UpdateType updateType, double value, DateTime dateStamp)
        {
            UpdateType = updateType;
            Value = value;
            DateStamp = dateStamp;
        }
    }
}
