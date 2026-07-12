using System;
using System.Collections.Generic;

namespace FMODUnity
{
	[Serializable]
	public class ThreadAffinityGroup
	{
		public ThreadAffinityGroup()
		{
		}

		public ThreadAffinityGroup(ThreadAffinityGroup other)
		{
			this.threads = new List<ThreadType>(other.threads);
			this.affinity = other.affinity;
		}

		public ThreadAffinityGroup(ThreadAffinity affinity, params ThreadType[] threads)
		{
			this.threads = new List<ThreadType>(threads);
			this.affinity = affinity;
		}

		public List<ThreadType> threads = new List<ThreadType>();

		public ThreadAffinity affinity;
	}
}
