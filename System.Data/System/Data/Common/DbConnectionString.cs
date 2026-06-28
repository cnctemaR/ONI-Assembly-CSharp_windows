using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace System.Data.Common
{
	[Obsolete]
	internal class DbConnectionString : DbConnectionOptions, ISerializable
	{
		protected internal DbConnectionString(DbConnectionString constr)
		{
			this.options = constr.options;
		}

		public DbConnectionString(string connectionString)
			: base(connectionString)
		{
			this.options = new NameValueCollection();
			base.ParseConnectionString(connectionString);
		}

		[MonoTODO]
		protected DbConnectionString(SerializationInfo si, StreamingContext sc)
		{
		}

		[MonoTODO]
		public DbConnectionString(string connectionString, string restrictions, KeyRestrictionBehavior behavior)
			: this(connectionString)
		{
			this.behavior = behavior;
		}

		public KeyRestrictionBehavior Behavior
		{
			get
			{
				return this.behavior;
			}
		}

		[MonoTODO]
		public string Restrictions
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		protected virtual string KeywordLookup(string keyname)
		{
			return keyname;
		}

		[MonoTODO]
		public virtual void PermissionDemand()
		{
			throw new NotImplementedException();
		}

		private KeyRestrictionBehavior behavior;
	}
}
