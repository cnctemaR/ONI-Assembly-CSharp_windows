using System;
using System.Collections.Generic;
using Generated;

namespace Klei
{
	public class Rivers : YamlIO<Rivers>
	{
		public Rivers()
		{
			this.rivers = new Dictionary<string, River>();
		}

		public Dictionary<string, River> rivers { get; private set; }
	}
}
