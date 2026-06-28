using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class RSA : AsymmetricAlgorithm
	{
		public new static RSA Create()
		{
			return RSA.Create("System.Security.Cryptography.RSA");
		}

		public new static RSA Create(string algName)
		{
			return (RSA)CryptoConfig.CreateFromName(algName);
		}

		public abstract byte[] EncryptValue(byte[] rgb);

		public abstract byte[] DecryptValue(byte[] rgb);

		public abstract RSAParameters ExportParameters(bool includePrivateParameters);

		public abstract void ImportParameters(RSAParameters parameters);

		internal void ZeroizePrivateKey(RSAParameters parameters)
		{
			if (parameters.P != null)
			{
				Array.Clear(parameters.P, 0, parameters.P.Length);
			}
			if (parameters.Q != null)
			{
				Array.Clear(parameters.Q, 0, parameters.Q.Length);
			}
			if (parameters.DP != null)
			{
				Array.Clear(parameters.DP, 0, parameters.DP.Length);
			}
			if (parameters.DQ != null)
			{
				Array.Clear(parameters.DQ, 0, parameters.DQ.Length);
			}
			if (parameters.InverseQ != null)
			{
				Array.Clear(parameters.InverseQ, 0, parameters.InverseQ.Length);
			}
			if (parameters.D != null)
			{
				Array.Clear(parameters.D, 0, parameters.D.Length);
			}
		}

		public override void FromXmlString(string xmlString)
		{
			if (xmlString == null)
			{
				throw new ArgumentNullException("xmlString");
			}
			RSAParameters rsaparameters = default(RSAParameters);
			try
			{
				rsaparameters.P = AsymmetricAlgorithm.GetNamedParam(xmlString, "P");
				rsaparameters.Q = AsymmetricAlgorithm.GetNamedParam(xmlString, "Q");
				rsaparameters.D = AsymmetricAlgorithm.GetNamedParam(xmlString, "D");
				rsaparameters.DP = AsymmetricAlgorithm.GetNamedParam(xmlString, "DP");
				rsaparameters.DQ = AsymmetricAlgorithm.GetNamedParam(xmlString, "DQ");
				rsaparameters.InverseQ = AsymmetricAlgorithm.GetNamedParam(xmlString, "InverseQ");
				rsaparameters.Exponent = AsymmetricAlgorithm.GetNamedParam(xmlString, "Exponent");
				rsaparameters.Modulus = AsymmetricAlgorithm.GetNamedParam(xmlString, "Modulus");
				this.ImportParameters(rsaparameters);
			}
			catch (Exception ex)
			{
				this.ZeroizePrivateKey(rsaparameters);
				throw new CryptographicException(Locale.GetText("Couldn't decode XML"), ex);
			}
			finally
			{
				this.ZeroizePrivateKey(rsaparameters);
			}
		}

		public override string ToXmlString(bool includePrivateParameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			RSAParameters rsaparameters = this.ExportParameters(includePrivateParameters);
			try
			{
				stringBuilder.Append("<RSAKeyValue>");
				stringBuilder.Append("<Modulus>");
				stringBuilder.Append(Convert.ToBase64String(rsaparameters.Modulus));
				stringBuilder.Append("</Modulus>");
				stringBuilder.Append("<Exponent>");
				stringBuilder.Append(Convert.ToBase64String(rsaparameters.Exponent));
				stringBuilder.Append("</Exponent>");
				if (includePrivateParameters)
				{
					if (rsaparameters.D == null)
					{
						string text = Locale.GetText("Missing D parameter for the private key.");
						throw new ArgumentNullException(text);
					}
					if (rsaparameters.P == null || rsaparameters.Q == null || rsaparameters.DP == null || rsaparameters.DQ == null || rsaparameters.InverseQ == null)
					{
						string text2 = Locale.GetText("Missing some CRT parameters for the private key.");
						throw new CryptographicException(text2);
					}
					stringBuilder.Append("<P>");
					stringBuilder.Append(Convert.ToBase64String(rsaparameters.P));
					stringBuilder.Append("</P>");
					stringBuilder.Append("<Q>");
					stringBuilder.Append(Convert.ToBase64String(rsaparameters.Q));
					stringBuilder.Append("</Q>");
					stringBuilder.Append("<DP>");
					stringBuilder.Append(Convert.ToBase64String(rsaparameters.DP));
					stringBuilder.Append("</DP>");
					stringBuilder.Append("<DQ>");
					stringBuilder.Append(Convert.ToBase64String(rsaparameters.DQ));
					stringBuilder.Append("</DQ>");
					stringBuilder.Append("<InverseQ>");
					stringBuilder.Append(Convert.ToBase64String(rsaparameters.InverseQ));
					stringBuilder.Append("</InverseQ>");
					stringBuilder.Append("<D>");
					stringBuilder.Append(Convert.ToBase64String(rsaparameters.D));
					stringBuilder.Append("</D>");
				}
				stringBuilder.Append("</RSAKeyValue>");
			}
			catch
			{
				this.ZeroizePrivateKey(rsaparameters);
				throw;
			}
			return stringBuilder.ToString();
		}
	}
}
