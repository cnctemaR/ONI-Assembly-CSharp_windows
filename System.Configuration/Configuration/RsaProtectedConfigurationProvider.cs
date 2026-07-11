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
		public override XmlNode Decrypt(XmlNode encryptedNode)
		{
			ConfigurationXmlDocument configurationXmlDocument = new ConfigurationXmlDocument();
			configurationXmlDocument.Load(new StringReader(encryptedNode.OuterXml));
			EncryptedXml encryptedXml = new EncryptedXml(configurationXmlDocument);
			encryptedXml.AddKeyNameMapping("Rsa Key", this.GetProvider());
			encryptedXml.DecryptDocument();
			return configurationXmlDocument.DocumentElement;
		}

		[MonoTODO]
		public override XmlNode Encrypt(XmlNode node)
		{
			XmlDocument xmlDocument = new ConfigurationXmlDocument();
			xmlDocument.Load(new StringReader(node.OuterXml));
			EncryptedXml encryptedXml = new EncryptedXml(xmlDocument);
			encryptedXml.AddKeyNameMapping("Rsa Key", this.GetProvider());
			return encryptedXml.Encrypt(xmlDocument.DocumentElement, "Rsa Key").GetXml();
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
			string text = this.GetProvider().ToXmlString(includePrivateParameters);
			StreamWriter streamWriter = new StreamWriter(new FileStream(xmlFileName, FileMode.OpenOrCreate, FileAccess.Write));
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
				return this.GetProvider().ExportParameters(false);
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
