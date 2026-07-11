using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
	public abstract class Mode
	{
		public static void Clear()
		{
			Mode.workingTargets.Clear();
		}

		public abstract SimViewMode ViewMode();

		public virtual void Enable()
		{
		}

		public virtual void Update()
		{
		}

		public virtual void Disable()
		{
		}

		public abstract string GetSoundName();

		public void RegisterSaveLoadListeners()
		{
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			saveManager.onRegister += this.OnSaveLoadRootRegistered;
			saveManager.onUnregister += this.OnSaveLoadRootUnregistered;
		}

		public void UnregisterSaveLoadListeners()
		{
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			saveManager.onRegister -= this.OnSaveLoadRootRegistered;
			saveManager.onUnregister -= this.OnSaveLoadRootUnregistered;
		}

		protected virtual void OnSaveLoadRootRegistered(SaveLoadRoot root)
		{
		}

		protected virtual void OnSaveLoadRootUnregistered(SaveLoadRoot root)
		{
		}

		protected void ProcessExistingSaveLoadRoots()
		{
			foreach (KeyValuePair<Tag, List<SaveLoadRoot>> keyValuePair in SaveLoader.Instance.saveManager.GetLists())
			{
				foreach (SaveLoadRoot saveLoadRoot in keyValuePair.Value)
				{
					this.OnSaveLoadRootRegistered(saveLoadRoot);
				}
			}
		}

		protected static UniformGrid<T> PopulatePartition<T>(ICollection<Tag> tags) where T : IUniformGridObject
		{
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			Dictionary<Tag, List<SaveLoadRoot>> lists = saveManager.GetLists();
			UniformGrid<T> uniformGrid = new UniformGrid<T>(Grid.WidthInCells, Grid.HeightInCells, 8, 8);
			foreach (Tag tag in tags)
			{
				List<SaveLoadRoot> list = null;
				if (lists.TryGetValue(tag, out list))
				{
					foreach (SaveLoadRoot saveLoadRoot in list)
					{
						T component = saveLoadRoot.GetComponent<T>();
						if (component != null)
						{
							uniformGrid.Add(component);
						}
					}
				}
			}
			return uniformGrid;
		}

		protected static void ResetDisplayValues<T>(ICollection<T> targets) where T : MonoBehaviour
		{
			foreach (T t in targets)
			{
				if (!(t == null))
				{
					KBatchedAnimController component = t.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						Mode.ResetDisplayValues(component);
					}
				}
			}
		}

		protected static void ResetDisplayValues(KBatchedAnimController controller)
		{
			controller.SetLayer(0);
			controller.HighlightColour = Color.clear;
			controller.TintColour = Color.white;
			controller.SetLayer(controller.GetComponent<KPrefabID>().defaultLayer);
		}

		protected static void RemoveOffscreenTargets<T>(ICollection<T> targets, Vector2I min, Vector2I max, Action<T> on_removed = null) where T : KMonoBehaviour
		{
			Mode.ClearOutsideViewObjects<T>(targets, min, max, null, delegate(T cmp)
			{
				if (cmp != null)
				{
					KBatchedAnimController component = cmp.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						Mode.ResetDisplayValues(component);
					}
					if (on_removed != null)
					{
						on_removed(cmp);
					}
				}
			});
			Mode.workingTargets.Clear();
		}

		protected static void ClearOutsideViewObjects<T>(ICollection<T> targets, Vector2I vis_min, Vector2I vis_max, ICollection<Tag> item_ids, Action<T> on_remove) where T : KMonoBehaviour
		{
			Mode.workingTargets.Clear();
			foreach (T t in targets)
			{
				if (!(t == null))
				{
					Vector2I vector2I = Grid.PosToXY(t.transform.GetPosition());
					if (!(vis_min <= vector2I) || !(vector2I <= vis_max))
					{
						Mode.workingTargets.Add(t);
					}
					else
					{
						KPrefabID component = t.GetComponent<KPrefabID>();
						if (item_ids != null && !item_ids.Contains(component.PrefabTag))
						{
							Mode.workingTargets.Add(t);
						}
					}
				}
			}
			foreach (KMonoBehaviour kmonoBehaviour in Mode.workingTargets)
			{
				T t2 = (T)((object)kmonoBehaviour);
				if (!(t2 == null))
				{
					if (on_remove != null)
					{
						on_remove(t2);
					}
					targets.Remove(t2);
				}
			}
			Mode.workingTargets.Clear();
		}

		protected static void RemoveOffscreenTargets<T>(ICollection<T> targets, ICollection<T> working_targets, Vector2I vis_min, Vector2I vis_max, Action<T> on_removed = null, Func<T, bool> special_clear_condition = null) where T : IUniformGridObject
		{
			Mode.ClearOutsideViewObjects<T>(targets, working_targets, vis_min, vis_max, delegate(T cmp)
			{
				if (cmp != null && on_removed != null)
				{
					on_removed(cmp);
				}
			});
			if (special_clear_condition != null)
			{
				working_targets.Clear();
				foreach (T t in targets)
				{
					if (special_clear_condition(t))
					{
						working_targets.Add(t);
					}
				}
				foreach (T t2 in working_targets)
				{
					if (t2 != null)
					{
						if (on_removed != null)
						{
							on_removed(t2);
						}
						targets.Remove(t2);
					}
				}
				working_targets.Clear();
			}
		}

		protected static void ClearOutsideViewObjects<T>(ICollection<T> targets, ICollection<T> working_targets, Vector2I vis_min, Vector2I vis_max, Action<T> on_removed = null) where T : IUniformGridObject
		{
			working_targets.Clear();
			foreach (T t in targets)
			{
				if (t != null)
				{
					Vector2 vector = t.PosMin();
					Vector2 vector2 = t.PosMin();
					if (vector2.x < (float)vis_min.x || vector2.y < (float)vis_min.y || (float)vis_max.x < vector.x || (float)vis_max.y < vector.y)
					{
						working_targets.Add(t);
					}
				}
			}
			foreach (T t2 in working_targets)
			{
				if (t2 != null)
				{
					if (on_removed != null)
					{
						on_removed(t2);
					}
					targets.Remove(t2);
				}
			}
			working_targets.Clear();
		}

		protected static float GetDefaultDepth(KMonoBehaviour cmp)
		{
			BuildingComplete component = cmp.GetComponent<BuildingComplete>();
			float num;
			if (component != null)
			{
				num = Grid.GetLayerZ(component.Def.SceneLayer);
			}
			else
			{
				num = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
			}
			return num;
		}

		protected void UpdateHighlightTypeOverlay<T>(Vector2I min, Vector2I max, ICollection<T> targets, ICollection<Tag> item_ids, ColorHighlightCondition[] highlights, BringToFrontLayerSetting bringToFrontSetting, int layer) where T : KMonoBehaviour
		{
			foreach (T t in targets)
			{
				if (!(t == null))
				{
					Vector3 position = t.transform.GetPosition();
					int num = Grid.PosToCell(position);
					if (Grid.IsValidCell(num))
					{
						if (Grid.IsVisible(num))
						{
							if (min <= position && position <= max)
							{
								KBatchedAnimController component = t.GetComponent<KBatchedAnimController>();
								if (!(component == null))
								{
									int num2 = 0;
									Color32 color = Color.clear;
									if (highlights != null)
									{
										foreach (ColorHighlightCondition colorHighlightCondition in highlights)
										{
											if (colorHighlightCondition.highlight_condition(t))
											{
												color = colorHighlightCondition.highlight_color(t);
												num2 = layer;
												break;
											}
										}
									}
									if (bringToFrontSetting != BringToFrontLayerSetting.Constant)
									{
										if (bringToFrontSetting == BringToFrontLayerSetting.Conditional)
										{
											component.SetLayer(num2);
										}
									}
									else
									{
										component.SetLayer(layer);
									}
									component.HighlightColour = color;
								}
							}
						}
					}
				}
			}
		}

		protected void DisableHighlightTypeOverlay<T>(ICollection<T> targets) where T : KMonoBehaviour
		{
			Color32 color = Color.clear;
			foreach (T t in targets)
			{
				if (!(t == null))
				{
					KBatchedAnimController component = t.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						component.HighlightColour = color;
						component.SetLayer(0);
					}
				}
			}
			targets.Clear();
		}

		protected void AddTargetIfVisible<T>(T instance, Vector2I vis_min, Vector2I vis_max, ICollection<T> targets, int layer, Action<T> on_added = null, Func<KMonoBehaviour, bool> should_add = null) where T : IUniformGridObject
		{
			if (instance.Equals(null))
			{
				return;
			}
			Vector2 vector = instance.PosMin();
			Vector2 vector2 = instance.PosMax();
			if (vector2.x < (float)vis_min.x || vector2.y < (float)vis_min.y || vector.x > (float)vis_max.x || vector.y > (float)vis_max.y)
			{
				return;
			}
			if (targets.Contains(instance))
			{
				return;
			}
			bool flag = false;
			int num = (int)vector.y;
			while ((float)num <= vector2.y)
			{
				int num2 = (int)vector.x;
				while ((float)num2 <= vector2.x)
				{
					int num3 = Grid.XYToCell(num2, num);
					if (Grid.Visible[num3] > 20 || !PropertyTextures.IsFogOfWarEnabled)
					{
						flag = true;
						break;
					}
					num2++;
				}
				num++;
			}
			if (flag)
			{
				bool flag2 = true;
				KMonoBehaviour kmonoBehaviour = instance as KMonoBehaviour;
				if (kmonoBehaviour != null && should_add != null)
				{
					flag2 = should_add(kmonoBehaviour);
				}
				if (flag2)
				{
					if (kmonoBehaviour != null)
					{
						KBatchedAnimController component = kmonoBehaviour.GetComponent<KBatchedAnimController>();
						if (component != null)
						{
							component.SetLayer(layer);
						}
					}
					targets.Add(instance);
					if (on_added != null)
					{
						on_added(instance);
					}
				}
			}
		}

		private static List<KMonoBehaviour> workingTargets = new List<KMonoBehaviour>();
	}
}
