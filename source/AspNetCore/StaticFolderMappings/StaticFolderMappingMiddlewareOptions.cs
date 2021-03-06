namespace FFCEI.Microservices.AspNetCore.StaticFolderMappings
{
    /// <summary>
    /// Web Api static folder mapping options
    /// </summary>
    public sealed class StaticFolderMappingMiddlewareOptions
    {
        internal readonly SortedDictionary<string, MappedFolder> MappedFolders = new();

        /// <summary>
        /// Has no mappings defined
        /// </summary>
        public bool Empty => MappedFolders.Count < 1;

        /// <summary>
        /// Add a Static Folder Mapping, and apply authorization policies
        /// </summary>
        /// <param name="webPath">HTTP path</param>
        /// <param name="physicalPath">Physical operating system path</param>
        /// <param name="directoryBrowsing">Enables directory browsing</param>
        /// <param name="authorizationPolicy">Authorization policy</param>
        /// <param name="authorizedRoles">Authorized roles (if applies)</param>
        /// <exception cref="InvalidOperationException">Throws if webPath is already mapped</exception>
        public IMappedFolder Map(string webPath, string physicalPath, bool directoryBrowsing = false,
            StaticFolderMappingAuthorizationPolicy authorizationPolicy = StaticFolderMappingAuthorizationPolicy.PublicAccess,
            IEnumerable<string>? authorizedRoles = null)
        {
            if (webPath is null)
            {
                throw new ArgumentNullException(nameof(webPath));
            }

            if (physicalPath is null)
            {
                throw new ArgumentNullException(nameof(physicalPath));
            }

            if (!Directory.Exists(physicalPath))
            {
                throw new InvalidOperationException($"Directory {physicalPath} cannot be accessed or does not exists");
            }

            while (webPath.StartsWith("/", StringComparison.InvariantCulture))
            {
                webPath = webPath[1..];
            }

            while (webPath.EndsWith("/", StringComparison.InvariantCulture))
            {
                webPath = webPath[..^1];
            }

            if (MappedFolders.ContainsKey(webPath))
            {
                throw new InvalidOperationException($"WebApi Static Path {webPath} already configured");
            }

            var mappedFolder = new MappedFolder()
            {
                WebPath = $"/{webPath}/",
                PhysicalPath = physicalPath,
                DirectoryBrowsing = directoryBrowsing,
                AuthorizationPolicy = authorizationPolicy,
                AuthorizedRoles = authorizedRoles?.ToHashSet()
            };

            MappedFolders.Add(webPath, mappedFolder);

            return mappedFolder;
        }
    }
}
