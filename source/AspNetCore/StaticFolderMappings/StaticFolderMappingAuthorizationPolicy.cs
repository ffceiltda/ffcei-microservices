namespace FFCEI.Microservices.AspNetCore.StaticFolderMappings
{
    /// <summary>
    /// Static folder mapping authorization policy
    /// </summary>
    public enum StaticFolderMappingAuthorizationPolicy
    {
        /// <summary>
        /// (default) Public read access from everyone
        /// </summary>
        PublicAccess,
        /// <summary>
        /// Require user to be authenticated
        /// </summary>
        AuthenticatedAccess,
        /// <summary>
        /// Require user to be authenticated and has role assigned
        /// </summary>
        AuthorizedRoles
    }
}
