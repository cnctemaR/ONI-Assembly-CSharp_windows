using System;

namespace TiledSharp
{
	public static class Tuple
	{
		public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
		{
			return new Tuple<T1, T2>(first, second);
		}

		public static Tuple<T1, T2> Create<T1, T2>(T1 t1, T2 t2)
		{
			return new Tuple<T1, T2>(t1, t2);
		}
	}
}
