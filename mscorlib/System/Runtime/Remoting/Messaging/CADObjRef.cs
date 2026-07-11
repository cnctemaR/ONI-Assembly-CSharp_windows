using System;

namespace System.Runtime.Remoting.Messaging
{
	internal class CADObjRef
	{
		public CADObjRef(ObjRef o, int sourceDomain)
		{
			this.objref = o;
			this.TypeInfo = o.SerializeType();
			this.SourceDomain = sourceDomain;
		}

		public string TypeName
		{
			get
			{
				return this.objref.TypeInfo.TypeName;
			}
		}

		public string URI
		{
			get
			{
				return this.objref.URI;
			}
		}

		internal ObjRef objref;

		internal int SourceDomain;

		internal byte[] TypeInfo;
	}
}
