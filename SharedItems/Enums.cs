using System;
using System.Collections.Generic;
using System.Text;

namespace SharedItems
{
    public enum UpdateType
    {
        Heartrate,
        Speed,
        AccumulatedPower,
        InstantaniousPower,
        AccumulatedDistance,
        ElapsedTime,
        Resistance
    }

    public enum Role
    {
        Doctor,
        Patient,
        Invallid
    }
}
