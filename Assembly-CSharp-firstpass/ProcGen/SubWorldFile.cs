using System;
using Klei;

namespace ProcGen
{
	public class SubWorldFile : YamlIO<SubWorldFile>
	{
		public SubWorldFile()
		{
			this.zone = new SubWorld();
		}

		public string name { get; private set; }

		public SubWorld zone { get; private set; }
	}
}
