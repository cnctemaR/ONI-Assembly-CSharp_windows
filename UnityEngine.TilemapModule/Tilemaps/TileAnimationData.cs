using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps
{
	/// <summary>
	///   <para>A Struct for the required data for animating a Tile.</para>
	/// </summary>
	[NativeType(Header = "Modules/Tilemap/TilemapScripting.h")]
	[RequiredByNativeCode]
	public struct TileAnimationData
	{
		/// <summary>
		///   <para>The array of that are ordered by appearance in the animation.</para>
		/// </summary>
		public Sprite[] animatedSprites
		{
			get
			{
				return this.m_AnimatedSprites;
			}
			set
			{
				this.m_AnimatedSprites = value;
			}
		}

		/// <summary>
		///   <para>The animation speed.</para>
		/// </summary>
		public float animationSpeed
		{
			get
			{
				return this.m_AnimationSpeed;
			}
			set
			{
				this.m_AnimationSpeed = value;
			}
		}

		/// <summary>
		///   <para>The start time of the animation. The animation will begin at this time offset.</para>
		/// </summary>
		public float animationStartTime
		{
			get
			{
				return this.m_AnimationStartTime;
			}
			set
			{
				this.m_AnimationStartTime = value;
			}
		}

		private Sprite[] m_AnimatedSprites;

		private float m_AnimationSpeed;

		private float m_AnimationStartTime;
	}
}
