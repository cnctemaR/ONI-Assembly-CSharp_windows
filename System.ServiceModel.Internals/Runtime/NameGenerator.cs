using System;
using System.Globalization;
using System.Threading;

namespace System.Runtime
{
	internal class NameGenerator
	{
		private NameGenerator()
		{
			this.prefix = "_" + Guid.NewGuid().ToString().Replace('-', '_') + "_";
		}

		public static string Next()
		{
			long num = Interlocked.Increment(ref NameGenerator.nameGenerator.id);
			return NameGenerator.nameGenerator.prefix + num.ToString(CultureInfo.InvariantCulture);
		}

		private static NameGenerator nameGenerator = new NameGenerator();

		private long id;

		private string prefix;
	}
}
