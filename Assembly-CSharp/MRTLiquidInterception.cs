using System;
using UnityEngine;
using UnityEngine.Rendering;

public class MRTLiquidInterception : MonoBehaviour
{
	private void Start()
	{
		this.mrt = base.GetComponent<MultipleRenderTargetProxy>();
		MultipleRenderTargetProxy multipleRenderTargetProxy = this.mrt;
		multipleRenderTargetProxy.OnTexturesRecreated = (global::System.Action)Delegate.Combine(multipleRenderTargetProxy.OnTexturesRecreated, new global::System.Action(this.RecreateCommandBuffer));
		this.RecreateCommandBuffer();
	}

	private void RecreateCommandBuffer()
	{
		if (this.cb != null)
		{
			CameraController.Instance.baseCamera.RemoveCommandBuffer(this.interceptionTime, this.cb);
			this.cb.Clear();
		}
		else
		{
			this.cb = new CommandBuffer();
			this.cb.name = "Get MRT1 before rendering liquid";
		}
		this.cb.Blit(this.mrt.Textures[0], this.mrt.TexturesCopies[0]);
		this.cb.Blit(this.mrt.Textures[1], this.mrt.TexturesCopies[1]);
		this.cb.SetGlobalTexture(this.mrt.TexturesCopies[0].name, this.mrt.TexturesCopies[0]);
		this.cb.SetGlobalTexture(this.mrt.TexturesCopies[1].name, this.mrt.TexturesCopies[1]);
		this.mrts_identifier[0] = this.mrt.Textures[0];
		this.mrts_identifier[1] = this.mrt.Textures[1];
		this.mrts_identifier[2] = this.mrt.Textures[2];
		this.mrts_identifier_2[0] = this.mrt.Textures[0];
		this.mrts_identifier_2[1] = this.mrt.Textures[1];
		this.cb.SetRenderTarget(this.mrt.IsColouredOverlayBufferEnabled ? this.mrts_identifier : this.mrts_identifier_2, this.mrt.Textures[0].depthBuffer);
		this.cb.DrawRenderer(WaterCubes.Instance.waterRenderer, WaterCubes.Instance.material);
		CameraController.Instance.baseCamera.AddCommandBuffer(this.interceptionTime, this.cb);
	}

	private MultipleRenderTargetProxy mrt;

	private CommandBuffer cb;

	private CameraEvent interceptionTime = CameraEvent.BeforeForwardOpaque;

	private RenderTargetIdentifier[] mrts_identifier = new RenderTargetIdentifier[3];

	private RenderTargetIdentifier[] mrts_identifier_2 = new RenderTargetIdentifier[2];
}
