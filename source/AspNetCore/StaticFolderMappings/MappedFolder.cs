namespace FFCEI.Microservices.AspNetCore.StaticFolderMappings
{
    internal sealed class MappedFolder : IMappedFolder
    {
        public string WebPath { get; set; } = string.Empty;

        public string PhysicalPath { get; set; } = string.Empty;

        public bool DirectoryBrowsing { get; set; }

        public StaticFolderMappingAuthorizationPolicy AuthorizationPolicy { get; set; } = StaticFolderMappingAuthorizationPolicy.PublicAccess;

        public HashSet<string>? AuthorizedRoles { get; set; }

        IReadOnlySet<string>? IMappedFolder.AuthorizedRoles => AuthorizedRoles;
    }
}
