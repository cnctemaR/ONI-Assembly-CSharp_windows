using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>A Camera is a device through which the player views the world.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/RenderTexture.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Misc/GameObjectUtility.h")]
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Runtime/Camera/Camera.h")]
	[NativeHeader("Runtime/Camera/RenderManager.h")]
	[NativeHeader("Runtime/GfxDevice/GfxDeviceTypes.h")]
	[NativeHeader("Runtime/Graphics/CommandBuffer/RenderingCommandBuffer.h")]
	[NativeHeader("Runtime/Shaders/Shader.h")]
	public sealed class Camera : Behaviour
	{
		/// <summary>
		///   <para>Get command buffers to be executed at a specified place.</para>
		/// </summary>
		/// <param name="evt">When to execute the command buffer during rendering.</param>
		/// <returns>
		///   <para>Array of command buffers.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern CommandBuffer[] GetCommandBuffers(CameraEvent evt);

		internal void OnlyUsedForTesting1()
		{
		}

		internal void OnlyUsedForTesting2()
		{
		}

		/// <summary>
		///   <para>The near clipping plane distance.</para>
		/// </summary>
		[NativeProperty("Near")]
		public extern float nearClipPlane
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The far clipping plane distance.</para>
		/// </summary>
		[NativeProperty("Far")]
		public extern float farClipPlane
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The field of view of the camera in degrees.</para>
		/// </summary>
		[NativeProperty("Fov")]
		public extern float fieldOfView
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The rendering path that should be used, if possible.</para>
		/// </summary>
		public extern RenderingPath renderingPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The rendering path that is currently being used (Read Only).</para>
		/// </summary>
		public extern RenderingPath actualRenderingPath
		{
			[NativeName("CalculateRenderingPath")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Revert all camera parameters to default.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Reset();

		/// <summary>
		///   <para>High dynamic range rendering.</para>
		/// </summary>
		public extern bool allowHDR
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>MSAA rendering.</para>
		/// </summary>
		public extern bool allowMSAA
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Dynamic Resolution Scaling.</para>
		/// </summary>
		public extern bool allowDynamicResolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Should camera rendering be forced into a RenderTexture.</para>
		/// </summary>
		[NativeProperty("ForceIntoRT")]
		public extern bool forceIntoRenderTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Camera's half-size when in orthographic mode.</para>
		/// </summary>
		public extern float orthographicSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Is the camera orthographic (true) or perspective (false)?</para>
		/// </summary>
		public extern bool orthographic
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Opaque object sorting mode.</para>
		/// </summary>
		public extern OpaqueSortMode opaqueSortMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Transparent object sorting mode.</para>
		/// </summary>
		public extern TransparencySortMode transparencySortMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>An axis that describes the direction along which the distances of objects are measured for the purpose of sorting.</para>
		/// </summary>
		public Vector3 transparencySortAxis
		{
			get
			{
				Vector3 vector;
				this.get_transparencySortAxis_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_transparencySortAxis_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Resets this Camera's transparency sort settings to the default. Default transparency settings are taken from GraphicsSettings instead of directly from this Camera.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetTransparencySortSettings();

		/// <summary>
		///   <para>Camera's depth in the camera rendering order.</para>
		/// </summary>
		public extern float depth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The aspect ratio (width divided by height).</para>
		/// </summary>
		public extern float aspect
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Revert the aspect ratio to the screen's aspect ratio.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetAspect();

		/// <summary>
		///   <para>Get the world-space speed of the camera (Read Only).</para>
		/// </summary>
		public Vector3 velocity
		{
			get
			{
				Vector3 vector;
				this.get_velocity_Injected(out vector);
				return vector;
			}
		}

		/// <summary>
		///   <para>This is used to render parts of the scene selectively.</para>
		/// </summary>
		public extern int cullingMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Mask to select which layers can trigger events on the camera.</para>
		/// </summary>
		public extern int eventMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How to perform per-layer culling for a Camera.</para>
		/// </summary>
		public extern bool layerCullSpherical
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Identifies what kind of camera this is.</para>
		/// </summary>
		public extern CameraType cameraType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction("CameraScripting::GetLayerCullDistances", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float[] GetLayerCullDistances();

		[FreeFunction("CameraScripting::SetLayerCullDistances", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetLayerCullDistances([NotNull] float[] d);

		/// <summary>
		///   <para>Per-layer culling distances.</para>
		/// </summary>
		public float[] layerCullDistances
		{
			get
			{
				return this.GetLayerCullDistances();
			}
			set
			{
				if (value.Length != 32)
				{
					throw new UnityException("Array needs to contain exactly 32 floats for layerCullDistances.");
				}
				this.SetLayerCullDistances(value);
			}
		}

		internal static extern int PreviewCullingLayer
		{
			[FreeFunction("CameraScripting::GetPreviewCullingLayer")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Whether or not the Camera will use occlusion culling during rendering.</para>
		/// </summary>
		public extern bool useOcclusionCulling
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Sets a custom matrix for the camera to use for all culling queries.</para>
		/// </summary>
		public Matrix4x4 cullingMatrix
		{
			get
			{
				Matrix4x4 matrix4x;
				this.get_cullingMatrix_Injected(out matrix4x);
				return matrix4x;
			}
			set
			{
				this.set_cullingMatrix_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Make culling queries reflect the camera's built in parameters.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetCullingMatrix();

		/// <summary>
		///   <para>The color with which the screen will be cleared.</para>
		/// </summary>
		public Color backgroundColor
		{
			get
			{
				Color color;
				this.get_backgroundColor_Injected(out color);
				return color;
			}
			set
			{
				this.set_backgroundColor_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>How the camera clears the background.</para>
		/// </summary>
		public extern CameraClearFlags clearFlags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How and if camera generates a depth texture.</para>
		/// </summary>
		public extern DepthTextureMode depthTextureMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Should the camera clear the stencil buffer after the deferred light pass?</para>
		/// </summary>
		public extern bool clearStencilAfterLightingPass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Make the camera render with shader replacement.</para>
		/// </summary>
		/// <param name="shader"></param>
		/// <param name="replacementTag"></param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetReplacementShader(Shader shader, string replacementTag);

		/// <summary>
		///   <para>Remove shader replacement from camera.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetReplacementShader();

		internal extern Camera.ProjectionMatrixMode projectionMatrixMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Enable [UsePhysicalProperties] to use physical camera properties to compute the field of view and the frustum.</para>
		/// </summary>
		public extern bool usePhysicalProperties
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The size of the camera sensor, expressed in millimeters.</para>
		/// </summary>
		public Vector2 sensorSize
		{
			get
			{
				Vector2 vector;
				this.get_sensorSize_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_sensorSize_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The lens offset of the camera. The lens shift is relative to the sensor size. For example, a lens shift of 0.5 offsets the sensor by half its horizontal size.</para>
		/// </summary>
		public Vector2 lensShift
		{
			get
			{
				Vector2 vector;
				this.get_lensShift_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_lensShift_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The camera focal length, expressed in millimeters. To use this property, enable UsePhysicalProperties.</para>
		/// </summary>
		public extern float focalLength
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal Vector3 GetLocalSpaceAim()
		{
			Vector3 vector;
			this.GetLocalSpaceAim_Injected(out vector);
			return vector;
		}

		/// <summary>
		///   <para>Where on the screen is the camera rendered in normalized coordinates.</para>
		/// </summary>
		[NativeProperty("NormalizedViewportRect")]
		public Rect rect
		{
			get
			{
				Rect rect;
				this.get_rect_Injected(out rect);
				return rect;
			}
			set
			{
				this.set_rect_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Where on the screen is the camera rendered in pixel coordinates.</para>
		/// </summary>
		[NativeProperty("ScreenViewportRect")]
		public Rect pixelRect
		{
			get
			{
				Rect rect;
				this.get_pixelRect_Injected(out rect);
				return rect;
			}
			set
			{
				this.set_pixelRect_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>How wide is the camera in pixels (not accounting for dynamic resolution scaling) (Read Only).</para>
		/// </summary>
		public extern int pixelWidth
		{
			[FreeFunction("CameraScripting::GetPixelWidth", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>How tall is the camera in pixels (not accounting for dynamic resolution scaling) (Read Only).</para>
		/// </summary>
		public extern int pixelHeight
		{
			[FreeFunction("CameraScripting::GetPixelHeight", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>How wide is the camera in pixels (accounting for dynamic resolution scaling) (Read Only).</para>
		/// </summary>
		public extern int scaledPixelWidth
		{
			[FreeFunction("CameraScripting::GetScaledPixelWidth", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>How tall is the camera in pixels (accounting for dynamic resolution scaling) (Read Only).</para>
		/// </summary>
		public extern int scaledPixelHeight
		{
			[FreeFunction("CameraScripting::GetScaledPixelHeight", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Destination render texture.</para>
		/// </summary>
		public extern RenderTexture targetTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Gets the temporary RenderTexture target for this Camera.</para>
		/// </summary>
		public extern RenderTexture activeTexture
		{
			[NativeName("GetCurrentTargetTexture")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Set the target display for this Camera.</para>
		/// </summary>
		public extern int targetDisplay
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction("CameraScripting::SetTargetBuffers", HasExplicitThis = true)]
		private void SetTargetBuffersImpl(RenderBuffer color, RenderBuffer depth)
		{
			this.SetTargetBuffersImpl_Injected(ref color, ref depth);
		}

		/// <summary>
		///   <para>Sets the Camera to render to the chosen buffers of one or more RenderTextures.</para>
		/// </summary>
		/// <param name="colorBuffer">The RenderBuffer(s) to which color information will be rendered.</param>
		/// <param name="depthBuffer">The RenderBuffer to which depth information will be rendered.</param>
		public void SetTargetBuffers(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
		{
			this.SetTargetBuffersImpl(colorBuffer, depthBuffer);
		}

		[FreeFunction("CameraScripting::SetTargetBuffers", HasExplicitThis = true)]
		private void SetTargetBuffersMRTImpl(RenderBuffer[] color, RenderBuffer depth)
		{
			this.SetTargetBuffersMRTImpl_Injected(color, ref depth);
		}

		/// <summary>
		///   <para>Sets the Camera to render to the chosen buffers of one or more RenderTextures.</para>
		/// </summary>
		/// <param name="colorBuffer">The RenderBuffer(s) to which color information will be rendered.</param>
		/// <param name="depthBuffer">The RenderBuffer to which depth information will be rendered.</param>
		public void SetTargetBuffers(RenderBuffer[] colorBuffer, RenderBuffer depthBuffer)
		{
			this.SetTargetBuffersMRTImpl(colorBuffer, depthBuffer);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string[] GetCameraBufferWarnings();

		/// <summary>
		///   <para>Matrix that transforms from camera space to world space (Read Only).</para>
		/// </summary>
		public Matrix4x4 cameraToWorldMatrix
		{
			get
			{
				Matrix4x4 matrix4x;
				this.get_cameraToWorldMatrix_Injected(out matrix4x);
				return matrix4x;
			}
		}

		/// <summary>
		///   <para>Matrix that transforms from world to camera space.</para>
		/// </summary>
		public Matrix4x4 worldToCameraMatrix
		{
			get
			{
				Matrix4x4 matrix4x;
				this.get_worldToCameraMatrix_Injected(out matrix4x);
				return matrix4x;
			}
			set
			{
				this.set_worldToCameraMatrix_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Set a custom projection matrix.</para>
		/// </summary>
		public Matrix4x4 projectionMatrix
		{
			get
			{
				Matrix4x4 matrix4x;
				this.get_projectionMatrix_Injected(out matrix4x);
				return matrix4x;
			}
			set
			{
				this.set_projectionMatrix_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Get or set the raw projection matrix with no camera offset (no jittering).</para>
		/// </summary>
		public Matrix4x4 nonJitteredProjectionMatrix
		{
			get
			{
				Matrix4x4 matrix4x;
				this.get_nonJitteredProjectionMatrix_Injected(out matrix4x);
				return matrix4x;
			}
			set
			{
				this.set_nonJitteredProjectionMatrix_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Should the jittered matrix be used for transparency rendering?</para>
		/// </summary>
		[NativeProperty("UseJitteredProjectionMatrixForTransparent")]
		public extern bool useJitteredProjectionMatrixForTransparentRendering
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Get the view projection matrix used on the last frame.</para>
		/// </summary>
		public Matrix4x4 previousViewProjectionMatrix
		{
			get
			{
				Matrix4x4 matrix4x;
				this.get_previousViewProjectionMatrix_Injected(out matrix4x);
				return matrix4x;
			}
		}

		/// <summary>
		///   <para>Make the rendering position reflect the camera's position in the scene.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetWorldToCameraMatrix();

		/// <summary>
		///   <para>Make the projection reflect normal camera's parameters.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetProjectionMatrix();

		/// <summary>
		///   <para>Calculates and returns oblique near-plane projection matrix.</para>
		/// </summary>
		/// <param name="clipPlane">Vector4 that describes a clip plane.</param>
		/// <returns>
		///   <para>Oblique near-plane projection matrix.</para>
		/// </returns>
		[FreeFunction("CameraScripting::CalculateObliqueMatrix", HasExplicitThis = true)]
		public Matrix4x4 CalculateObliqueMatrix(Vector4 clipPlane)
		{
			Matrix4x4 matrix4x;
			this.CalculateObliqueMatrix_Injected(ref clipPlane, out matrix4x);
			return matrix4x;
		}

		public Vector3 WorldToScreenPoint(Vector3 position, Camera.MonoOrStereoscopicEye eye)
		{
			Vector3 vector;
			this.WorldToScreenPoint_Injected(ref position, eye, out vector);
			return vector;
		}

		public Vector3 WorldToViewportPoint(Vector3 position, Camera.MonoOrStereoscopicEye eye)
		{
			Vector3 vector;
			this.WorldToViewportPoint_Injected(ref position, eye, out vector);
			return vector;
		}

		public Vector3 ViewportToWorldPoint(Vector3 position, Camera.MonoOrStereoscopicEye eye)
		{
			Vector3 vector;
			this.ViewportToWorldPoint_Injected(ref position, eye, out vector);
			return vector;
		}

		public Vector3 ScreenToWorldPoint(Vector3 position, Camera.MonoOrStereoscopicEye eye)
		{
			Vector3 vector;
			this.ScreenToWorldPoint_Injected(ref position, eye, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Transforms position from world space into screen space.</para>
		/// </summary>
		/// <param name="eye">Optional argument that can be used to specify which eye transform to use. Default is Mono.</param>
		/// <param name="position"></param>
		public Vector3 WorldToScreenPoint(Vector3 position)
		{
			return this.WorldToScreenPoint(position, Camera.MonoOrStereoscopicEye.Mono);
		}

		/// <summary>
		///   <para>Transforms position from world space into viewport space.</para>
		/// </summary>
		/// <param name="eye">Optional argument that can be used to specify which eye transform to use. Default is Mono.</param>
		/// <param name="position"></param>
		public Vector3 WorldToViewportPoint(Vector3 position)
		{
			return this.WorldToViewportPoint(position, Camera.MonoOrStereoscopicEye.Mono);
		}

		/// <summary>
		///   <para>Transforms position from viewport space into world space.</para>
		/// </summary>
		/// <param name="position">The 3d vector in Viewport space.</param>
		/// <returns>
		///   <para>The 3d vector in World space.</para>
		/// </returns>
		public Vector3 ViewportToWorldPoint(Vector3 position)
		{
			return this.ViewportToWorldPoint(position, Camera.MonoOrStereoscopicEye.Mono);
		}

		/// <summary>
		///   <para>Transforms position from screen space into world space.</para>
		/// </summary>
		/// <param name="position"></param>
		public Vector3 ScreenToWorldPoint(Vector3 position)
		{
			return this.ScreenToWorldPoint(position, Camera.MonoOrStereoscopicEye.Mono);
		}

		/// <summary>
		///   <para>Transforms position from screen space into viewport space.</para>
		/// </summary>
		/// <param name="position"></param>
		public Vector3 ScreenToViewportPoint(Vector3 position)
		{
			Vector3 vector;
			this.ScreenToViewportPoint_Injected(ref position, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Transforms position from viewport space into screen space.</para>
		/// </summary>
		/// <param name="position"></param>
		public Vector3 ViewportToScreenPoint(Vector3 position)
		{
			Vector3 vector;
			this.ViewportToScreenPoint_Injected(ref position, out vector);
			return vector;
		}

		internal Vector2 GetFrustumPlaneSizeAt(float distance)
		{
			Vector2 vector;
			this.GetFrustumPlaneSizeAt_Injected(distance, out vector);
			return vector;
		}

		private Ray ViewportPointToRay(Vector2 pos, Camera.MonoOrStereoscopicEye eye)
		{
			Ray ray;
			this.ViewportPointToRay_Injected(ref pos, eye, out ray);
			return ray;
		}

		public Ray ViewportPointToRay(Vector3 pos, Camera.MonoOrStereoscopicEye eye)
		{
			return this.ViewportPointToRay(pos, eye);
		}

		/// <summary>
		///   <para>Returns a ray going from camera through a viewport point.</para>
		/// </summary>
		/// <param name="eye">Optional argument that can be used to specify which eye transform to use. Default is Mono.</param>
		/// <param name="pos"></param>
		public Ray ViewportPointToRay(Vector3 pos)
		{
			return this.ViewportPointToRay(pos, Camera.MonoOrStereoscopicEye.Mono);
		}

		private Ray ScreenPointToRay(Vector2 pos, Camera.MonoOrStereoscopicEye eye)
		{
			Ray ray;
			this.ScreenPointToRay_Injected(ref pos, eye, out ray);
			return ray;
		}

		public Ray ScreenPointToRay(Vector3 pos, Camera.MonoOrStereoscopicEye eye)
		{
			return this.ScreenPointToRay(pos, eye);
		}

		/// <summary>
		///   <para>Returns a ray going from camera through a screen point.</para>
		/// </summary>
		/// <param name="eye">Optional argument that can be used to specify which eye transform to use. Default is Mono.</param>
		/// <param name="pos"></param>
		public Ray ScreenPointToRay(Vector3 pos)
		{
			return this.ScreenPointToRay(pos, Camera.MonoOrStereoscopicEye.Mono);
		}

		[FreeFunction("CameraScripting::RaycastTry", HasExplicitThis = true)]
		internal GameObject RaycastTry(Ray ray, float distance, int layerMask)
		{
			return this.RaycastTry_Injected(ref ray, distance, layerMask);
		}

		[FreeFunction("CameraScripting::RaycastTry2D", HasExplicitThis = true)]
		internal GameObject RaycastTry2D(Ray ray, float distance, int layerMask)
		{
			return this.RaycastTry2D_Injected(ref ray, distance, layerMask);
		}

		[FreeFunction("CameraScripting::CalculateViewportRayVectors", HasExplicitThis = true)]
		private void CalculateFrustumCornersInternal(Rect viewport, float z, Camera.MonoOrStereoscopicEye eye, [Out] Vector3[] outCorners)
		{
			this.CalculateFrustumCornersInternal_Injected(ref viewport, z, eye, outCorners);
		}

		public void CalculateFrustumCorners(Rect viewport, float z, Camera.MonoOrStereoscopicEye eye, Vector3[] outCorners)
		{
			if (outCorners == null)
			{
				throw new ArgumentNullException("outCorners");
			}
			if (outCorners.Length < 4)
			{
				throw new ArgumentException("outCorners minimum size is 4", "outCorners");
			}
			this.CalculateFrustumCornersInternal(viewport, z, eye, outCorners);
		}

		/// <summary>
		///   <para>Converts focal length to field of view.</para>
		/// </summary>
		/// <param name="focalLength">Focal length in millimeters.</param>
		/// <param name="sensorSize">Sensor size in millimeters. Use sensor height to get vertical field of view. Use sensor width to get horizontal field of view.</param>
		/// <returns>
		///   <para>Field of view in degrees.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float FocalLengthToFOV(float focalLength, float sensorSize);

		/// <summary>
		///   <para>Converts field of view to focal length. Use either sensor height and vertical field of view, or sensor width and horizontal field of view.</para>
		/// </summary>
		/// <param name="fov">Field of view in degrees.</param>
		/// <param name="sensorSize">Sensor size in millimeters.</param>
		/// <returns>
		///   <para>Focal length in millimeters.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float FOVToFocalLength(float fov, float sensorSize);

		/// <summary>
		///   <para>The first enabled camera tagged "MainCamera" (Read Only).</para>
		/// </summary>
		public static extern Camera main
		{
			[FreeFunction("FindMainCamera")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The camera we are currently rendering with, for low-level render control only (Read Only).</para>
		/// </summary>
		public static extern Camera current
		{
			[FreeFunction("GetCurrentCameraPtr")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>If not null, the camera will only render the contents of the specified scene.</para>
		/// </summary>
		public Scene scene
		{
			[FreeFunction("CameraScripting::GetScene", HasExplicitThis = true)]
			get
			{
				Scene scene;
				this.get_scene_Injected(out scene);
				return scene;
			}
			[FreeFunction("CameraScripting::SetScene", HasExplicitThis = true)]
			set
			{
				this.set_scene_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Stereoscopic rendering.</para>
		/// </summary>
		public extern bool stereoEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The distance between the virtual eyes. Use this to query or set the current eye separation. Note that most VR devices provide this value, in which case setting the value will have no effect.</para>
		/// </summary>
		public extern float stereoSeparation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Distance to a point where virtual eyes converge.</para>
		/// </summary>
		public extern float stereoConvergence
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Determines whether the stereo view matrices are suitable to allow for a single pass cull.</para>
		/// </summary>
		public extern bool areVRStereoViewMatricesWithinSingleCullTolerance
		{
			[NativeName("AreVRStereoViewMatricesWithinSingleCullTolerance")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Defines which eye of a VR display the Camera renders into.</para>
		/// </summary>
		public extern StereoTargetEyeMask stereoTargetEye
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns the eye that is currently rendering.
		/// If called when stereo is not enabled it will return Camera.MonoOrStereoscopicEye.Mono.
		///
		/// If called during a camera rendering callback such as OnRenderImage it will return the currently rendering eye.
		///
		/// If called outside of a rendering callback and stereo is enabled, it will return the default eye which is Camera.MonoOrStereoscopicEye.Left.</para>
		/// </summary>
		public extern Camera.MonoOrStereoscopicEye stereoActiveEye
		{
			[FreeFunction("CameraScripting::GetStereoActiveEye", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Matrix4x4 GetStereoNonJitteredProjectionMatrix(Camera.StereoscopicEye eye)
		{
			Matrix4x4 matrix4x;
			this.GetStereoNonJitteredProjectionMatrix_Injected(eye, out matrix4x);
			return matrix4x;
		}

		public Matrix4x4 GetStereoViewMatrix(Camera.StereoscopicEye eye)
		{
			Matrix4x4 matrix4x;
			this.GetStereoViewMatrix_Injected(eye, out matrix4x);
			return matrix4x;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CopyStereoDeviceProjectionMatrixToNonJittered(Camera.StereoscopicEye eye);

		public Matrix4x4 GetStereoProjectionMatrix(Camera.StereoscopicEye eye)
		{
			Matrix4x4 matrix4x;
			this.GetStereoProjectionMatrix_Injected(eye, out matrix4x);
			return matrix4x;
		}

		public void SetStereoProjectionMatrix(Camera.StereoscopicEye eye, Matrix4x4 matrix)
		{
			this.SetStereoProjectionMatrix_Injected(eye, ref matrix);
		}

		/// <summary>
		///   <para>Reset the camera to using the Unity computed projection matrices for all stereoscopic eyes.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetStereoProjectionMatrices();

		public void SetStereoViewMatrix(Camera.StereoscopicEye eye, Matrix4x4 matrix)
		{
			this.SetStereoViewMatrix_Injected(eye, ref matrix);
		}

		/// <summary>
		///   <para>Reset the camera to using the Unity computed view matrices for all stereoscopic eyes.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetStereoViewMatrices();

		[FreeFunction("CameraScripting::GetAllCamerasCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAllCamerasCount();

		[FreeFunction("CameraScripting::GetAllCameras")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAllCamerasImpl([NotNull] [Out] Camera[] cam);

		/// <summary>
		///   <para>The number of cameras in the current scene.</para>
		/// </summary>
		public static int allCamerasCount
		{
			get
			{
				return Camera.GetAllCamerasCount();
			}
		}

		/// <summary>
		///   <para>Returns all enabled cameras in the scene.</para>
		/// </summary>
		public static Camera[] allCameras
		{
			get
			{
				Camera[] array = new Camera[Camera.allCamerasCount];
				Camera.GetAllCamerasImpl(array);
				return array;
			}
		}

		/// <summary>
		///   <para>Fills an array of Camera with the current cameras in the scene, without allocating a new array.</para>
		/// </summary>
		/// <param name="cameras">An array to be filled up with cameras currently in the scene.</param>
		public static int GetAllCameras(Camera[] cameras)
		{
			if (cameras.Length < Camera.allCamerasCount)
			{
				throw new ArgumentException("Passed in array to fill with cameras is to small to hold the number of cameras. Use Camera.allCamerasCount to get the needed size.");
			}
			return Camera.GetAllCamerasImpl(cameras);
		}

		[FreeFunction("CameraScripting::RenderToCubemap", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool RenderToCubemapImpl(Texture tex, [DefaultValue("63")] int faceMask);

		/// <summary>
		///   <para>Render into a static cubemap from this camera.</para>
		/// </summary>
		/// <param name="cubemap">The cube map to render to.</param>
		/// <param name="faceMask">A bitmask which determines which of the six faces are rendered to.</param>
		/// <returns>
		///   <para>False if rendering fails, else true.</para>
		/// </returns>
		public bool RenderToCubemap(Cubemap cubemap, int faceMask)
		{
			return this.RenderToCubemapImpl(cubemap, faceMask);
		}

		public bool RenderToCubemap(Cubemap cubemap)
		{
			return this.RenderToCubemapImpl(cubemap, 63);
		}

		/// <summary>
		///   <para>Render into a cubemap from this camera.</para>
		/// </summary>
		/// <param name="faceMask">A bitfield indicating which cubemap faces should be rendered into.</param>
		/// <param name="cubemap">The texture to render to.</param>
		/// <returns>
		///   <para>False if rendering fails, else true.</para>
		/// </returns>
		public bool RenderToCubemap(RenderTexture cubemap, int faceMask)
		{
			return this.RenderToCubemapImpl(cubemap, faceMask);
		}

		public bool RenderToCubemap(RenderTexture cubemap)
		{
			return this.RenderToCubemapImpl(cubemap, 63);
		}

		[NativeName("RenderToCubemap")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool RenderToCubemapEyeImpl(RenderTexture cubemap, int faceMask, Camera.MonoOrStereoscopicEye stereoEye);

		public bool RenderToCubemap(RenderTexture cubemap, int faceMask, Camera.MonoOrStereoscopicEye stereoEye)
		{
			return this.RenderToCubemapEyeImpl(cubemap, faceMask, stereoEye);
		}

		/// <summary>
		///   <para>Render the camera manually.</para>
		/// </summary>
		[FreeFunction("CameraScripting::Render", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Render();

		/// <summary>
		///   <para>Render the camera with shader replacement.</para>
		/// </summary>
		/// <param name="shader"></param>
		/// <param name="replacementTag"></param>
		[FreeFunction("CameraScripting::RenderWithShader", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RenderWithShader(Shader shader, string replacementTag);

		[FreeFunction("CameraScripting::RenderDontRestore", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RenderDontRestore();

		[FreeFunction("CameraScripting::SetupCurrent")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetupCurrent(Camera cur);

		/// <summary>
		///   <para>Makes this camera's settings match other camera.</para>
		/// </summary>
		/// <param name="other">Copy camera settings to the other camera.</param>
		[FreeFunction("CameraScripting::CopyFrom", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CopyFrom(Camera other);

		/// <summary>
		///   <para>Number of command buffers set up on this camera (Read Only).</para>
		/// </summary>
		public extern int commandBufferCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Remove command buffers from execution at a specified place.</para>
		/// </summary>
		/// <param name="evt">When to execute the command buffer during rendering.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveCommandBuffers(CameraEvent evt);

		/// <summary>
		///   <para>Remove all command buffers set on this camera.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveAllCommandBuffers();

		[NativeName("AddCommandBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddCommandBufferImpl(CameraEvent evt, [NotNull] CommandBuffer buffer);

		[NativeName("AddCommandBufferAsync")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddCommandBufferAsyncImpl(CameraEvent evt, [NotNull] CommandBuffer buffer, ComputeQueueType queueType);

		[NativeName("RemoveCommandBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RemoveCommandBufferImpl(CameraEvent evt, [NotNull] CommandBuffer buffer);

		/// <summary>
		///   <para>Add a command buffer to be executed at a specified place.</para>
		/// </summary>
		/// <param name="evt">When to execute the command buffer during rendering.</param>
		/// <param name="buffer">The buffer to execute.</param>
		public void AddCommandBuffer(CameraEvent evt, CommandBuffer buffer)
		{
			if (!Enum.IsDefined(typeof(CameraEvent), evt))
			{
				throw new ArgumentException(string.Format("Invalid CameraEvent value \"{0}\".", (int)evt), "evt");
			}
			if (buffer == null)
			{
				throw new NullReferenceException("buffer is null");
			}
			this.AddCommandBufferImpl(evt, buffer);
		}

		/// <summary>
		///   <para>Adds a command buffer to the GPU's async compute queues and executes that command buffer when graphics processing reaches a given point.</para>
		/// </summary>
		/// <param name="evt">The point during the graphics processing at which this command buffer should commence on the GPU.</param>
		/// <param name="buffer">The buffer to execute.</param>
		/// <param name="queueType">The desired async compute queue type to execute the buffer on.</param>
		public void AddCommandBufferAsync(CameraEvent evt, CommandBuffer buffer, ComputeQueueType queueType)
		{
			if (!Enum.IsDefined(typeof(CameraEvent), evt))
			{
				throw new ArgumentException(string.Format("Invalid CameraEvent value \"{0}\".", (int)evt), "evt");
			}
			if (buffer == null)
			{
				throw new NullReferenceException("buffer is null");
			}
			this.AddCommandBufferAsyncImpl(evt, buffer, queueType);
		}

		/// <summary>
		///   <para>Remove command buffer from execution at a specified place.</para>
		/// </summary>
		/// <param name="evt">When to execute the command buffer during rendering.</param>
		/// <param name="buffer">The buffer to execute.</param>
		public void RemoveCommandBuffer(CameraEvent evt, CommandBuffer buffer)
		{
			if (!Enum.IsDefined(typeof(CameraEvent), evt))
			{
				throw new ArgumentException(string.Format("Invalid CameraEvent value \"{0}\".", (int)evt), "evt");
			}
			if (buffer == null)
			{
				throw new NullReferenceException("buffer is null");
			}
			this.RemoveCommandBufferImpl(evt, buffer);
		}

		[RequiredByNativeCode]
		private static void FireOnPreCull(Camera cam)
		{
			if (Camera.onPreCull != null)
			{
				Camera.onPreCull(cam);
			}
		}

		[RequiredByNativeCode]
		private static void FireOnPreRender(Camera cam)
		{
			if (Camera.onPreRender != null)
			{
				Camera.onPreRender(cam);
			}
		}

		[RequiredByNativeCode]
		private static void FireOnPostRender(Camera cam)
		{
			if (Camera.onPostRender != null)
			{
				Camera.onPostRender(cam);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_transparencySortAxis_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_transparencySortAxis_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_velocity_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_cullingMatrix_Injected(out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_cullingMatrix_Injected(ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_backgroundColor_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_backgroundColor_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_sensorSize_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_sensorSize_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_lensShift_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_lensShift_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetLocalSpaceAim_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_rect_Injected(out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_rect_Injected(ref Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_pixelRect_Injected(out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_pixelRect_Injected(ref Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTargetBuffersImpl_Injected(ref RenderBuffer color, ref RenderBuffer depth);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTargetBuffersMRTImpl_Injected(RenderBuffer[] color, ref RenderBuffer depth);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_cameraToWorldMatrix_Injected(out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_worldToCameraMatrix_Injected(out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_worldToCameraMatrix_Injected(ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_projectionMatrix_Injected(out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_projectionMatrix_Injected(ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_nonJitteredProjectionMatrix_Injected(out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_nonJitteredProjectionMatrix_Injected(ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_previousViewProjectionMatrix_Injected(out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void CalculateObliqueMatrix_Injected(ref Vector4 clipPlane, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void WorldToScreenPoint_Injected(ref Vector3 position, Camera.MonoOrStereoscopicEye eye, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void WorldToViewportPoint_Injected(ref Vector3 position, Camera.MonoOrStereoscopicEye eye, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ViewportToWorldPoint_Injected(ref Vector3 position, Camera.MonoOrStereoscopicEye eye, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ScreenToWorldPoint_Injected(ref Vector3 position, Camera.MonoOrStereoscopicEye eye, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ScreenToViewportPoint_Injected(ref Vector3 position, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ViewportToScreenPoint_Injected(ref Vector3 position, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetFrustumPlaneSizeAt_Injected(float distance, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ViewportPointToRay_Injected(ref Vector2 pos, Camera.MonoOrStereoscopicEye eye, out Ray ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ScreenPointToRay_Injected(ref Vector2 pos, Camera.MonoOrStereoscopicEye eye, out Ray ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern GameObject RaycastTry_Injected(ref Ray ray, float distance, int layerMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern GameObject RaycastTry2D_Injected(ref Ray ray, float distance, int layerMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void CalculateFrustumCornersInternal_Injected(ref Rect viewport, float z, Camera.MonoOrStereoscopicEye eye, [Out] Vector3[] outCorners);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_scene_Injected(out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_scene_Injected(ref Scene value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetStereoNonJitteredProjectionMatrix_Injected(Camera.StereoscopicEye eye, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetStereoViewMatrix_Injected(Camera.StereoscopicEye eye, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetStereoProjectionMatrix_Injected(Camera.StereoscopicEye eye, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetStereoProjectionMatrix_Injected(Camera.StereoscopicEye eye, ref Matrix4x4 matrix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetStereoViewMatrix_Injected(Camera.StereoscopicEye eye, ref Matrix4x4 matrix);

		/// <summary>
		///   <para>Event that is fired before any camera starts culling.</para>
		/// </summary>
		public static Camera.CameraCallback onPreCull;

		/// <summary>
		///   <para>Event that is fired before any camera starts rendering.</para>
		/// </summary>
		public static Camera.CameraCallback onPreRender;

		/// <summary>
		///   <para>Event that is fired after any camera finishes rendering.</para>
		/// </summary>
		public static Camera.CameraCallback onPostRender;

		internal enum ProjectionMatrixMode
		{
			Explicit,
			Implicit,
			PhysicalPropertiesBased
		}

		/// <summary>
		///   <para>Enum used to specify either the left or the right eye of a stereoscopic camera.</para>
		/// </summary>
		public enum StereoscopicEye
		{
			/// <summary>
			///   <para>Specifies the target to be the left eye.</para>
			/// </summary>
			Left,
			/// <summary>
			///   <para>Specifies the target to be the right eye.</para>
			/// </summary>
			Right
		}

		/// <summary>
		///   <para>A Camera eye corresponding to the left or right human eye for stereoscopic rendering, or neither for non-stereoscopic rendering.
		///
		/// A single Camera can render both left and right views in a single frame. Therefore, this enum describes which eye the Camera is currently rendering when returned by Camera.stereoActiveEye during a rendering callback (such as Camera.OnRenderImage), or which eye to act on when passed into a function.
		///
		/// The default value is Camera.MonoOrStereoscopicEye.Left, so Camera.MonoOrStereoscopicEye.Left may be returned by some methods or properties when called outside of rendering if stereoscopic rendering is enabled.</para>
		/// </summary>
		public enum MonoOrStereoscopicEye
		{
			/// <summary>
			///   <para>Camera eye corresponding to stereoscopic rendering of the left eye.</para>
			/// </summary>
			Left,
			/// <summary>
			///   <para>Camera eye corresponding to stereoscopic rendering of the right eye.</para>
			/// </summary>
			Right,
			/// <summary>
			///   <para>Camera eye corresponding to non-stereoscopic rendering.</para>
			/// </summary>
			Mono
		}

		/// <summary>
		///   <para>Delegate type for camera callbacks.</para>
		/// </summary>
		/// <param name="cam"></param>
		public delegate void CameraCallback(Camera cam);
	}
}
