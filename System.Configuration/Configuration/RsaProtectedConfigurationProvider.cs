using System;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace System.Configuration
{
	public sealed class RsaProtectedConfigurationProvider : ProtectedConfigurationProvider
	{
		private RSACryptoServiceProvider GetProvider()
		{
			if (this.rsa == null)
			{
				CspParameters cspParameters = new CspParameters();
				cspParameters.ProviderName = this.cspProviderName;
				cspParameters.KeyContainerName = this.keyContainerName;
				if (this.useMachineContainer)
				{
					cspParameters.Flags |= CspProviderFlags.UseMachineKeyStore;
				}
				this.rsa = new RSACryptoServiceProvider(cspParameters);
			}
			return this.rsa;
		}

		[MonoTODO]
		public override XmlNode Decrypt(XmlNode encrypted_node)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(new StringReader(encrypted_node.OuterXml));
			EncryptedXml encryptedXml = new EncryptedXml(xmlDocument);
			encryptedXml.AddKeyNameMapping("Rsa Key", this.GetProvider());
			encryptedXml.DecryptDocument();
			return xmlDocument.DocumentElement;
		}

		[MonoTODO]
		public override XmlNode Encrypt(XmlNode node)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(new StringReader(node.OuterXml));
			EncryptedXml encryptedXml = new EncryptedXml(xmlDocument);
			encryptedXml.AddKeyNameMapping("Rsa Key", this.GetProvider());
			EncryptedData encryptedData = encryptedXml.Encrypt(xmlDocument.DocumentElement, "Rsa Key");
			return encryptedData.GetXml();
		}

		[MonoTODO]
		public override void Initialize(string name, NameValueCollection configurationValues)
		{
			base.Initialize(name, configurationValues);
			this.keyContainerName = configurationValues["keyContainerName"];
			this.cspProviderName = configurationValues["cspProviderName"];
			string text = configurationValues["useMachineContainer"];
			if (text != null && text.ToLower() == "true")
			{
				this.useMachineContainer = true;
			}
			text = configurationValues["useOAEP"];
			if (text != null && text.ToLower() == "true")
			{
				this.useOAEP = true;
			}
		}

		[MonoTODO]
		public void AddKey(int keySize, bool exportable)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void DeleteKey()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void ExportKey(string xmlFileName, bool includePrivateParameters)
		{
			RSACryptoServiceProvider provider = this.GetProvider();
			string text = provider.ToXmlString(includePrivateParameters);
			FileStream fileStream = new FileStream(xmlFileName, FileMode.OpenOrCreate, FileAccess.Write);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(text);
			streamWriter.Close();
		}

		[MonoTODO]
		public void ImportKey(string xmlFileName, bool exportable)
		{
			throw new NotImplementedException();
		}

		public string CspProviderName
		{
			get
			{
				return this.cspProviderName;
			}
		}

		public string KeyContainerName
		{
			get
			{
				return this.keyContainerName;
			}
		}

		public RSAParameters RsaPublicKey
		{
			get
			{
				RSACryptoServiceProvider provider = this.GetProvider();
				return provider.ExportParameters(false);
			}
		}

		public bool UseMachineContainer
		{
			get
			{
				return this.useMachineContainer;
			}
		}

		public bool UseOAEP
		{
			get
			{
				return this.useOAEP;
			}
		}

		private string cspProviderName;

		private string keyContainerName;

		private bool useMachineContainer;

		private bool useOAEP;

		private RSACryptoServiceProvider rsa;
	}
}
