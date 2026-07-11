using System;
using System.Collections;
using System.Text;

namespace Mono.Security.X509.Extensions
{
	public class ExtendedKeyUsageExtension : X509Extension
	{
		public ExtendedKeyUsageExtension()
		{
			this.extnOid = "2.5.29.37";
			this.keyPurpose = new ArrayList();
		}

		public ExtendedKeyUsageExtension(ASN1 asn1)
			: base(asn1)
		{
		}

		public ExtendedKeyUsageExtension(X509Extension extension)
			: base(extension)
		{
		}

		protected override void Decode()
		{
			this.keyPurpose = new ArrayList();
			ASN1 asn = new ASN1(this.extnValue.Value);
			if (asn.Tag != 48)
			{
				throw new ArgumentException("Invalid ExtendedKeyUsage extension");
			}
			for (int i = 0; i < asn.Count; i++)
			{
				this.keyPurpose.Add(ASN1Convert.ToOid(asn[i]));
			}
		}

		protected override void Encode()
		{
			ASN1 asn = new ASN1(48);
			foreach (object obj in this.keyPurpose)
			{
				string text = (string)obj;
				asn.Add(ASN1Convert.FromOid(text));
			}
			this.extnValue = new ASN1(4);
			this.extnValue.Add(asn);
		}

		public ArrayList KeyPurpose
		{
			get
			{
				return this.keyPurpose;
			}
		}

		public override string Name
		{
			get
			{
				return "Extended Key Usage";
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in this.keyPurpose)
			{
				string text = (string)obj;
				if (!(text == "1.3.6.1.5.5.7.3.1"))
				{
					if (!(text == "1.3.6.1.5.5.7.3.2"))
					{
						if (!(text == "1.3.6.1.5.5.7.3.3"))
						{
							if (!(text == "1.3.6.1.5.5.7.3.4"))
							{
								if (!(text == "1.3.6.1.5.5.7.3.8"))
								{
									if (!(text == "1.3.6.1.5.5.7.3.9"))
									{
										stringBuilder.Append("unknown");
									}
									else
									{
										stringBuilder.Append("OCSP Signing");
									}
								}
								else
								{
									stringBuilder.Append("Time Stamping");
								}
							}
							else
							{
								stringBuilder.Append("Email Protection");
							}
						}
						else
						{
							stringBuilder.Append("Code Signing");
						}
					}
					else
					{
						stringBuilder.Append("Client Authentication");
					}
				}
				else
				{
					stringBuilder.Append("Server Authentication");
				}
				stringBuilder.AppendFormat(" ({0}){1}", text, Environment.NewLine);
			}
			return stringBuilder.ToString();
		}

		private ArrayList keyPurpose;
	}
}
