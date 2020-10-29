namespace SharedItems
{
    /// <summary>
    /// The type of update
    /// </summary>
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

    /// <summary>
    /// Roles that clients can have
    /// </summary>
    public enum Role
    {
        Doctor,
        Patient,
        Invallid
    }
}
