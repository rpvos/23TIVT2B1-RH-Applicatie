using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FietsDemo
{
    public sealed class DataSingleton
    {
        private static readonly Lazy<DataSingleton>
            lazy =
            new Lazy<DataSingleton>
                (() => new DataSingleton());

        public static DataSingleton Instance { get { return lazy.Value; } }

        private DataSingleton()
        {
        }
    }

}
