using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using MemberSuite.SDK.Utilities;


public static class DigitalSignatureManager
{
    #region Private Static Fields

    private static readonly string CertificateContainer;
    private static readonly string PortalCertificateSubject;

    #endregion

    static DigitalSignatureManager()
    {
        //Load digital signature certificate settings
        CertificateContainer = ConfigurationManager.AppSettings["CertificatesStoreName"];
        PortalCertificateSubject = ConfigurationManager.AppSettings["PortalCertificateSubject"];
    }

    #region Private Static Methods



    /// <summary>
    /// Retrieves the X509 Certificate for the console from the Local Machine certificate store using configured settings
    /// </summary>
    /// <returns></returns>
    private static X509Certificate2 GetPortalCertificate()
    {
        return CryptoManager.GetCertificateByStoreAndSubject(PortalCertificateSubject, CertificateContainer);
    }


    #endregion

    #region Internal Static Methods

    public static byte[] Sign(byte[] data)
    {
        X509Certificate2 consoleCertificate = GetPortalCertificate();
        return CryptoManager.Sign(data, consoleCertificate.PrivateKey);
    }

    #endregion
}
