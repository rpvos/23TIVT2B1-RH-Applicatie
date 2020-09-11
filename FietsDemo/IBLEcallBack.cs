using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FietsDemo
{
    public interface IBLEcallBack
    {
        void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e);
    }
}
