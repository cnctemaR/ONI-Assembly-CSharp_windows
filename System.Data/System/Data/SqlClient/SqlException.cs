using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Unity;

namespace System.Data.SqlClient
{
	[Serializable]
	public sealed class SqlException : DbException
	{
		private SqlException(string message, SqlErrorCollection errorCollection, Exception innerException, Guid conId)
		{
			this._clientConnectionId = Guid.Empty;
			base..ctor(message, innerException);
			base.HResult = -2146232060;
			this._errors = errorCollection;
			this._clientConnectionId = conId;
		}

		private SqlException(SerializationInfo si, StreamingContext sc)
		{
			this._clientConnectionId = Guid.Empty;
			base..ctor(si, sc);
			base.HResult = -2146232060;
			foreach (SerializationEntry serializationEntry in si)
			{
				if ("ClientConnectionId" == serializationEntry.Name)
				{
					this._clientConnectionId = (Guid)serializationEntry.Value;
					return;
				}
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo si, StreamingContext context)
		{
			base.GetObjectData(si, context);
			si.AddValue("Errors", null);
			si.AddValue("ClientConnectionId", this._clientConnectionId, typeof(Guid));
			for (int i = 0; i < this.Errors.Count; i++)
			{
				string text = "SqlError " + (i + 1).ToString();
				if (this.Data.Contains(text))
				{
					this.Data.Remove(text);
				}
				this.Data.Add(text, this.Errors[i].ToString());
			}
		}

		public SqlErrorCollection Errors
		{
			get
			{
				if (this._errors == null)
				{
					this._errors = new SqlErrorCollection();
				}
				return this._errors;
			}
		}

		public Guid ClientConnectionId
		{
			get
			{
				return this._clientConnectionId;
			}
		}

		public byte Class
		{
			get
			{
				if (this.Errors.Count <= 0)
				{
					return 0;
				}
				return this.Errors[0].Class;
			}
		}

		public int LineNumber
		{
			get
			{
				if (this.Errors.Count <= 0)
				{
					return 0;
				}
				return this.Errors[0].LineNumber;
			}
		}

		public int Number
		{
			get
			{
				if (this.Errors.Count <= 0)
				{
					return 0;
				}
				return this.Errors[0].Number;
			}
		}

		public string Procedure
		{
			get
			{
				if (this.Errors.Count <= 0)
				{
					return null;
				}
				return this.Errors[0].Procedure;
			}
		}

		public string Server
		{
			get
			{
				if (this.Errors.Count <= 0)
				{
					return null;
				}
				return this.Errors[0].Server;
			}
		}

		public byte State
		{
			get
			{
				if (this.Errors.Count <= 0)
				{
					return 0;
				}
				return this.Errors[0].State;
			}
		}

		public override string Source
		{
			get
			{
				if (this.Errors.Count <= 0)
				{
					return null;
				}
				return this.Errors[0].Source;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.ToString());
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat(SQLMessage.ExClientConnectionId(), this._clientConnectionId);
			if (this.Errors.Count > 0 && this.Number != 0)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat(SQLMessage.ExErrorNumberStateClass(), this.Number, this.State, this.Class);
			}
			if (this.Data.Contains("OriginalClientConnectionId"))
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat(SQLMessage.ExOriginalClientConnectionId(), this.Data["OriginalClientConnectionId"]);
			}
			if (this.Data.Contains("RoutingDestination"))
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat(SQLMessage.ExRoutingDestination(), this.Data["RoutingDestination"]);
			}
			return stringBuilder.ToString();
		}

		internal static SqlException CreateException(SqlErrorCollection errorCollection, string serverVersion)
		{
			return SqlException.CreateException(errorCollection, serverVersion, Guid.Empty, null);
		}

		internal static SqlException CreateException(SqlErrorCollection errorCollection, string serverVersion, SqlInternalConnectionTds internalConnection, Exception innerException = null)
		{
			Guid guid = ((internalConnection == null) ? Guid.Empty : internalConnection._clientConnectionId);
			SqlException ex = SqlException.CreateException(errorCollection, serverVersion, guid, innerException);
			if (internalConnection != null)
			{
				if (internalConnection.OriginalClientConnectionId != Guid.Empty && internalConnection.OriginalClientConnectionId != internalConnection.ClientConnectionId)
				{
					ex.Data.Add("OriginalClientConnectionId", internalConnection.OriginalClientConnectionId);
				}
				if (!string.IsNullOrEmpty(internalConnection.RoutingDestination))
				{
					ex.Data.Add("RoutingDestination", internalConnection.RoutingDestination);
				}
			}
			return ex;
		}

		internal static SqlException CreateException(SqlErrorCollection errorCollection, string serverVersion, Guid conId, Exception innerException = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < errorCollection.Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(Environment.NewLine);
				}
				stringBuilder.Append(errorCollection[i].Message);
			}
			if (innerException == null && errorCollection[0].Win32ErrorCode != 0 && errorCollection[0].Win32ErrorCode != -1)
			{
				innerException = new Win32Exception(errorCollection[0].Win32ErrorCode);
			}
			SqlException ex = new SqlException(stringBuilder.ToString(), errorCollection, innerException, conId);
			ex.Data.Add("HelpLink.ProdName", "Microsoft SQL Server");
			if (!string.IsNullOrEmpty(serverVersion))
			{
				ex.Data.Add("HelpLink.ProdVer", serverVersion);
			}
			ex.Data.Add("HelpLink.EvtSrc", "MSSQLServer");
			ex.Data.Add("HelpLink.EvtID", errorCollection[0].Number.ToString(CultureInfo.InvariantCulture));
			ex.Data.Add("HelpLink.BaseHelpUrl", "http://go.microsoft.com/fwlink");
			ex.Data.Add("HelpLink.LinkId", "20476");
			return ex;
		}

		internal SqlException InternalClone()
		{
			SqlException ex = new SqlException(this.Message, this._errors, base.InnerException, this._clientConnectionId);
			if (this.Data != null)
			{
				foreach (object obj in this.Data)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					ex.Data.Add(dictionaryEntry.Key, dictionaryEntry.Value);
				}
			}
			ex._doNotReconnect = this._doNotReconnect;
			return ex;
		}

		public override string Message
		{
			get
			{
				if (this.Errors.Count == 0)
				{
					return base.Message;
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (base.Message != "SQL Exception has occured.")
				{
					stringBuilder.Append(base.Message);
					stringBuilder.Append("\n");
				}
				for (int i = 0; i < this.Errors.Count - 1; i++)
				{
					stringBuilder.Append(this.Errors[i].Message);
					stringBuilder.Append("\n");
				}
				stringBuilder.Append(this.Errors[this.Errors.Count - 1].Message);
				return stringBuilder.ToString();
			}
		}

		internal SqlException()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private const string OriginalClientConnectionIdKey = "OriginalClientConnectionId";

		private const string RoutingDestinationKey = "RoutingDestination";

		private const int SqlExceptionHResult = -2146232060;

		private SqlErrorCollection _errors;

		private Guid _clientConnectionId;

		internal bool _doNotReconnect;

		private const string DEF_MESSAGE = "SQL Exception has occured.";
	}
}
