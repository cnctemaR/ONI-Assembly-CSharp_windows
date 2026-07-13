using System;

namespace Mono.Cecil.Cil
{
	internal sealed class Document : DebugInformation
	{
		public string Url
		{
			get
			{
				return this.url;
			}
			set
			{
				this.url = value;
			}
		}

		public DocumentType Type
		{
			get
			{
				return this.type.ToType();
			}
			set
			{
				this.type = value.ToGuid();
			}
		}

		public Guid TypeGuid
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public DocumentHashAlgorithm HashAlgorithm
		{
			get
			{
				return this.hash_algorithm.ToHashAlgorithm();
			}
			set
			{
				this.hash_algorithm = value.ToGuid();
			}
		}

		public Guid HashAlgorithmGuid
		{
			get
			{
				return this.hash_algorithm;
			}
			set
			{
				this.hash_algorithm = value;
			}
		}

		public DocumentLanguage Language
		{
			get
			{
				return this.language.ToLanguage();
			}
			set
			{
				this.language = value.ToGuid();
			}
		}

		public Guid LanguageGuid
		{
			get
			{
				return this.language;
			}
			set
			{
				this.language = value;
			}
		}

		public DocumentLanguageVendor LanguageVendor
		{
			get
			{
				return this.language_vendor.ToVendor();
			}
			set
			{
				this.language_vendor = value.ToGuid();
			}
		}

		public Guid LanguageVendorGuid
		{
			get
			{
				return this.language_vendor;
			}
			set
			{
				this.language_vendor = value;
			}
		}

		public byte[] Hash
		{
			get
			{
				return this.hash;
			}
			set
			{
				this.hash = value;
			}
		}

		public byte[] EmbeddedSource
		{
			get
			{
				return this.embedded_source;
			}
			set
			{
				this.embedded_source = value;
			}
		}

		public Document(string url)
		{
			this.url = url;
			this.hash = Empty<byte>.Array;
			this.embedded_source = Empty<byte>.Array;
			this.token = new MetadataToken(TokenType.Document);
		}

		private string url;

		private Guid type;

		private Guid hash_algorithm;

		private Guid language;

		private Guid language_vendor;

		private byte[] hash;

		private byte[] embedded_source;
	}
}
