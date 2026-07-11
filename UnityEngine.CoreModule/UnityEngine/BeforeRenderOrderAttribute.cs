using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Use this BeforeRenderOrderAttribute when you need to specify a custom callback order for Application.onBeforeRender.</para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class BeforeRenderOrderAttribute : Attribute
	{
		/// <summary>
		///   <para>When applied to methods, specifies the order called during Application.onBeforeRender events.</para>
		/// </summary>
		/// <param name="order">The sorting order, sorted lowest to highest.</param>
		public BeforeRenderOrderAttribute(int order)
		{
			this.order = order;
		}

		/// <summary>
		///   <para>The order, lowest to highest, that the Application.onBeforeRender event recievers will be called in.</para>
		/// </summary>
		public int order { get; private set; }
	}
}
