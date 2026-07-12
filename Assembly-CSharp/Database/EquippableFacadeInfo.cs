using System;
using System.Collections.Generic;

namespace Database
{
	public class EquippableFacadeInfo
	{
		public string defID { get; set; }

		public List<EquippableFacadeInfo.equippable> equippables { get; set; }

		public class equippable
		{
			public string name { get; set; }

			public string buildoverride { get; set; }

			public string animfile { get; set; }
		}
	}
}
