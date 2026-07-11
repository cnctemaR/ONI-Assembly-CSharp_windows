using System;

namespace System.Xml.Schema
{
	internal class UpaException : Exception
	{
		public UpaException(object particle1, object particle2)
		{
			this.particle1 = particle1;
			this.particle2 = particle2;
		}

		public object Particle1
		{
			get
			{
				return this.particle1;
			}
		}

		public object Particle2
		{
			get
			{
				return this.particle2;
			}
		}

		private object particle1;

		private object particle2;
	}
}
