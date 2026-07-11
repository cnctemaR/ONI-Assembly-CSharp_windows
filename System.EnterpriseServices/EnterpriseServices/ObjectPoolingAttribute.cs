using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ObjectPoolingAttribute : Attribute, IConfigurationAttribute
	{
		public ObjectPoolingAttribute()
			: this(true)
		{
		}

		public ObjectPoolingAttribute(bool enable)
		{
			this.enabled = enable;
		}

		public ObjectPoolingAttribute(int minPoolSize, int maxPoolSize)
			: this(true, minPoolSize, maxPoolSize)
		{
		}

		public ObjectPoolingAttribute(bool enable, int minPoolSize, int maxPoolSize)
		{
			this.enabled = enable;
			this.minPoolSize = minPoolSize;
			this.maxPoolSize = maxPoolSize;
		}

		public int CreationTimeout
		{
			get
			{
				return this.creationTimeout;
			}
			set
			{
				this.creationTimeout = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		public int MaxPoolSize
		{
			get
			{
				return this.maxPoolSize;
			}
			set
			{
				this.maxPoolSize = value;
			}
		}

		public int MinPoolSize
		{
			get
			{
				return this.minPoolSize;
			}
			set
			{
				this.minPoolSize = value;
			}
		}

		[MonoTODO]
		public bool AfterSaveChanges(Hashtable info)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public bool Apply(Hashtable info)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public bool IsValidTarget(string s)
		{
			throw new NotImplementedException();
		}

		private int creationTimeout;

		private bool enabled;

		private int minPoolSize;

		private int maxPoolSize;
	}
}
