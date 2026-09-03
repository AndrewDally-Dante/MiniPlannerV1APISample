namespace DanteAPI
{
    public class ValidateKeyResult
    {
        public bool Valid { get; set; }
        public List<string> Messages { get; set; }
        public List<ValidateKeyPermission> Permisison { get; set; }
    }

    public class ValidateKeyPermission
    {
        public string Context { get; set; }
        public int PermissionsValue { get; set; }
        public List<string> PermissionsList { get; set; }
    }
}
