using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Describes a material render queue range.</para>
	/// </summary>
	public struct RenderQueueRange
	{
		/// <summary>
		///   <para>A range that includes all objects.</para>
		/// </summary>
		public static RenderQueueRange all
		{
			get
			{
				return new RenderQueueRange
				{
					min = 0,
					max = 5000
				};
			}
		}

		/// <summary>
		///   <para>A range that includes only opaque objects.</para>
		/// </summary>
		public static RenderQueueRange opaque
		{
			get
			{
				return new RenderQueueRange
				{
					min = 0,
					max = 2500
				};
			}
		}

		/// <summary>
		///   <para>A range that includes only transparent objects.</para>
		/// </summary>
		public static RenderQueueRange transparent
		{
			get
			{
				return new RenderQueueRange
				{
					min = 2501,
					max = 5000
				};
			}
		}

		/// <summary>
		///   <para>Inclusive lower bound for the range.</para>
		/// </summary>
		public int min;

		/// <summary>
		///   <para>Inclusive upper bound for the range.</para>
		/// </summary>
		public int max;
	}
}
