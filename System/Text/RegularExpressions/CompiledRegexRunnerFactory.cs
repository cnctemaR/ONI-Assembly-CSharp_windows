using System;
using System.Reflection.Emit;

namespace System.Text.RegularExpressions
{
	internal sealed class CompiledRegexRunnerFactory : RegexRunnerFactory
	{
		internal CompiledRegexRunnerFactory(DynamicMethod go, DynamicMethod firstChar, DynamicMethod trackCount)
		{
			this.goMethod = go;
			this.findFirstCharMethod = firstChar;
			this.initTrackCountMethod = trackCount;
		}

		protected internal override RegexRunner CreateInstance()
		{
			CompiledRegexRunner compiledRegexRunner = new CompiledRegexRunner();
			compiledRegexRunner.SetDelegates((NoParamDelegate)this.goMethod.CreateDelegate(typeof(NoParamDelegate)), (FindFirstCharDelegate)this.findFirstCharMethod.CreateDelegate(typeof(FindFirstCharDelegate)), (NoParamDelegate)this.initTrackCountMethod.CreateDelegate(typeof(NoParamDelegate)));
			return compiledRegexRunner;
		}

		private DynamicMethod goMethod;

		private DynamicMethod findFirstCharMethod;

		private DynamicMethod initTrackCountMethod;
	}
}
