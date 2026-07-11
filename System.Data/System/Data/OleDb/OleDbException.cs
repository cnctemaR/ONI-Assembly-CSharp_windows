using System;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbException : DbException
	{
		internal OleDbException()
		{
		}

		public override int ErrorCode
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public OleDbErrorCollection Errors
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo si, StreamingContext context)
		{
			throw ADP.OleDb();
		}
	}
}
