using System;
using UnityEngine;

public static class RenderUtil
{
	public static Material GetMaterial(Transform node, string name)
	{
		if (node != null)
		{
			Renderer component = node.GetComponent<Renderer>();
			if (component != null)
			{
				foreach (Material material in component.materials)
				{
					if (material != null && material.name.StartsWith(name))
					{
						return material;
					}
				}
			}
			foreach (object obj in node)
			{
				Transform transform = (Transform)obj;
				Material material2 = RenderUtil.GetMaterial(transform, name);
				if (material2 != null)
				{
					return material2;
				}
			}
		}
		return null;
	}

	public static void SetShader(Transform node, Shader shader)
	{
		if (node != null)
		{
			Renderer component = node.GetComponent<Renderer>();
			if (component != null)
			{
				foreach (Material material in component.materials)
				{
					if (material != null)
					{
						material.shader = shader;
					}
				}
			}
			foreach (object obj in node)
			{
				Transform transform = (Transform)obj;
				RenderUtil.SetShader(transform, shader);
			}
		}
	}

	public static void SetColor(Transform node, Color color)
	{
		if (node != null)
		{
			Renderer component = node.GetComponent<Renderer>();
			if (component != null)
			{
				foreach (Material material in component.materials)
				{
					if (material != null)
					{
						material.color = color;
					}
				}
			}
			foreach (object obj in node)
			{
				Transform transform = (Transform)obj;
				RenderUtil.SetColor(transform, color);
			}
		}
	}

	public static void SetMaterial(Transform node, Material material)
	{
		if (node != null)
		{
			Renderer component = node.GetComponent<Renderer>();
			if (component != null)
			{
				component.sharedMaterial = material;
			}
			foreach (object obj in node)
			{
				Transform transform = (Transform)obj;
				RenderUtil.SetMaterial(transform, material);
			}
		}
	}

	public static void ReplaceMaterial(Transform node, string name, Material material)
	{
		if (node != null)
		{
			Renderer component = node.GetComponent<Renderer>();
			if (component != null)
			{
				Material[] materials = component.materials;
				for (int i = 0; i < materials.Length; i++)
				{
					if (materials[i] != null && materials[i].name.StartsWith(name))
					{
						materials[i] = material;
					}
				}
				component.materials = materials;
			}
			foreach (object obj in node)
			{
				Transform transform = (Transform)obj;
				RenderUtil.ReplaceMaterial(transform, name, material);
			}
		}
	}

	public static void EnableRenderer(Transform node, bool is_enabled)
	{
		if (node != null)
		{
			Renderer component = node.GetComponent<Renderer>();
			if (component != null)
			{
				component.enabled = is_enabled;
			}
			foreach (object obj in node)
			{
				Transform transform = (Transform)obj;
				RenderUtil.EnableRenderer(transform, is_enabled);
			}
		}
	}

	public static void EnableLights(Transform node, bool is_enabled)
	{
		if (node != null)
		{
			Light component = node.GetComponent<Light>();
			if (component != null)
			{
				component.enabled = is_enabled;
			}
			foreach (object obj in node)
			{
				Transform transform = (Transform)obj;
				RenderUtil.EnableLights(transform, is_enabled);
			}
		}
	}

	public static void SetMaterialBlockColor(Transform node, string parameter_name, Color color)
	{
		if (node != null)
		{
			Renderer component = node.GetComponent<MeshRenderer>();
			if (component != null)
			{
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				component.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetColor(parameter_name, color);
				component.SetPropertyBlock(materialPropertyBlock);
			}
			foreach (object obj in node)
			{
				Transform transform = (Transform)obj;
				RenderUtil.SetMaterialBlockColor(transform, parameter_name, color);
			}
		}
	}

	public static void SetMaterialBlockTexture(Transform node, string parameter_name, Texture texture)
	{
		if (node != null)
		{
			Renderer component = node.GetComponent<MeshRenderer>();
			if (component != null)
			{
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				component.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetTexture(parameter_name, texture);
				component.SetPropertyBlock(materialPropertyBlock);
			}
			foreach (object obj in node)
			{
				Transform transform = (Transform)obj;
				RenderUtil.SetMaterialBlockTexture(transform, parameter_name, texture);
			}
		}
	}

	public static void AddMaterialBlockVector(Transform node, string parameter_name, Vector4 value)
	{
		int num = Shader.PropertyToID(parameter_name);
		RenderUtil.AddMaterialBlockVector(node, num, value);
	}

	public static void RemoveMaterialBlockVector(Transform node, string parameter_name)
	{
		int num = Shader.PropertyToID(parameter_name);
		RenderUtil.RemoveMaterialBlockVector(node, num);
	}

	public static void AddMaterialBlockVector(Transform node, int parameter, Vector4 value)
	{
		if (node != null)
		{
			KAnimRenderer component = node.GetComponent<KAnimRenderer>();
			if (component != null)
			{
				component.AddShaderVector(parameter, value);
			}
			foreach (object obj in node)
			{
				Transform transform = (Transform)obj;
				RenderUtil.AddMaterialBlockVector(transform, parameter, value);
			}
		}
	}

	public static void RemoveMaterialBlockVector(Transform node, int parameter)
	{
		if (node != null)
		{
			KAnimRenderer component = node.GetComponent<KAnimRenderer>();
			if (component != null)
			{
				component.RemoveShaderVector(parameter);
			}
			else
			{
				Renderer renderer = node.GetComponent<MeshRenderer>();
				if (renderer == null)
				{
					renderer = node.GetComponent<SpriteRenderer>();
				}
				if (renderer != null)
				{
					MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
					renderer.GetPropertyBlock(materialPropertyBlock);
					materialPropertyBlock.SetVector(parameter, Vector4.zero);
					renderer.SetPropertyBlock(materialPropertyBlock);
				}
			}
			foreach (object obj in node)
			{
				Transform transform = (Transform)obj;
				RenderUtil.RemoveMaterialBlockVector(transform, parameter);
			}
		}
	}
}
