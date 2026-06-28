using System;
using System.Collections.Generic;

namespace Klei
{
	public class Temperatures : YamlIO<Temperatures>
	{
		public Temperatures()
		{
			this.ranges = new Dictionary<Temperature.Range, Temperature>();
		}

		public Dictionary<Temperature.Range, Temperature> ranges { get; private set; }
	}
}
