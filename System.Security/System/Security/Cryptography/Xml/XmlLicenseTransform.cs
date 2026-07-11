using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class XmlLicenseTransform : Transform
	{
		public XmlLicenseTransform()
		{
			base.Algorithm = "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform";
		}

		public IRelDecryptor Decryptor
		{
			get
			{
				return this._decryptor;
			}
			set
			{
				this._decryptor = value;
			}
		}

		public override Type[] InputTypes
		{
			get
			{
				if (this.inputTypes == null)
				{
					this.inputTypes = new Type[] { typeof(XmlDocument) };
				}
				return this.inputTypes;
			}
		}

		public override Type[] OutputTypes
		{
			get
			{
				if (this.outputTypes == null)
				{
					this.outputTypes = new Type[] { typeof(XmlDocument) };
				}
				return this.outputTypes;
			}
		}

		[MonoTODO]
		protected override XmlNodeList GetInnerXml()
		{
			return null;
		}

		[MonoTODO]
		public override object GetOutput()
		{
			return null;
		}

		public override object GetOutput(Type type)
		{
			if (type != typeof(XmlDocument))
			{
				throw new ArgumentException("type");
			}
			return this.GetOutput();
		}

		public override void LoadInnerXml(XmlNodeList nodeList)
		{
		}

		[MonoTODO]
		public override void LoadInput(object obj)
		{
			if (obj != typeof(XmlDocument))
			{
				throw new ArgumentException("obj");
			}
			if (this._decryptor == null)
			{
				throw new CryptographicException(Locale.GetText("missing decryptor"));
			}
		}

		private IRelDecryptor _decryptor;

		private Type[] inputTypes;

		private Type[] outputTypes;
	}
}
