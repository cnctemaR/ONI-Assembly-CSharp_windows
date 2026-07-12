using System;
using System.Collections.Generic;

namespace Database
{
	public class FacadeInfo
	{
		public string prefabID { get; set; }

		public string id { get; set; }

		public string name { get; set; }

		public string description { get; set; }

		public string animFile { get; set; }

		public List<FacadeInfo.workable> workables { get; set; }

		public string[] DLCIds { get; set; }

		public class workable
		{
			public string workableName { get; set; }

			public string workableAnim { get; set; }
		}
	}
}
