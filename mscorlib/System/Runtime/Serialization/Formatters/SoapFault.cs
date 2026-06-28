using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata;

namespace System.Runtime.Serialization.Formatters
{
	[SoapType]
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapFault : ISerializable
	{
		public SoapFault()
		{
		}

		private SoapFault(SerializationInfo info, StreamingContext context)
		{
			this.code = info.GetString("faultcode");
			this.faultString = info.GetString("faultstring");
			this.detail = info.GetValue("detail", typeof(object));
		}

		public SoapFault(string faultCode, string faultString, string faultActor, ServerFault serverFault)
		{
			this.code = faultCode;
			this.actor = faultActor;
			this.faultString = faultString;
			this.detail = serverFault;
		}

		public object Detail
		{
			get
			{
				return this.detail;
			}
			set
			{
				this.detail = value;
			}
		}

		public string FaultActor
		{
			get
			{
				return this.actor;
			}
			set
			{
				this.actor = value;
			}
		}

		public string FaultCode
		{
			get
			{
				return this.code;
			}
			set
			{
				this.code = value;
			}
		}

		public string FaultString
		{
			get
			{
				return this.faultString;
			}
			set
			{
				this.faultString = value;
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("faultcode", this.code, typeof(string));
			info.AddValue("faultstring", this.faultString, typeof(string));
			info.AddValue("detail", this.detail, typeof(object));
		}

		private string code;

		private string actor;

		private string faultString;

		private object detail;
	}
}
