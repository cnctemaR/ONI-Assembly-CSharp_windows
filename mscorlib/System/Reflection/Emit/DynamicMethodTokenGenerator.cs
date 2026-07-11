using System;

namespace System.Reflection.Emit
{
	internal class DynamicMethodTokenGenerator : TokenGenerator
	{
		public DynamicMethodTokenGenerator(DynamicMethod m)
		{
			this.m = m;
		}

		public int GetToken(string str)
		{
			return this.m.AddRef(str);
		}

		public int GetToken(MethodBase method, Type[] opt_param_types)
		{
			throw new InvalidOperationException();
		}

		public int GetToken(MemberInfo member, bool create_open_instance)
		{
			return this.m.AddRef(member);
		}

		public int GetToken(SignatureHelper helper)
		{
			return this.m.AddRef(helper);
		}

		private DynamicMethod m;
	}
}
