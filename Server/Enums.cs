using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public enum ValueType
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
