using System;

namespace System.Reflection.Emit
{
	internal class ModuleBuilderTokenGenerator : TokenGenerator
	{
		public ModuleBuilderTokenGenerator(ModuleBuilder mb)
		{
			this.mb = mb;
		}

		public int GetToken(string str)
		{
			return this.mb.GetToken(str);
		}

		public int GetToken(MemberInfo member, bool create_open_instance)
		{
			return this.mb.GetToken(member, create_open_instance);
		}

		public int GetToken(MethodBase method, Type[] opt_param_types)
		{
			return this.mb.GetToken(method, opt_param_types);
		}

		public int GetToken(SignatureHelper helper)
		{
			return this.mb.GetToken(helper);
		}

		private ModuleBuilder mb;
	}
}
