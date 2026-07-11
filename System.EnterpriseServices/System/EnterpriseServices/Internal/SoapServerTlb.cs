using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices.Internal
{
	[Guid("F6B6768F-F99E-4152-8ED2-0412F78517FB")]
	public sealed class SoapServerTlb : ISoapServerTlb
	{
		[MonoTODO]
		public SoapServerTlb()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void AddServerTlb(string progId, string classId, string interfaceId, string srcTlbPath, string rootWebServer, string inBaseUrl, string inVirtualRoot, string clientActivated, string wellKnown, string discoFile, string operation, out string strAssemblyName, out string typeName)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void DeleteServerTlb(string progId, string classId, string interfaceId, string srcTlbPath, string rootWebServer, string baseUrl, string virtualRoot, string operation, string assemblyName, string typeName)
		{
			throw new NotImplementedException();
		}
	}
}
