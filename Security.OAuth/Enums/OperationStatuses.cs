namespace FrostAura.Libraries.Security.OAuth.Enums
{
    /// <summary>
    /// Possible operation statuses.
    /// </summary>
    public enum OperationStatus
    {
        Idle,
        ConcentUrlGenerated,
        AuthTokenRetrieving,
        AuthTokenRetrieved,
        ProfileInformationFetching,
        ProfileInformationFetched,
        Error
    }
}