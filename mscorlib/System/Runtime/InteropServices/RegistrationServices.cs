using System;
using System.Reflection;
using System.Security;

namespace System.Runtime.InteropServices
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[Guid("475e398f-8afa-43a7-a3be-f4ef8d6787c9")]
	public class RegistrationServices : IRegistrationServices
	{
		public virtual Guid GetManagedCategoryGuid()
		{
			return RegistrationServices.guidManagedCategory;
		}

		[SecurityCritical]
		public virtual string GetProgIdForType(Type type)
		{
			return Marshal.GenerateProgIdForType(type);
		}

		[MonoTODO("implement")]
		[SecurityCritical]
		public virtual Type[] GetRegistrableTypesInAssembly(Assembly assembly)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("implement")]
		[SecurityCritical]
		public virtual bool RegisterAssembly(Assembly assembly, AssemblyRegistrationFlags flags)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("implement")]
		[SecurityCritical]
		public virtual void RegisterTypeForComClients(Type type, ref Guid g)
		{
			throw new NotImplementedException();
		}

		[SecuritySafeCritical]
		[MonoTODO("implement")]
		public virtual bool TypeRepresentsComType(Type type)
		{
			throw new NotImplementedException();
		}

		[SecurityCritical]
		[MonoTODO("implement")]
		public virtual bool TypeRequiresRegistration(Type type)
		{
			throw new NotImplementedException();
		}

		[SecurityCritical]
		[MonoTODO("implement")]
		public virtual bool UnregisterAssembly(Assembly assembly)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("implement")]
		[ComVisible(false)]
		public virtual int RegisterTypeForComClients(Type type, RegistrationClassContext classContext, RegistrationConnectionType flags)
		{
			throw new NotImplementedException();
		}

		[ComVisible(false)]
		[MonoTODO("implement")]
		public virtual void UnregisterTypeForComClients(int cookie)
		{
			throw new NotImplementedException();
		}

		private static Guid guidManagedCategory = new Guid("{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}");
	}
}
