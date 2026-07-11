using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.U2D
{
	/// <summary>
	///   <para>A static class that helps tessellate a SpriteShape mesh.</para>
	/// </summary>
	[NativeHeader("Modules/SpriteShape/Public/SpriteShapeUtility.h")]
	public class SpriteShapeUtility
	{
		/// <summary>
		///   <para>Generate a mesh based on input parameters.</para>
		/// </summary>
		/// <param name="mesh">The output mesh.</param>
		/// <param name="shapeParams">Input parameters for the SpriteShape tessellator.</param>
		/// <param name="points">A list of control points that describes the shape.</param>
		/// <param name="metaData">Additional data about the shape's control point. This is useful during tessellation of the shape.</param>
		/// <param name="sprites">The list of Sprites that could be used for the edges.</param>
		/// <param name="corners">The list of Sprites that could be used for the corners.</param>
		/// <param name="angleRange">A parameter that determins how to tessellate each of the edge.</param>
		[FreeFunction("SpriteShapeUtility::Generate")]
		[NativeThrows]
		public static int[] Generate(Mesh mesh, SpriteShapeParameters shapeParams, ShapeControlPoint[] points, SpriteShapeMetaData[] metaData, AngleRangeInfo[] angleRange, Sprite[] sprites, Sprite[] corners)
		{
			return SpriteShapeUtility.Generate_Injected(mesh, ref shapeParams, points, metaData, angleRange, sprites, corners);
		}

		/// <summary>
		///   <para>Generate a mesh based on input parameters.</para>
		/// </summary>
		/// <param name="renderer">SpriteShapeRenderer to which the generated geometry is fed to.</param>
		/// <param name="shapeParams">Input parameters for the SpriteShape tessellator.</param>
		/// <param name="points">A list of control points that describes the shape.</param>
		/// <param name="metaData">Additional data about the shape's control point. This is useful during tessellation of the shape.</param>
		/// <param name="sprites">The list of Sprites that could be used for the edges.</param>
		/// <param name="corners">The list of Sprites that could be used for the corners.</param>
		/// <param name="angleRange">A parameter that determins how to tessellate each of the edge.</param>
		[FreeFunction("SpriteShapeUtility::GenerateSpriteShape")]
		[NativeThrows]
		public static void GenerateSpriteShape(SpriteShapeRenderer renderer, SpriteShapeParameters shapeParams, ShapeControlPoint[] points, SpriteShapeMetaData[] metaData, AngleRangeInfo[] angleRange, Sprite[] sprites, Sprite[] corners)
		{
			SpriteShapeUtility.GenerateSpriteShape_Injected(renderer, ref shapeParams, points, metaData, angleRange, sprites, corners);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int[] Generate_Injected(Mesh mesh, ref SpriteShapeParameters shapeParams, ShapeControlPoint[] points, SpriteShapeMetaData[] metaData, AngleRangeInfo[] angleRange, Sprite[] sprites, Sprite[] corners);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateSpriteShape_Injected(SpriteShapeRenderer renderer, ref SpriteShapeParameters shapeParams, ShapeControlPoint[] points, SpriteShapeMetaData[] metaData, AngleRangeInfo[] angleRange, Sprite[] sprites, Sprite[] corners);
	}
}
