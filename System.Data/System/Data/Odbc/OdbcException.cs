using System;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Unity;

namespace System.Data.Odbc
{
	[Serializable]
	public sealed class OdbcException : DbException
	{
		internal static OdbcException CreateException(OdbcErrorCollection errors, ODBC32.RetCode retcode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in errors)
			{
				OdbcError odbcError = (OdbcError)obj;
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(Environment.NewLine);
				}
				stringBuilder.Append(SR.GetString("{0} [{1}] {2}", new object[]
				{
					ODBC32.RetcodeToString(retcode),
					odbcError.SQLState,
					odbcError.Message
				}));
			}
			return new OdbcException(stringBuilder.ToString(), errors);
		}

		internal OdbcException(string message, OdbcErrorCollection errors)
		{
			this._odbcErrors = new OdbcErrorCollection();
			base..ctor(message);
			this._odbcErrors = errors;
			base.HResult = -2146232009;
		}

		private OdbcException(SerializationInfo si, StreamingContext sc)
		{
			this._odbcErrors = new OdbcErrorCollection();
			base..ctor(si, sc);
			this._odbcErrors = (OdbcErrorCollection)si.GetValue("odbcErrors", typeof(OdbcErrorCollection));
			base.HResult = -2146232009;
		}

		public OdbcErrorCollection Errors
		{
			get
			{
				return this._odbcErrors;
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo si, StreamingContext context)
		{
			base.GetObjectData(si, context);
			si.AddValue("odbcRetcode", ODBC32.RETCODE.SUCCESS, typeof(ODBC32.RETCODE));
			si.AddValue("odbcErrors", this._odbcErrors, typeof(OdbcErrorCollection));
		}

		public override string Source
		{
			get
			{
				if (0 >= this.Errors.Count)
				{
					return "";
				}
				string source = this.Errors[0].Source;
				if (!string.IsNullOrEmpty(source))
				{
					return source;
				}
				return "";
			}
		}

		internal OdbcException()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private OdbcErrorCollection _odbcErrors;
	}
}
