namespace FFCEI.Microservices.AspNetCore.StaticFolderMappings
{
    /// <summary>
    /// Web Api static folder mapping interface
    /// </summary>
    public interface IMappedFolder
    {
        /// <summary>
        /// HTTP path
        /// </summary>
        string WebPath { get; }

        /// <summary>
        /// Physical path
        /// </summary>
        string PhysicalPath { get; }

        /// <summary>
        /// Enables directory browsing
        /// </summary>
        bool DirectoryBrowsing { get; }

        /// <summary>
        /// Authorization policy
        /// </summary>
        StaticFolderMappingAuthorizationPolicy AuthorizationPolicy { get; }

        /// <summary>
        /// Authorized roles (if applies)
        /// </summary>
        IReadOnlySet<string>? AuthorizedRoles { get; }
    }
}
