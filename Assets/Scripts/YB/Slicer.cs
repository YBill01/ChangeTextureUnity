using Parabox.CSG;
using UnityEngine;
using YBSlice.Objects;

namespace YBSlice
{
	public class Slicer
	{
		public bool TrySlice(SlicerObject sourceObject, Vector3[] points, out SlicerObject partObject1, out SlicerObject partObject2)
		{
			partObject1 = null;
			partObject2 = null;

			try
			{
				if (!IsClockwise(points))
				{
					System.Array.Reverse(points);
				}

				FindNearAndFarPlanes(sourceObject.Bounds, Camera.main.transform, out Plane nearPlane, out Plane farPlane);

				Vector3[] projectedNear = ProjectPointsOnPlane(points, nearPlane, Camera.main.transform);
				Vector3[] projectedFar = ProjectPointsOnPlane(points, farPlane, Camera.main.transform);

				Mesh cuttingMesh = CreateMeshFromProjections(projectedNear, projectedFar);

				GameObject cuttingObject = new GameObject("CuttingObject");
				cuttingObject.AddComponent<MeshFilter>()
					.mesh = cuttingMesh;
				cuttingObject.AddComponent<MeshRenderer>()
					.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

				Model resultModel1 = CSG.Intersect(sourceObject.gameObject, cuttingObject);
				Model resultModel2 = CSG.Subtract(sourceObject.gameObject, cuttingObject);

				partObject1 = CompositeSlaicerObject($"{sourceObject.name}_part01", sourceObject, resultModel1);
				partObject2 = CompositeSlaicerObject($"{sourceObject.name}_part02", sourceObject, resultModel2);

				GameObject.Destroy(cuttingObject);
				GameObject.Destroy(sourceObject.gameObject);

				return true;
			}
			catch
			{
				return false;
			}
		}

		private void FindNearAndFarPlanes(Bounds bounds, Transform cameraTransform, out Plane nearPlane, out Plane farPlane)
		{
			Vector3[] corners = GetBoundsCorners(bounds);

			Vector3 nearPoint = corners[0];
			Vector3 farPoint = corners[0];

			Vector3 camPosition = cameraTransform.position;
			Vector3 camNormal = cameraTransform.forward;

			float minDist = Vector3.Distance(camPosition, nearPoint);
			float maxDist = minDist;
			foreach (Vector3 corner in corners)
			{
				float dist = Vector3.Distance(camPosition, corner);
				if (dist < minDist) { minDist = dist; nearPoint = corner; }
				if (dist > maxDist) { maxDist = dist; farPoint = corner; }
			}

			if (Vector3.Dot(nearPoint - camPosition, camNormal) < 0)
			{
				nearPoint = camPosition + (camNormal * 0.02f);
			}

			nearPlane = new Plane(camNormal, nearPoint - (camNormal * 0.01f));
			farPlane = new Plane(camNormal, farPoint + (camNormal * 0.01f));
		}

		private Vector3[] GetBoundsCorners(Bounds bounds)
		{
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;

			return new Vector3[]
			{
				new Vector3(min.x, min.y, min.z), new Vector3(min.x, min.y, max.z),
				new Vector3(min.x, max.y, min.z), new Vector3(min.x, max.y, max.z),
				new Vector3(max.x, min.y, min.z), new Vector3(max.x, min.y, max.z),
				new Vector3(max.x, max.y, min.z), new Vector3(max.x, max.y, max.z)
			};
		}

		private Vector3[] ProjectPointsOnPlane(Vector3[] points, Plane plane, Transform cameraTransform)
		{
			Vector3[] projectedPoints = new Vector3[points.Length];

			for (int i = 0; i < points.Length; i++)
			{
				projectedPoints[i] = ProjectPointOnPlane(points[i], plane, cameraTransform);
			}

			return projectedPoints;
		}

		private Vector3 ProjectPointOnPlane(Vector3 point, Plane plane, Transform cameraTransform)
		{
			Vector3 rayDir = (point - cameraTransform.position).normalized;
			Ray ray = new Ray(cameraTransform.position, rayDir);

			if (plane.Raycast(ray, out float enter))
			{
				return ray.GetPoint(enter);
			}

			return point;
		}

		private SlicerObject CompositeSlaicerObject(string name, SlicerObject source, Model model)
		{
			GameObject composite = new GameObject(name);

			Mesh mesh = model.mesh;

			mesh.subMeshCount = 1;
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();

			composite.transform.SetParent(source.transform.parent, false);
			composite.isStatic = true;

			composite.AddComponent<MeshFilter>().sharedMesh = mesh;
			composite.AddComponent<MeshRenderer>().sharedMaterials = source.GetComponent<MeshRenderer>().materials;
			composite.AddComponent<MeshCollider>();
			composite.GetComponent<Renderer>().lightmapIndex = source.GetComponent<Renderer>().lightmapIndex;
			composite.GetComponent<Renderer>().lightmapScaleOffset = source.GetComponent<Renderer>().lightmapScaleOffset;

			SlicerObject result = composite.AddComponent<SlicerObject>();
			result.InteractionCondition = source.InteractionCondition;
			result.InteractionEnable = source.InteractionEnable;

			return result;
		}

		public bool IsPointsVisible(Vector3[] points)
		{
			for (int i = 0; i < points.Length; i++)
			{
				if (!IsPointVisible(points[i]))
				{
					return false;
				}
			}

			return true;
		}

		public bool IsPointVisible(Vector3 point)
		{
			Vector3 viewportPoint = Camera.main.WorldToViewportPoint(point);

			return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;
		}

		private Mesh CreateMeshFromProjections(Vector3[] near, Vector3[] far)
		{
			int pointCount = near.Length;
			Vector3[] vertices = new Vector3[pointCount * 2];
			int[] triangles = new int[pointCount * 6 * 2];

			for (int i = 0; i < pointCount; i++)
			{
				vertices[i] = near[i];
				vertices[i + pointCount] = far[i];
			}

			int index = 0;

			for (int i = 0; i < pointCount; i++)
			{
				int near1 = i;
				int near2 = (i + 1) % pointCount;
				int far1 = i + pointCount;
				int far2 = ((i + 1) % pointCount) + pointCount;

				triangles[index++] = near1;
				triangles[index++] = far1;
				triangles[index++] = near2;

				triangles[index++] = near2;
				triangles[index++] = far1;
				triangles[index++] = far2;
			}

			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			return mesh;
		}

		private bool IsClockwise(Vector3[] points)
		{
			Vector3 cameraPosition = Camera.main.transform.position;
			Vector3 normal = Vector3.zero;

			for (int i = 0; i < points.Length; i++)
			{
				Vector3 current = points[i];
				Vector3 next = points[(i + 1) % points.Length];
				normal += Vector3.Cross(next - current, cameraPosition - current);
			}
			
			return Vector3.Dot(normal, Camera.main.transform.forward) < 0.0f;
		}
	}
}