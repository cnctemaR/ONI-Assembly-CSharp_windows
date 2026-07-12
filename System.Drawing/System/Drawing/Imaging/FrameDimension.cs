using System;

namespace System.Drawing.Imaging
{
	public sealed class FrameDimension
	{
		public FrameDimension(Guid guid)
		{
			this._guid = guid;
		}

		public Guid Guid
		{
			get
			{
				return this._guid;
			}
		}

		public static FrameDimension Time
		{
			get
			{
				return FrameDimension.s_time;
			}
		}

		public static FrameDimension Resolution
		{
			get
			{
				return FrameDimension.s_resolution;
			}
		}

		public static FrameDimension Page
		{
			get
			{
				return FrameDimension.s_page;
			}
		}

		public override bool Equals(object o)
		{
			FrameDimension frameDimension = o as FrameDimension;
			return frameDimension != null && this._guid == frameDimension._guid;
		}

		public override int GetHashCode()
		{
			return this._guid.GetHashCode();
		}

		public override string ToString()
		{
			if (this == FrameDimension.s_time)
			{
				return "Time";
			}
			if (this == FrameDimension.s_resolution)
			{
				return "Resolution";
			}
			if (this == FrameDimension.s_page)
			{
				return "Page";
			}
			string text = "[FrameDimension: ";
			Guid guid = this._guid;
			return text + guid.ToString() + "]";
		}

		private static FrameDimension s_time = new FrameDimension(new Guid("{6aedbd6d-3fb5-418a-83a6-7f45229dc872}"));

		private static FrameDimension s_resolution = new FrameDimension(new Guid("{84236f7b-3bd3-428f-8dab-4ea1439ca315}"));

		private static FrameDimension s_page = new FrameDimension(new Guid("{7462dc86-6180-4c7e-8e3f-ee7333a7a483}"));

		private Guid _guid;
	}
}
