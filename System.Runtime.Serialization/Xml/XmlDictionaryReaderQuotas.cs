using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace System.Xml
{
	public sealed class XmlDictionaryReaderQuotas
	{
		public XmlDictionaryReaderQuotas()
		{
			XmlDictionaryReaderQuotas.defaultQuota.CopyTo(this);
		}

		private XmlDictionaryReaderQuotas(int maxDepth, int maxStringContentLength, int maxArrayLength, int maxBytesPerRead, int maxNameTableCharCount, XmlDictionaryReaderQuotaTypes modifiedQuotas)
		{
			this.maxDepth = maxDepth;
			this.maxStringContentLength = maxStringContentLength;
			this.maxArrayLength = maxArrayLength;
			this.maxBytesPerRead = maxBytesPerRead;
			this.maxNameTableCharCount = maxNameTableCharCount;
			this.modifiedQuotas = modifiedQuotas;
			this.MakeReadOnly();
		}

		public static XmlDictionaryReaderQuotas Max
		{
			get
			{
				return XmlDictionaryReaderQuotas.maxQuota;
			}
		}

		public void CopyTo(XmlDictionaryReaderQuotas quotas)
		{
			if (quotas == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("quotas"));
			}
			if (quotas.readOnly)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Cannot copy XmlDictionaryReaderQuotas. Target is readonly.")));
			}
			this.InternalCopyTo(quotas);
		}

		internal void InternalCopyTo(XmlDictionaryReaderQuotas quotas)
		{
			quotas.maxStringContentLength = this.maxStringContentLength;
			quotas.maxArrayLength = this.maxArrayLength;
			quotas.maxDepth = this.maxDepth;
			quotas.maxNameTableCharCount = this.maxNameTableCharCount;
			quotas.maxBytesPerRead = this.maxBytesPerRead;
			quotas.modifiedQuotas = this.modifiedQuotas;
		}

		[DefaultValue(8192)]
		public int MaxStringContentLength
		{
			get
			{
				return this.maxStringContentLength;
			}
			set
			{
				if (this.readOnly)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("The '{0}' quota is readonly.", new object[] { "MaxStringContentLength" })));
				}
				if (value <= 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Quota must be a positive value."), "value"));
				}
				this.maxStringContentLength = value;
				this.modifiedQuotas |= XmlDictionaryReaderQuotaTypes.MaxStringContentLength;
			}
		}

		[DefaultValue(16384)]
		public int MaxArrayLength
		{
			get
			{
				return this.maxArrayLength;
			}
			set
			{
				if (this.readOnly)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("The '{0}' quota is readonly.", new object[] { "MaxArrayLength" })));
				}
				if (value <= 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Quota must be a positive value."), "value"));
				}
				this.maxArrayLength = value;
				this.modifiedQuotas |= XmlDictionaryReaderQuotaTypes.MaxArrayLength;
			}
		}

		[DefaultValue(4096)]
		public int MaxBytesPerRead
		{
			get
			{
				return this.maxBytesPerRead;
			}
			set
			{
				if (this.readOnly)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("The '{0}' quota is readonly.", new object[] { "MaxBytesPerRead" })));
				}
				if (value <= 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Quota must be a positive value."), "value"));
				}
				this.maxBytesPerRead = value;
				this.modifiedQuotas |= XmlDictionaryReaderQuotaTypes.MaxBytesPerRead;
			}
		}

		[DefaultValue(32)]
		public int MaxDepth
		{
			get
			{
				return this.maxDepth;
			}
			set
			{
				if (this.readOnly)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("The '{0}' quota is readonly.", new object[] { "MaxDepth" })));
				}
				if (value <= 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Quota must be a positive value."), "value"));
				}
				this.maxDepth = value;
				this.modifiedQuotas |= XmlDictionaryReaderQuotaTypes.MaxDepth;
			}
		}

		[DefaultValue(16384)]
		public int MaxNameTableCharCount
		{
			get
			{
				return this.maxNameTableCharCount;
			}
			set
			{
				if (this.readOnly)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("The '{0}' quota is readonly.", new object[] { "MaxNameTableCharCount" })));
				}
				if (value <= 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Quota must be a positive value."), "value"));
				}
				this.maxNameTableCharCount = value;
				this.modifiedQuotas |= XmlDictionaryReaderQuotaTypes.MaxNameTableCharCount;
			}
		}

		public XmlDictionaryReaderQuotaTypes ModifiedQuotas
		{
			get
			{
				return this.modifiedQuotas;
			}
		}

		internal void MakeReadOnly()
		{
			this.readOnly = true;
		}

		private bool readOnly;

		private int maxStringContentLength;

		private int maxArrayLength;

		private int maxDepth;

		private int maxNameTableCharCount;

		private int maxBytesPerRead;

		private XmlDictionaryReaderQuotaTypes modifiedQuotas;

		private const int DefaultMaxDepth = 32;

		private const int DefaultMaxStringContentLength = 8192;

		private const int DefaultMaxArrayLength = 16384;

		private const int DefaultMaxBytesPerRead = 4096;

		private const int DefaultMaxNameTableCharCount = 16384;

		private static XmlDictionaryReaderQuotas defaultQuota = new XmlDictionaryReaderQuotas(32, 8192, 16384, 4096, 16384, (XmlDictionaryReaderQuotaTypes)0);

		private static XmlDictionaryReaderQuotas maxQuota = new XmlDictionaryReaderQuotas(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, XmlDictionaryReaderQuotaTypes.MaxDepth | XmlDictionaryReaderQuotaTypes.MaxStringContentLength | XmlDictionaryReaderQuotaTypes.MaxArrayLength | XmlDictionaryReaderQuotaTypes.MaxBytesPerRead | XmlDictionaryReaderQuotaTypes.MaxNameTableCharCount);
	}
}
