using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Mono.Xml;

namespace Mono.Security.Cryptography
{
	public abstract class DiffieHellman : AsymmetricAlgorithm
	{
		public new static DiffieHellman Create()
		{
			return DiffieHellman.Create("Mono.Security.Cryptography.DiffieHellman");
		}

		public new static DiffieHellman Create(string algName)
		{
			return (DiffieHellman)CryptoConfig.CreateFromName(algName);
		}

		public abstract byte[] CreateKeyExchange();

		public abstract byte[] DecryptKeyExchange(byte[] keyex);

		public abstract DHParameters ExportParameters(bool includePrivate);

		public abstract void ImportParameters(DHParameters parameters);

		private byte[] GetNamedParam(SecurityElement se, string param)
		{
			SecurityElement securityElement = se.SearchForChildByTag(param);
			if (securityElement == null)
			{
				return null;
			}
			return Convert.FromBase64String(securityElement.Text);
		}

		public override void FromXmlString(string xmlString)
		{
			if (xmlString == null)
			{
				throw new ArgumentNullException("xmlString");
			}
			DHParameters dhparameters = default(DHParameters);
			try
			{
				SecurityParser securityParser = new SecurityParser();
				securityParser.LoadXml(xmlString);
				SecurityElement securityElement = securityParser.ToXml();
				if (securityElement.Tag != "DHKeyValue")
				{
					throw new CryptographicException();
				}
				dhparameters.P = this.GetNamedParam(securityElement, "P");
				dhparameters.G = this.GetNamedParam(securityElement, "G");
				dhparameters.X = this.GetNamedParam(securityElement, "X");
				this.ImportParameters(dhparameters);
			}
			finally
			{
				if (dhparameters.P != null)
				{
					Array.Clear(dhparameters.P, 0, dhparameters.P.Length);
				}
				if (dhparameters.G != null)
				{
					Array.Clear(dhparameters.G, 0, dhparameters.G.Length);
				}
				if (dhparameters.X != null)
				{
					Array.Clear(dhparameters.X, 0, dhparameters.X.Length);
				}
			}
		}

		public override string ToXmlString(bool includePrivateParameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			DHParameters dhparameters = this.ExportParameters(includePrivateParameters);
			try
			{
				stringBuilder.Append("<DHKeyValue>");
				stringBuilder.Append("<P>");
				stringBuilder.Append(Convert.ToBase64String(dhparameters.P));
				stringBuilder.Append("</P>");
				stringBuilder.Append("<G>");
				stringBuilder.Append(Convert.ToBase64String(dhparameters.G));
				stringBuilder.Append("</G>");
				if (includePrivateParameters)
				{
					stringBuilder.Append("<X>");
					stringBuilder.Append(Convert.ToBase64String(dhparameters.X));
					stringBuilder.Append("</X>");
				}
				stringBuilder.Append("</DHKeyValue>");
			}
			finally
			{
				Array.Clear(dhparameters.P, 0, dhparameters.P.Length);
				Array.Clear(dhparameters.G, 0, dhparameters.G.Length);
				if (dhparameters.X != null)
				{
					Array.Clear(dhparameters.X, 0, dhparameters.X.Length);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
