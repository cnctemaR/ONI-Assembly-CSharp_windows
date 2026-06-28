using System;

namespace System.EnterpriseServices
{
	public sealed class ResourcePool
	{
		[MonoTODO]
		public ResourcePool(ResourcePool.TransactionEndDelegate cb)
		{
		}

		[MonoTODO]
		public object GetResource()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public bool PutResource(object resource)
		{
			throw new NotImplementedException();
		}

		public delegate void TransactionEndDelegate(object resource);
	}
}
