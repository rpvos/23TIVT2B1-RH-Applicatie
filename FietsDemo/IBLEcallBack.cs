using Avans.TI.BLE;

namespace FietsDemo
{
    public interface IBLEcallBack
    {
        void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e);
    }
}
