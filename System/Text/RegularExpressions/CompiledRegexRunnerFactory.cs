using System;
using System.Reflection.Emit;

namespace System.Text.RegularExpressions
{
	internal sealed class CompiledRegexRunnerFactory : RegexRunnerFactory
	{
		public CompiledRegexRunnerFactory(DynamicMethod go, DynamicMethod firstChar, DynamicMethod trackCount)
		{
			this._goMethod = go;
			this._findFirstCharMethod = firstChar;
			this._initTrackCountMethod = trackCount;
		}

		protected internal override RegexRunner CreateInstance()
		{
			CompiledRegexRunner compiledRegexRunner = new CompiledRegexRunner();
			compiledRegexRunner.SetDelegates((Action<RegexRunner>)this._goMethod.CreateDelegate(typeof(Action<RegexRunner>)), (Func<RegexRunner, bool>)this._findFirstCharMethod.CreateDelegate(typeof(Func<RegexRunner, bool>)), (Action<RegexRunner>)this._initTrackCountMethod.CreateDelegate(typeof(Action<RegexRunner>)));
			return compiledRegexRunner;
		}

		private readonly DynamicMethod _goMethod;

		private readonly DynamicMethod _findFirstCharMethod;

		private readonly DynamicMethod _initTrackCountMethod;
	}
}
