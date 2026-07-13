using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	internal static class WorldSpaceInput
	{
		public static VisualElement Pick3D(UIDocument document, Ray worldRay)
		{
			Ray ray = document.transform.worldToLocalMatrix.TransformRay(worldRay);
			return WorldSpaceInput.Pick_Internal(document, ray, null);
		}

		public static void PickAll3D(UIDocument document, Ray worldRay, List<VisualElement> outResults)
		{
			Ray ray = document.transform.worldToLocalMatrix.TransformRay(worldRay);
			WorldSpaceInput.Pick_Internal(document, ray, outResults);
		}

		public static VisualElement Pick3D(UIDocument document, Ray worldRay, out float distance)
		{
			Ray ray = document.transform.worldToLocalMatrix.TransformRay(worldRay);
			VisualElement visualElement = WorldSpaceInput.Pick_Internal(document, ray, null);
			bool flag = visualElement != null;
			if (flag)
			{
				float num;
				Vector3 vector;
				visualElement.IntersectWorldRay(ray, out num, out vector);
				Vector3 vector2 = ray.origin + ray.direction * num;
				Vector3 vector3 = document.transform.TransformPoint(vector2);
				distance = Vector3.Distance(worldRay.origin, vector3);
			}
			else
			{
				distance = float.PositiveInfinity;
			}
			return visualElement;
		}

		public static bool Pick3D(UIDocument document, Ray worldRay, out WorldSpaceInput.PickResult pickResult)
		{
			Ray ray = document.transform.worldToLocalMatrix.TransformRay(worldRay);
			VisualElement visualElement = WorldSpaceInput.Pick_Internal(document, ray, null);
			bool flag = visualElement == null;
			bool flag2;
			if (flag)
			{
				pickResult = WorldSpaceInput.PickResult.Empty;
				flag2 = false;
			}
			else
			{
				float num;
				Vector3 vector;
				visualElement.IntersectWorldRay(ray, out num, out vector);
				Vector3 vector2 = ray.origin + ray.direction * num;
				Vector3 vector3 = document.transform.TransformPoint(vector2);
				float num2 = Vector3.Distance(worldRay.origin, vector3);
				pickResult = new WorldSpaceInput.PickResult
				{
					document = document,
					pickedElement = visualElement,
					distance = num2
				};
				pickResult.ComputeCollisionData(worldRay);
				flag2 = true;
			}
			return flag2;
		}

		public static bool PickElement3D(VisualElement element, Ray worldRay, out WorldSpaceInput.PickResult pickResult, bool acceptOutside = false)
		{
			UIDocument uidocument = UIDocument.FindRootUIDocument(element);
			bool flag = uidocument == null;
			if (flag)
			{
				throw new ArgumentException("Element must be part of a UI Document.");
			}
			Ray ray = uidocument.transform.worldToLocalMatrix.TransformRay(worldRay);
			float num;
			Vector3 vector;
			bool flag2 = !element.IntersectWorldRay(ray, out num, out vector) && (!acceptOutside || num <= 0f);
			bool flag3;
			if (flag2)
			{
				pickResult = WorldSpaceInput.PickResult.Empty;
				flag3 = false;
			}
			else
			{
				pickResult = new WorldSpaceInput.PickResult
				{
					document = uidocument,
					pickedElement = element,
					distance = num
				};
				pickResult.ComputeCollisionData(worldRay);
				flag3 = true;
			}
			return flag3;
		}

		public static WorldSpaceInput.PickResult PickDocument3D(Ray worldRay, float maxDistance = float.PositiveInfinity, int layerMask = -5)
		{
			WorldSpaceInput.PickResult pickResult = new WorldSpaceInput.PickResult
			{
				distance = float.PositiveInfinity
			};
			float num = 0f;
			Ray ray = worldRay;
			int num2 = 0;
			while (num < maxDistance)
			{
				bool flag = ++num2 > 100;
				if (flag)
				{
					Debug.LogWarning("PickDocument3D exceeded iteration limit of " + 100.ToString() + ". Returned values may be incorrect.");
					break;
				}
				RaycastHit raycastHit;
				bool flag2 = !Physics.Raycast(ray, out raycastHit, maxDistance - num, layerMask, QueryTriggerInteraction.Collide);
				if (flag2)
				{
					break;
				}
				float num3 = raycastHit.distance + num;
				UIDocument componentInParent = raycastHit.collider.GetComponentInParent<UIDocument>(true);
				bool flag3 = componentInParent == null;
				if (flag3)
				{
					bool flag4 = num3 < pickResult.distance;
					if (flag4)
					{
						pickResult = new WorldSpaceInput.PickResult
						{
							distance = num3,
							collider = raycastHit.collider,
							normal = raycastHit.normal,
							point = raycastHit.point,
							localPoint = raycastHit.point
						};
					}
					break;
				}
				float num4 = raycastHit.distance + 0.001f;
				ray.origin += ray.direction * num4;
				num += num4;
				bool flag5 = componentInParent.containerPanel == null;
				if (!flag5)
				{
					Bounds picking3DLocalBounds = WorldSpaceInput.GetPicking3DLocalBounds(componentInParent.rootVisualElement);
					ref Matrix4x4 worldTransformRef = ref componentInParent.rootVisualElement.worldTransformRef;
					Vector3 vector = worldTransformRef.MultiplyPoint3x4(picking3DLocalBounds.center);
					Vector3 vector2 = worldTransformRef.MultiplyVector(picking3DLocalBounds.size);
					Matrix4x4 localToWorldMatrix = componentInParent.transform.localToWorldMatrix;
					Vector3 vector3 = worldTransformRef.MultiplyPoint3x4(vector);
					Vector3 vector4 = localToWorldMatrix.MultiplyVector(vector2);
					float num5 = Vector3.Distance(worldRay.origin, vector3) + vector4.magnitude / 2f + 0.001f;
					maxDistance = Mathf.Min(maxDistance, num + num5);
					bool flag6 = num3 >= pickResult.distance;
					if (!flag6)
					{
						VisualElement visualElement = WorldSpaceInput.Pick3D(componentInParent, worldRay, out num3);
						bool flag7 = visualElement != null && num3 <= maxDistance && num3 < pickResult.distance;
						if (flag7)
						{
							pickResult = new WorldSpaceInput.PickResult
							{
								collider = raycastHit.collider,
								pickedElement = visualElement,
								document = componentInParent,
								distance = num3
							};
							pickResult.ComputeCollisionData(worldRay);
						}
					}
				}
			}
			return pickResult;
		}

		internal static VisualElement Pick_Internal(UIDocument document, Ray documentRay, List<VisualElement> outResults = null)
		{
			document.containerPanel.ValidateLayout();
			VisualElement rootVisualElement = document.rootVisualElement;
			Ray ray = rootVisualElement.WorldToLocal(documentRay);
			return WorldSpaceInput.PerformPick(rootVisualElement, ray, null);
		}

		[VisibleToOtherModules(new string[] { "Assembly-CSharp-testable" })]
		internal static VisualElement PerformPick(VisualElement root, Ray ray, List<VisualElement> outResults)
		{
			return root.needs3DBounds ? WorldSpaceInput.PerformPick3D(root, ray, outResults) : WorldSpaceInput.PerformPick2D(root, ray, outResults);
		}

		private static VisualElement PerformPick2D(VisualElement root, Ray ray, List<VisualElement> outResults)
		{
			Vector3 vector;
			root.IntersectLocalRay(ray, out vector);
			return WorldSpaceInput.PerformPick2D_LocalPoint(root, vector, outResults);
		}

		private static VisualElement PerformPick3D(VisualElement root, Ray ray, List<VisualElement> outResults)
		{
			bool flag = root.resolvedStyle.display == DisplayStyle.None;
			VisualElement visualElement;
			if (flag)
			{
				visualElement = null;
			}
			else
			{
				bool flag2 = root.pickingMode == PickingMode.Ignore && root.hierarchy.childCount == 0;
				if (flag2)
				{
					visualElement = null;
				}
				else
				{
					bool flag3 = !WorldSpaceInput.GetPicking3DLocalBounds(root).IntersectRay(ray);
					if (flag3)
					{
						visualElement = null;
					}
					else
					{
						Vector3 vector;
						bool flag4 = root.IntersectLocalRay(ray, out vector) && root.ContainsPoint(vector);
						bool flag5 = !flag4 && root.ShouldClip();
						if (flag5)
						{
							visualElement = null;
						}
						else
						{
							VisualElement visualElement2 = null;
							int childCount = root.hierarchy.childCount;
							for (int i = childCount - 1; i >= 0; i--)
							{
								VisualElement visualElement3 = root.hierarchy[i];
								Ray ray2 = root.ChangeCoordinatesTo(visualElement3, ray);
								VisualElement visualElement4 = WorldSpaceInput.PerformPick(visualElement3, ray2, outResults);
								bool flag6 = visualElement2 == null && visualElement4 != null;
								if (flag6)
								{
									bool flag7 = outResults == null;
									if (flag7)
									{
										return visualElement4;
									}
									visualElement2 = visualElement4;
								}
							}
							bool flag8 = root.visible && root.pickingMode == PickingMode.Position && flag4;
							if (flag8)
							{
								if (outResults != null)
								{
									outResults.Add(root);
								}
								bool flag9 = visualElement2 == null;
								if (flag9)
								{
									visualElement2 = root;
								}
							}
							visualElement = visualElement2;
						}
					}
				}
			}
			return visualElement;
		}

		private static VisualElement PerformPick2D_LocalPoint(VisualElement root, Vector3 localPoint, List<VisualElement> picked = null)
		{
			bool flag = root.resolvedStyle.display == DisplayStyle.None;
			VisualElement visualElement;
			if (flag)
			{
				visualElement = null;
			}
			else
			{
				bool flag2 = root.pickingMode == PickingMode.Ignore && root.hierarchy.childCount == 0;
				if (flag2)
				{
					visualElement = null;
				}
				else
				{
					bool flag3 = !root.boundingBox.Contains(localPoint);
					if (flag3)
					{
						visualElement = null;
					}
					else
					{
						bool flag4 = root.ContainsPoint(localPoint);
						bool flag5 = !flag4 && root.ShouldClip();
						if (flag5)
						{
							visualElement = null;
						}
						else
						{
							VisualElement visualElement2 = null;
							int childCount = root.hierarchy.childCount;
							for (int i = childCount - 1; i >= 0; i--)
							{
								VisualElement visualElement3 = root.hierarchy[i];
								Vector2 vector = root.ChangeCoordinatesTo(visualElement3, localPoint);
								VisualElement visualElement4 = WorldSpaceInput.PerformPick2D_LocalPoint(visualElement3, vector, picked);
								bool flag6 = visualElement2 == null && visualElement4 != null;
								if (flag6)
								{
									bool flag7 = picked == null;
									if (flag7)
									{
										return visualElement4;
									}
									visualElement2 = visualElement4;
								}
							}
							bool flag8 = root.visible && root.pickingMode == PickingMode.Position && flag4;
							if (flag8)
							{
								if (picked != null)
								{
									picked.Add(root);
								}
								bool flag9 = visualElement2 == null;
								if (flag9)
								{
									visualElement2 = root;
								}
							}
							visualElement = visualElement2;
						}
					}
				}
			}
			return visualElement;
		}

		internal static Bounds GetPicking3DWorldBounds(VisualElement ve)
		{
			Bounds picking3DLocalBounds = WorldSpaceInput.GetPicking3DLocalBounds(ve);
			VisualElement.TransformAlignedBounds(ve.worldTransformRef, ref picking3DLocalBounds);
			return picking3DLocalBounds;
		}

		internal static Bounds GetPicking3DLocalBounds(VisualElement ve)
		{
			bool needs3DBounds = ve.needs3DBounds;
			Bounds bounds;
			if (needs3DBounds)
			{
				bounds = ve.localBoundsPicking3D;
			}
			else
			{
				Rect boundingBox = ve.boundingBox;
				bounds = new Bounds(boundingBox.center, boundingBox.size);
			}
			return bounds;
		}

		public static Vector3 LocalPointToGameObjectWorldSpace(VisualElement element, Vector3 localPoint)
		{
			UIDocument uidocument = UIDocument.FindRootUIDocument(element);
			bool flag = uidocument == null;
			if (flag)
			{
				throw new ArgumentException("Element must be part of a UI Document.");
			}
			Vector3 vector = element.LocalToWorld3D(localPoint);
			return uidocument.transform.TransformPoint(vector);
		}

		public static Vector3 LocalDeltaToGameObjectWorldSpace(VisualElement element, Vector3 localDelta)
		{
			return WorldSpaceInput.LocalPointToGameObjectWorldSpace(element, localDelta) - WorldSpaceInput.LocalPointToGameObjectWorldSpace(element, Vector3.zero);
		}

		public static Vector3 GameObjectWorldSpaceToLocalPoint(VisualElement element, Vector3 worldPoint)
		{
			UIDocument uidocument = UIDocument.FindRootUIDocument(element);
			bool flag = uidocument == null;
			if (flag)
			{
				throw new ArgumentException("Element must be part of a UI Document.");
			}
			Vector3 vector = uidocument.transform.InverseTransformPoint(worldPoint);
			return element.WorldToLocal3D(vector);
		}

		public static Vector3 GameObjectWorldSpaceToLocalDelta(VisualElement element, Vector3 worldDelta)
		{
			return WorldSpaceInput.GameObjectWorldSpaceToLocalPoint(element, worldDelta) - WorldSpaceInput.GameObjectWorldSpaceToLocalPoint(element, Vector3.zero);
		}

		public struct PickResult
		{
			internal void ComputeCollisionData(Ray ray)
			{
				this.point = ray.origin + ray.direction * this.distance;
				bool flag = this.document != null && this.pickedElement != null;
				if (flag)
				{
					this.localPoint = this.pickedElement.worldTransformInverse.MultiplyPoint3x4(this.document.transform.InverseTransformPoint(this.point));
					this.normal = this.document.transform.TransformDirection(this.pickedElement.worldTransformRef.MultiplyVector(Vector3.forward));
				}
			}

			public static readonly WorldSpaceInput.PickResult Empty = new WorldSpaceInput.PickResult
			{
				distance = float.PositiveInfinity
			};

			public Collider collider;

			public UIDocument document;

			public VisualElement pickedElement;

			public float distance;

			public Vector3 normal;

			public Vector3 point;

			public Vector3 localPoint;
		}
	}
}
