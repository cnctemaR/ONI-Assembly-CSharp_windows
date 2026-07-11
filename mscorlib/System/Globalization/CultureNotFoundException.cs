using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System.Globalization
{
	[ComVisible(true)]
	[Serializable]
	public class CultureNotFoundException : ArgumentException, ISerializable
	{
		public CultureNotFoundException()
			: base(CultureNotFoundException.DefaultMessage)
		{
		}

		public CultureNotFoundException(string message)
			: base(message)
		{
		}

		public CultureNotFoundException(string paramName, string message)
			: base(message, paramName)
		{
		}

		public CultureNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public CultureNotFoundException(string paramName, int invalidCultureId, string message)
			: base(message, paramName)
		{
			this.m_invalidCultureId = new int?(invalidCultureId);
		}

		public CultureNotFoundException(string message, int invalidCultureId, Exception innerException)
			: base(message, innerException)
		{
			this.m_invalidCultureId = new int?(invalidCultureId);
		}

		public CultureNotFoundException(string paramName, string invalidCultureName, string message)
			: base(message, paramName)
		{
			this.m_invalidCultureName = invalidCultureName;
		}

		public CultureNotFoundException(string message, string invalidCultureName, Exception innerException)
			: base(message, innerException)
		{
			this.m_invalidCultureName = invalidCultureName;
		}

		protected CultureNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.m_invalidCultureId = (int?)info.GetValue("InvalidCultureId", typeof(int?));
			this.m_invalidCultureName = (string)info.GetValue("InvalidCultureName", typeof(string));
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			int? num = null;
			num = this.m_invalidCultureId;
			info.AddValue("InvalidCultureId", num, typeof(int?));
			info.AddValue("InvalidCultureName", this.m_invalidCultureName, typeof(string));
		}

		public virtual int? InvalidCultureId
		{
			get
			{
				return this.m_invalidCultureId;
			}
		}

		public virtual string InvalidCultureName
		{
			get
			{
				return this.m_invalidCultureName;
			}
		}

		private static string DefaultMessage
		{
			get
			{
				return Environment.GetResourceString("Culture is not supported.");
			}
		}

		private string FormatedInvalidCultureId
		{
			get
			{
				if (this.InvalidCultureId != null)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0} (0x{0:x4})", this.InvalidCultureId.Value);
				}
				return this.InvalidCultureName;
			}
		}

		public override string Message
		{
			get
			{
				string message = base.Message;
				if (this.m_invalidCultureId == null && this.m_invalidCultureName == null)
				{
					return message;
				}
				string resourceString = Environment.GetResourceString("{0} is an invalid culture identifier.", new object[] { this.FormatedInvalidCultureId });
				if (message == null)
				{
					return resourceString;
				}
				return message + Environment.NewLine + resourceString;
			}
		}

		private string m_invalidCultureName;

		private int? m_invalidCultureId;
	}
}
