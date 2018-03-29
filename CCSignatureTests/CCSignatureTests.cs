using System;
using CCSignature;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CCSignatureTests
{
	[TestClass]
	public class SignTests
	{
		[TestMethod]
		public void SignTest()
		{
			var certificateManager = new CertificateManager();
			var availableCertificates = certificateManager.GetStoreCertificates();
			Assert.IsNotNull(availableCertificates);
			Assert.AreNotEqual(0, availableCertificates.Count);

			var selectedCertificate = availableCertificates[0];
			var docToSign = "My document to sign";

			var base64Signature = certificateManager.Sign(selectedCertificate.Id, docToSign);
			Assert.IsNotNull(base64Signature);

			Console.WriteLine("Base 64 signature:\n{0}", base64Signature);
		}
	}
}
