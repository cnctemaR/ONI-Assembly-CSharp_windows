using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	public sealed class SharedPropertyGroupManager : IEnumerable
	{
		[MonoTODO]
		public SharedPropertyGroup CreatePropertyGroup(string name, ref PropertyLockMode dwIsoMode, ref PropertyReleaseMode dwRelMode, out bool fExist)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public SharedPropertyGroup Group(string name)
		{
			throw new NotImplementedException();
		}
	}
}
