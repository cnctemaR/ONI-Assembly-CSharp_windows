using System;

namespace System.Drawing.Imaging
{
	public sealed class FrameDimension
	{
		public FrameDimension(Guid guid)
		{
			this.guid = guid;
		}

		internal FrameDimension(Guid guid, string name)
		{
			this.guid = guid;
			this.name = name;
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public static FrameDimension Page
		{
			get
			{
				if (FrameDimension.page == null)
				{
					FrameDimension.page = new FrameDimension(new Guid("7462dc86-6180-4c7e-8e3f-ee7333a7a483"), "Page");
				}
				return FrameDimension.page;
			}
		}

		public static FrameDimension Resolution
		{
			get
			{
				if (FrameDimension.resolution == null)
				{
					FrameDimension.resolution = new FrameDimension(new Guid("84236f7b-3bd3-428f-8dab-4ea1439ca315"), "Resolution");
				}
				return FrameDimension.resolution;
			}
		}

		public static FrameDimension Time
		{
			get
			{
				if (FrameDimension.time == null)
				{
					FrameDimension.time = new FrameDimension(new Guid("6aedbd6d-3fb5-418a-83a6-7f45229dc872"), "Time");
				}
				return FrameDimension.time;
			}
		}

		public override bool Equals(object o)
		{
			FrameDimension frameDimension = o as FrameDimension;
			return frameDimension != null && this.guid == frameDimension.guid;
		}

		public override int GetHashCode()
		{
			return this.guid.GetHashCode();
		}

		public override string ToString()
		{
			if (this.name == null)
			{
				this.name = string.Format("[FrameDimension: {0}]", this.guid);
			}
			return this.name;
		}

		private Guid guid;

		private string name;

		private static FrameDimension page;

		private static FrameDimension resolution;

		private static FrameDimension time;
	}
}
