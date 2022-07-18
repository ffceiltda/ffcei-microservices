namespace FFCEI.Microservices.AspNetCore.StaticFiles
{
    /// <summary>
    /// Web Api static folder mapping
    /// </summary>
    public sealed class FolderMapping
    {
        /// <summary>
        /// HTTP path
        /// </summary>
        public string WebPath { get; set; } = string.Empty;

        /// <summary>
        /// Physical path
        /// </summary>
        public string PhysicalPath { get; set; } = string.Empty;

        /// <summary>
        /// Enables directory browsing
        /// </summary>
        public bool DirectoryBrowsing { get; set; }

        /// <summary>
        /// Authorization policy
        /// </summary>
        public StaticFolderMappingAuthorizationPolicy AuthorizationPolicy { get; set; } = StaticFolderMappingAuthorizationPolicy.PublicAccess;

        /// <summary>
        /// Authorized roles (if applies)
        /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
        public HashSet<string>? AuthorizedRoles { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
