using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;

namespace System.Runtime.Remoting
{
	[ComVisible(true)]
	public class WellKnownServiceTypeEntry : TypeEntry
	{
		public WellKnownServiceTypeEntry(Type type, string objectUri, WellKnownObjectMode mode)
		{
			base.AssemblyName = type.Assembly.FullName;
			base.TypeName = type.FullName;
			this.obj_type = type;
			this.obj_uri = objectUri;
			this.obj_mode = mode;
		}

		public WellKnownServiceTypeEntry(string typeName, string assemblyName, string objectUri, WellKnownObjectMode mode)
		{
			base.AssemblyName = assemblyName;
			base.TypeName = typeName;
			Assembly assembly = Assembly.Load(assemblyName);
			this.obj_type = assembly.GetType(typeName);
			this.obj_uri = objectUri;
			this.obj_mode = mode;
			if (this.obj_type == null)
			{
				throw new RemotingException("Type not found: " + typeName + ", " + assemblyName);
			}
		}

		public IContextAttribute[] ContextAttributes
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public WellKnownObjectMode Mode
		{
			get
			{
				return this.obj_mode;
			}
		}

		public Type ObjectType
		{
			get
			{
				return this.obj_type;
			}
		}

		public string ObjectUri
		{
			get
			{
				return this.obj_uri;
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[] { base.TypeName, ", ", base.AssemblyName, " ", this.ObjectUri });
		}

		private Type obj_type;

		private string obj_uri;

		private WellKnownObjectMode obj_mode;
	}
}
