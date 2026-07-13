using System;

namespace UnityEngine.TextCore.Text
{
	public static class TextEventManager
	{
		public static void ON_PRE_RENDER_OBJECT_CHANGED()
		{
			TextEventManager.OnPreRenderObject_Event.Call();
		}

		public static void ON_MATERIAL_PROPERTY_CHANGED(bool isChanged, Material mat)
		{
			TextEventManager.MATERIAL_PROPERTY_EVENT.Call(isChanged, mat);
		}

		public static void ON_FONT_PROPERTY_CHANGED(bool isChanged, Object font)
		{
			TextEventManager.FONT_PROPERTY_EVENT.Call(isChanged, font);
		}

		public static void ON_SPRITE_ASSET_PROPERTY_CHANGED(bool isChanged, Object obj)
		{
			TextEventManager.SPRITE_ASSET_PROPERTY_EVENT.Call(isChanged, obj);
		}

		public static void ON_TEXTMESHPRO_PROPERTY_CHANGED(bool isChanged, Object obj)
		{
			TextEventManager.TEXTMESHPRO_PROPERTY_EVENT.Call(isChanged, obj);
		}

		public static void ON_DRAG_AND_DROP_MATERIAL_CHANGED(GameObject sender, Material currentMaterial, Material newMaterial)
		{
			TextEventManager.DRAG_AND_DROP_MATERIAL_EVENT.Call(sender, currentMaterial, newMaterial);
		}

		public static void ON_TEXT_STYLE_PROPERTY_CHANGED(bool isChanged)
		{
			TextEventManager.TEXT_STYLE_PROPERTY_EVENT.Call(isChanged);
		}

		public static void ON_COLOR_GRADIENT_PROPERTY_CHANGED(Object gradient)
		{
			TextEventManager.COLOR_GRADIENT_PROPERTY_EVENT.Call(gradient);
		}

		public static void ON_TEXT_CHANGED(Object obj)
		{
			TextEventManager.TEXT_CHANGED_EVENT.Call(obj);
		}

		public static void ON_TMP_SETTINGS_CHANGED()
		{
			TextEventManager.TMP_SETTINGS_PROPERTY_EVENT.Call();
		}

		public static void ON_RESOURCES_LOADED()
		{
			TextEventManager.RESOURCE_LOAD_EVENT.Call();
		}

		public static void ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED(bool isChanged, Object obj)
		{
			TextEventManager.TEXTMESHPRO_UGUI_PROPERTY_EVENT.Call(isChanged, obj);
		}

		public static readonly FastAction<bool, Material> MATERIAL_PROPERTY_EVENT = new FastAction<bool, Material>();

		public static readonly FastAction<bool, Object> FONT_PROPERTY_EVENT = new FastAction<bool, Object>();

		public static readonly FastAction<bool, Object> SPRITE_ASSET_PROPERTY_EVENT = new FastAction<bool, Object>();

		public static readonly FastAction<bool, Object> TEXTMESHPRO_PROPERTY_EVENT = new FastAction<bool, Object>();

		public static readonly FastAction<GameObject, Material, Material> DRAG_AND_DROP_MATERIAL_EVENT = new FastAction<GameObject, Material, Material>();

		public static readonly FastAction<bool> TEXT_STYLE_PROPERTY_EVENT = new FastAction<bool>();

		public static readonly FastAction<Object> COLOR_GRADIENT_PROPERTY_EVENT = new FastAction<Object>();

		public static readonly FastAction TMP_SETTINGS_PROPERTY_EVENT = new FastAction();

		public static readonly FastAction RESOURCE_LOAD_EVENT = new FastAction();

		public static readonly FastAction<bool, Object> TEXTMESHPRO_UGUI_PROPERTY_EVENT = new FastAction<bool, Object>();

		public static readonly FastAction OnPreRenderObject_Event = new FastAction();

		public static readonly FastAction<Object> TEXT_CHANGED_EVENT = new FastAction<Object>();
	}
}
