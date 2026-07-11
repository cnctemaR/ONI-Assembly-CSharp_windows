using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices.Internal
{
	[Guid("ecabafd1-7f19-11d2-978e-0000f8757e2a")]
	public class ClrObjectFactory : IClrObjectFactory
	{
		[MonoTODO]
		public ClrObjectFactory()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public object CreateFromAssembly(string AssemblyName, string TypeName, string Mode)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public object CreateFromMailbox(string Mailbox, string Mode)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public object CreateFromVroot(string VrootUrl, string Mode)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public object CreateFromWsdl(string WsdlUrl, string Mode)
		{
			throw new NotImplementedException();
		}
	}
}
