using System;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Security;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class DSA : AsymmetricAlgorithm
	{
		public new static DSA Create()
		{
			return DSA.Create("System.Security.Cryptography.DSA");
		}

		public new static DSA Create(string algName)
		{
			return (DSA)CryptoConfig.CreateFromName(algName);
		}

		public abstract byte[] CreateSignature(byte[] rgbHash);

		public abstract DSAParameters ExportParameters(bool includePrivateParameters);

		internal void ZeroizePrivateKey(DSAParameters parameters)
		{
			if (parameters.X != null)
			{
				Array.Clear(parameters.X, 0, parameters.X.Length);
			}
		}

		public override void FromXmlString(string xmlString)
		{
			if (xmlString == null)
			{
				throw new ArgumentNullException("xmlString");
			}
			DSAParameters dsaparameters = default(DSAParameters);
			try
			{
				dsaparameters.P = AsymmetricAlgorithm.GetNamedParam(xmlString, "P");
				dsaparameters.Q = AsymmetricAlgorithm.GetNamedParam(xmlString, "Q");
				dsaparameters.G = AsymmetricAlgorithm.GetNamedParam(xmlString, "G");
				dsaparameters.J = AsymmetricAlgorithm.GetNamedParam(xmlString, "J");
				dsaparameters.Y = AsymmetricAlgorithm.GetNamedParam(xmlString, "Y");
				dsaparameters.X = AsymmetricAlgorithm.GetNamedParam(xmlString, "X");
				dsaparameters.Seed = AsymmetricAlgorithm.GetNamedParam(xmlString, "Seed");
				byte[] namedParam = AsymmetricAlgorithm.GetNamedParam(xmlString, "PgenCounter");
				if (namedParam != null)
				{
					byte[] array = new byte[4];
					Buffer.BlockCopy(namedParam, 0, array, 0, namedParam.Length);
					dsaparameters.Counter = BitConverterLE.ToInt32(array, 0);
				}
				this.ImportParameters(dsaparameters);
			}
			catch
			{
				this.ZeroizePrivateKey(dsaparameters);
				throw;
			}
			finally
			{
				this.ZeroizePrivateKey(dsaparameters);
			}
		}

		public abstract void ImportParameters(DSAParameters parameters);

		public override string ToXmlString(bool includePrivateParameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			DSAParameters dsaparameters = this.ExportParameters(includePrivateParameters);
			try
			{
				stringBuilder.Append("<DSAKeyValue>");
				stringBuilder.Append("<P>");
				stringBuilder.Append(Convert.ToBase64String(dsaparameters.P));
				stringBuilder.Append("</P>");
				stringBuilder.Append("<Q>");
				stringBuilder.Append(Convert.ToBase64String(dsaparameters.Q));
				stringBuilder.Append("</Q>");
				stringBuilder.Append("<G>");
				stringBuilder.Append(Convert.ToBase64String(dsaparameters.G));
				stringBuilder.Append("</G>");
				stringBuilder.Append("<Y>");
				stringBuilder.Append(Convert.ToBase64String(dsaparameters.Y));
				stringBuilder.Append("</Y>");
				if (dsaparameters.J != null)
				{
					stringBuilder.Append("<J>");
					stringBuilder.Append(Convert.ToBase64String(dsaparameters.J));
					stringBuilder.Append("</J>");
				}
				if (dsaparameters.Seed != null)
				{
					stringBuilder.Append("<Seed>");
					stringBuilder.Append(Convert.ToBase64String(dsaparameters.Seed));
					stringBuilder.Append("</Seed>");
					stringBuilder.Append("<PgenCounter>");
					if (dsaparameters.Counter != 0)
					{
						byte[] bytes = BitConverterLE.GetBytes(dsaparameters.Counter);
						int num = bytes.Length;
						while (bytes[num - 1] == 0)
						{
							num--;
						}
						stringBuilder.Append(Convert.ToBase64String(bytes, 0, num));
					}
					else
					{
						stringBuilder.Append("AA==");
					}
					stringBuilder.Append("</PgenCounter>");
				}
				if (dsaparameters.X != null)
				{
					stringBuilder.Append("<X>");
					stringBuilder.Append(Convert.ToBase64String(dsaparameters.X));
					stringBuilder.Append("</X>");
				}
				else if (includePrivateParameters)
				{
					throw new ArgumentNullException("X");
				}
				stringBuilder.Append("</DSAKeyValue>");
			}
			catch
			{
				this.ZeroizePrivateKey(dsaparameters);
				throw;
			}
			return stringBuilder.ToString();
		}

		public abstract bool VerifySignature(byte[] rgbHash, byte[] rgbSignature);
	}
}
