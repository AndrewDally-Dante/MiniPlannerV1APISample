namespace DanteAPI.Lookups
{
    [Flags]
    public enum ResourceTypes
    {
        Venue = 1,
        Tutor_Internal = 2,
        Tutor_External = 4,
        Assessor = 8,
        Verifier = 16,
        Other = 32,
        Equipment = 64,
        Machinery = 128,
        Training_Provider = 256
    }
}