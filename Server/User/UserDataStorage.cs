using SharedItems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class UserDataStorage
    {
        public List<DataSet> dataSets { get; set; }

        private Dictionary<UpdateType, DateTime> lastUpdate;

        public UserDataStorage()
        {
            this.dataSets = new List<DataSet>();
            this.lastUpdate = new Dictionary<UpdateType, DateTime>();
        }

        /// <summary>
        /// Method to add the updates to the historical data
        /// </summary>
        /// <param name="updateType"></param>
        /// <param name="value"></param>
        public void addDataSet(UpdateType updateType, double value)
        {
            DateTime lastUpdateForThisUpdateType;
            // If the updatetype has a value
            if(lastUpdate.TryGetValue(updateType,out lastUpdateForThisUpdateType))
            {
                // Check if the lastUpdateForThisUpdateType is longer then a second ago
                if (DateTime.Now.CompareTo(lastUpdateForThisUpdateType.AddSeconds(1)) >= 0)
                {
                    lastUpdate.Remove(updateType);
                    // The lastUpdateForThisUpdateType is longer ago or equal to 1 second
                    lastUpdate.Add(updateType, DateTime.Now);

                    // Add it to the historical data
                    this.dataSets.Add(new DataSet(updateType, value, DateTime.Now));
                }
            }
            else
            {
                // Update has not been added yet
                lastUpdate.Add(updateType, DateTime.Now);

                // Add it to the historical data
                this.dataSets.Add(new DataSet(updateType, value, DateTime.Now));
            }

        }
    }

   
}
