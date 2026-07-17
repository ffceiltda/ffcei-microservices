using FFCEI.Microservices.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace FFCEI.Microservices.Security;

/// <summary>
/// Base security key factory implementation
/// </summary>
public class SecurityKeyFactory
{
    private readonly SecurityKey? _securityKey;
    private readonly bool _securityKeyIsAsymmetric;
    private readonly bool _securityKeyIsX509;
    private readonly SigningCredentials? _signingCredentials;
    private readonly EncryptingCredentials? _encryptingCredentials;

    /// <summary>
    /// Security key
    /// </summary>
    public SecurityKey? SecurityKey => _securityKey;

    /// <summary>
    /// Security key is asymmetric?
    /// </summary>
    public bool SecurityKeyIsAsymmetric => _securityKeyIsAsymmetric;

    /// <summary>
    /// Security key is based on x.509 certificate?
    /// </summary>
    public bool SecurityKeyIsX509 => _securityKeyIsX509;

    /// <summary>
    /// Signing credentials
    /// </summary>
    public SigningCredentials? SigningCredentials => _signingCredentials;

    /// <summary>
    /// Encrypt credentials
    /// </summary>
    public EncryptingCredentials? EncryptingCredentials => _encryptingCredentials;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="configurationManager">ConfigurationManager</param>
    /// <param name="configurationKeyStringPrefix">Configuration key string prefix</param>
    /// <param name="logger">Logger</param>
    /// <exception cref="ArgumentNullException">throw is configurationManager is null of configurationKeyStringPrefis is null or empty</exception>
    public SecurityKeyFactory(IConfigurationManager configurationManager, string configurationKeyStringPrefix, ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(configurationManager, nameof(configurationManager));
        ArgumentNullException.ThrowIfNull(configurationKeyStringPrefix, nameof(configurationKeyStringPrefix));

#pragma warning disable CA1031 // Do not catch general exception types
        if (_securityKey is null)
        {
            var x509certificateFilename = configurationManager[$"{configurationKeyStringPrefix}X509Certificate.Filename"];
            var x509certificatePassword = configurationManager[$"{configurationKeyStringPrefix}X509Certificate.Password"];
            var x509certificateKeyFilename = configurationManager[$"{configurationKeyStringPrefix}X509Certificate.KeyFilename"];

            if (string.IsNullOrWhiteSpace(x509certificateKeyFilename))
            {
                x509certificateKeyFilename = null;
            }

            if (!string.IsNullOrWhiteSpace(x509certificateFilename) && File.Exists(x509certificateFilename))
            {
                try
                {
                    var x509certificateFilenameExtension = Path.GetExtension(x509certificateFilename).ToUpperInvariant();
                    var x509certificateFilenameIsPK = x509certificateFilenameExtension.EndsWith("P12", StringComparison.InvariantCulture) ||
                        x509certificateFilenameExtension.EndsWith("PFX", StringComparison.InvariantCulture);
                    var x509certificateFilenameIsPEM = x509certificateFilenameExtension.EndsWith("PEM", StringComparison.InvariantCulture);

                    using var certificate =
                        x509certificateFilenameIsPK ? X509CertificateLoader.LoadPkcs12FromFile(x509certificateFilename, x509certificatePassword) :
                        !x509certificateFilenameIsPEM ? X509CertificateLoader.LoadCertificateFromFile(x509certificateFilename) :
                            string.IsNullOrWhiteSpace(x509certificatePassword) ?
                                X509Certificate2.CreateFromPemFile(x509certificateFilename, x509certificateKeyFilename) :
                                X509Certificate2.CreateFromEncryptedPemFile(x509certificateFilename, x509certificatePassword, x509certificateKeyFilename);

                    _securityKey = new X509SecurityKey(certificate);
                    _signingCredentials = new X509SigningCredentials(certificate);
                    _securityKeyIsAsymmetric = true;
                    _securityKeyIsX509 = true;
                }
                catch (CryptographicException e)
                {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
                    logger?.LogError(e, "Exception loading certificate {X509Certificatefilename}", x509certificateFilename);
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                }
            }
        }

        if (_securityKey is null)
        {
            var certificateList = new List<X509Certificate2>();

            foreach (StoreLocation storeLocation in Enum.GetValues<StoreLocation>())
            {
                foreach (StoreName storeName in Enum.GetValues<StoreName>())
                {
                    try
                    {
                        using var store = new X509Store(storeName, storeLocation);

                        store.Open(OpenFlags.OpenExistingOnly);

                        try
                        {
                            var x509certificateSubject = configurationManager[$"{configurationKeyStringPrefix}X509Certificate.Subject"];

                            if (!string.IsNullOrWhiteSpace(x509certificateSubject))
                            {
                                var certificateCollection = store.Certificates.Find(X509FindType.FindBySubjectName, x509certificateSubject, true);

                                if (certificateCollection is { Count: > 0 })
                                {
                                    foreach (var certificateItem in certificateCollection)
                                    {
                                        certificateList.Add(certificateItem);
                                    }
                                }
                            }

                            var x509certificateSubjectKeyIdentifier = configurationManager[$"{configurationKeyStringPrefix}X509Certificate.SubjectKeyIdentifier"];

                            if (!string.IsNullOrWhiteSpace(x509certificateSubjectKeyIdentifier))
                            {
                                var certificateCollection = store.Certificates.Find(X509FindType.FindBySubjectKeyIdentifier, x509certificateSubjectKeyIdentifier, true);

                                if (certificateCollection is { Count: > 0 })
                                {
                                    foreach (var certificateItem in certificateCollection)
                                    {
                                        certificateList.Add(certificateItem);
                                    }
                                }
                            }

                            var x509certificateSubjectDistinguishedName = configurationManager[$"{configurationKeyStringPrefix}X509Certificate.SubjectDistinguishedName"];

                            if (!string.IsNullOrWhiteSpace(x509certificateSubjectDistinguishedName))
                            {
                                var certificateCollection = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, x509certificateSubjectDistinguishedName, true);

                                if (certificateCollection is { Count: > 0 })
                                {
                                    foreach (var certificateItem in certificateCollection)
                                    {
                                        certificateList.Add(certificateItem);
                                    }
                                }
                            }

                            var x509certificateSerialNumber = configurationManager[$"{configurationKeyStringPrefix}X509Certificate.SerialNumber"];

                            if (!string.IsNullOrWhiteSpace(x509certificateSerialNumber))
                            {
                                var certificateCollection = store.Certificates.Find(X509FindType.FindBySerialNumber, x509certificateSerialNumber, true);

                                if (certificateCollection is { Count: > 0 })
                                {
                                    foreach (var certificateItem in certificateCollection)
                                    {
                                        certificateList.Add(certificateItem);
                                    }
                                }
                            }
                        }
                        finally
                        {
                            store.Close();
                        }
                    }
                    catch (Exception e)
                    {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
                        logger?.LogError(e, "Exception loading certificate asymmetric key");
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                    }
                }
            }

            var certificate = certificateList.Where(c => c.NotAfter >= DateTimeOffset.UtcNow.LocalDateTime).OrderByDescending(c => c.NotBefore).FirstOrDefault();

            if (certificate is not null)
            {
                _securityKey = new X509SecurityKey(certificate);
                _securityKeyIsAsymmetric = true;
                _securityKeyIsX509 = true;
                _signingCredentials = new X509SigningCredentials(certificate);
                _encryptingCredentials = new X509EncryptingCredentials(certificate);
            }
        }
#pragma warning restore CA1031 // Do not catch general exception types

        if (_securityKey is null)
        {
            var symmetricSecurityKey = configurationManager[$"{configurationKeyStringPrefix}Symmetric.Key"];
            var symmetricSecuritySignatureAlgorithm = configurationManager[$"{configurationKeyStringPrefix}Symmetric.SignatureAlgorithm"];
            var symmetricEncryptionKeyWrapAlgorithm = configurationManager[$"{configurationKeyStringPrefix}Symmetric.EncryptionKeyWrapAlgorithm"];
            var symmetricEncryptionAlgorithm = configurationManager[$"{configurationKeyStringPrefix}Symmetric.EncryptionAlgorithm"];

            if (!string.IsNullOrWhiteSpace(symmetricSecurityKey))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(symmetricSecuritySignatureAlgorithm))
                    {
                        symmetricSecuritySignatureAlgorithm = SecurityAlgorithms.HmacSha512;
                    }

                    if (string.IsNullOrWhiteSpace(symmetricEncryptionKeyWrapAlgorithm))
                    {
                        symmetricEncryptionKeyWrapAlgorithm = SecurityAlgorithms.Aes256KW;
                    }

                    if (string.IsNullOrWhiteSpace(symmetricEncryptionAlgorithm))
                    {
                        symmetricEncryptionAlgorithm = SecurityAlgorithms.Aes256CbcHmacSha512;
                    }

                    _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symmetricSecurityKey));
                    _securityKeyIsAsymmetric = false;
                    _securityKeyIsX509 = false;
                    _signingCredentials = new SigningCredentials(_securityKey, symmetricSecuritySignatureAlgorithm);
                    _encryptingCredentials = new EncryptingCredentials(_securityKey, symmetricEncryptionKeyWrapAlgorithm, symmetricEncryptionAlgorithm);
                }
                catch (CryptographicException e)
                {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
                    logger?.LogError(e, "Exception loading symmetric key");
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                }
            }
        }
    }
}

