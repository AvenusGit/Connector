using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
namespace ConnectorCenter.Services.Certificates
{
    public class CertificateGenerator
    {
        public X509Certificate2 GenerateSelfSignedCert(string domainName)
        {
            RSA rsaKey = RSA.Create(2048);
            string subject = $"CN={domainName}";

            CertificateRequest certificateRequest = new CertificateRequest(
                subject,
                rsaKey,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );
            certificateRequest.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(
                    certificateAuthority: false,
                    hasPathLengthConstraint: false,
                    pathLengthConstraint: 0,
                    critical: true
                )
            );
            certificateRequest.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    keyUsages:
                        X509KeyUsageFlags.DigitalSignature
                        | X509KeyUsageFlags.KeyEncipherment,
                    critical: false
                )
            );
            certificateRequest.CertificateExtensions.Add(
                new X509SubjectKeyIdentifierExtension(
                    key: certificateRequest.PublicKey,
                    critical: false
                )
            );
            if(domainName == "localhost")
                certificateRequest.CertificateExtensions.Add(
                    new X509Extension(
                        new AsnEncodedData(
                            "Subject Alternative Name",
                            new byte[] { 48, 11, 130, 9, 108, 111, 99, 97, 108, 104, 111, 115, 116 }
                        ),
                        false
                    )
                );
            DateTimeOffset expireAt = DateTimeOffset.Now.AddYears(5);
            return certificateRequest.CreateSelfSigned(DateTimeOffset.Now, expireAt);
        }
    }
}
