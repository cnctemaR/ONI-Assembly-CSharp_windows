using System;
using System.Runtime.Serialization;
using System.Security;

namespace System.Globalization
{
	[Serializable]
	public class CultureNotFoundException : ArgumentException
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

		public CultureNotFoundException(string paramName, string invalidCultureName, string message)
			: base(message, paramName)
		{
			this._invalidCultureName = invalidCultureName;
		}

		public CultureNotFoundException(string message, string invalidCultureName, Exception innerException)
			: base(message, innerException)
		{
			this._invalidCultureName = invalidCultureName;
		}

		public CultureNotFoundException(string message, int invalidCultureId, Exception innerException)
			: base(message, innerException)
		{
			this._invalidCultureId = new int?(invalidCultureId);
		}

		public CultureNotFoundException(string paramName, int invalidCultureId, string message)
			: base(message, paramName)
		{
			this._invalidCultureId = new int?(invalidCultureId);
		}

		protected CultureNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._invalidCultureId = (int?)info.GetValue("InvalidCultureId", typeof(int?));
			this._invalidCultureName = (string)info.GetValue("InvalidCultureName", typeof(string));
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("InvalidCultureId", this._invalidCultureId, typeof(int?));
			info.AddValue("InvalidCultureName", this._invalidCultureName, typeof(string));
		}

		public virtual int? InvalidCultureId
		{
			get
			{
				return this._invalidCultureId;
			}
		}

		public virtual string InvalidCultureName
		{
			get
			{
				return this._invalidCultureName;
			}
		}

		private static string DefaultMessage
		{
			get
			{
				return "Culture is not supported.";
			}
		}

		private string FormatedInvalidCultureId
		{
			get
			{
				if (this.InvalidCultureId == null)
				{
					return this.InvalidCultureName;
				}
				return string.Format(CultureInfo.InvariantCulture, "{0} (0x{0:x4})", this.InvalidCultureId.Value);
			}
		}

		public override string Message
		{
			get
			{
				string message = base.Message;
				if (this._invalidCultureId == null && this._invalidCultureName == null)
				{
					return message;
				}
				string text = SR.Format("{0} is an invalid culture identifier.", this.FormatedInvalidCultureId);
				if (message == null)
				{
					return text;
				}
				return message + Environment.NewLine + text;
			}
		}

		private string _invalidCultureName;

		private int? _invalidCultureId;
	}
}
