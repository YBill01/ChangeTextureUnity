using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Parabox.CSG;

namespace YBSlice
{
	public class SlicerTest : MonoBehaviour
	{
		[SerializeField]
		private MeshFilter m_object;
		[SerializeField]
		private GameObject m_knifeObject;

		[SerializeField]
		private Transform[] m_slicePoints;

		[SerializeField]
		private float m_depth = 1.0f;


		[SerializeField]
		private Transform m_resultContainer;


		private void Awake()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			stopwatch.Restart();

			var knife = CreateMeshAtPoints();
			knife.GetComponent<MeshRenderer>().sharedMaterials = m_object.GetComponent<MeshRenderer>().materials;
			//knife.transform.SetParent(m_resultContainer);

			stopwatch.Stop();
			UnityEngine.Debug.Log("CreateMeshAtPoints: " + stopwatch.ElapsedMilliseconds + " ms");

			stopwatch.Restart();

			Model result1 = CSG.Intersect(m_object.gameObject, knife);
			Model result2 = CSG.Subtract(m_object.gameObject, knife);


			//Model result = CSG.Subtract(m_object.gameObject, m_knifeObject);

			var composite1 = new GameObject();
			composite1.AddComponent<MeshFilter>().sharedMesh = result1.mesh;
			composite1.AddComponent<MeshRenderer>().sharedMaterials = m_object.GetComponent<MeshRenderer>().materials;
			composite1.AddComponent<MeshCollider>();//.convex = true;

			var composite2 = new GameObject();
			composite2.AddComponent<MeshFilter>().sharedMesh = result2.mesh;
			composite2.AddComponent<MeshRenderer>().sharedMaterials = m_object.GetComponent<MeshRenderer>().materials;
			composite2.AddComponent<MeshCollider>();//.convex = true;

			//MeshCutter.CutCurved(m_object.gameObject, knife);
			//MeshCutter.CutCurved(m_object.gameObject, m_knifeObject);

			//Destroy(knife);
			Destroy(m_object.gameObject);

			stopwatch.Stop();

			UnityEngine.Debug.Log("MeshCutter.CutCurved: " + stopwatch.ElapsedMilliseconds + " ms");
		}


		private GameObject CreateMeshAtPoints()
		{
			if (!IsClockwise(m_slicePoints))
			{
				UnityEngine.Debug.Log("Точки идут против часовой стрелки, разворачиваем порядок...");
				
				System.Array.Reverse(m_slicePoints);
			}

			Vector3[] vertices = new Vector3[m_slicePoints.Length * 2];
			for (int i = 0; i < m_slicePoints.Length; i++)
			{
				// Передняя поверхность
				vertices[i] = m_slicePoints[i].position;
				// Задняя поверхность (смещенная вдоль направления камеры)
				Vector3 direction = (m_slicePoints[i].position - Camera.main.transform.position).normalized;
				vertices[i + m_slicePoints.Length] = m_slicePoints[i].position + direction * m_depth;
			}

			// Генерируем индексы для треугольников
			int[] triangles = new int[(m_slicePoints.Length /*- 1*/) * 6];
			for (int i = 0; i < m_slicePoints.Length /*- 1*/; i++)
			{
				int next = (i + 1) % m_slicePoints.Length;

				// Передняя поверхность
				triangles[i * 6] = i;
				triangles[i * 6 + 1] = next;
				triangles[i * 6 + 2] = i + m_slicePoints.Length;

				// Задняя поверхность
				triangles[i * 6 + 3] = next;
				triangles[i * 6 + 4] = next + m_slicePoints.Length;
				triangles[i * 6 + 5] = i + m_slicePoints.Length;
			}

			// Создаем меш
			Mesh mesh = new Mesh
			{
				vertices = vertices,
				triangles = triangles
			};
			mesh.RecalculateNormals(); // Пересчитываем нормали

			// Переворачиваем нормали, чтобы они смотрели внутрь
			//ReverseNormals(mesh);

			GameObject myComplexObject = new GameObject("KnifeObject", typeof(MeshRenderer));

			// Добавляем MeshFilter и MeshRenderer к объекту
			MeshFilter meshFilter = myComplexObject.AddComponent<MeshFilter>();
			//MeshRenderer meshRenderer = myComplexObject.AddComponent<MeshRenderer>();

			meshFilter.mesh = mesh;


			return myComplexObject;
			// Устанавливаем материал (можно заменить на свой)
			//meshRenderer.material = new Material(Shader.Find("Standard"));
		}

		/*private void ReverseNormals(Mesh mesh)
		{
			Vector3[] normals = mesh.normals;
			for (int i = 0; i < normals.Length; i++)
			{
				normals[i] = -normals[i];
			}
			mesh.normals = normals;

			int[] triangles = mesh.triangles;
			for (int i = 0; i < triangles.Length; i += 3)
			{
				// Меняем местами вершины треугольника
				int temp = triangles[i];
				triangles[i] = triangles[i + 1];
				triangles[i + 1] = temp;
			}
			mesh.triangles = triangles;
		}*/

		private bool IsClockwise(Transform[] points)
		{
			Vector3 cameraPosition = Camera.main.transform.position;
			Vector3 normal = Vector3.zero;

			for (int i = 0; i < points.Length; i++)
			{
				Vector3 current = points[i].position;
				Vector3 next = points[(i + 1) % points.Length].position;
				normal += Vector3.Cross(next - current, cameraPosition - current);
			}

			// Нормаль указывает "вверх", если порядок точек по часовой стрелке относительно камеры
			return Vector3.Dot(normal, Camera.main.transform.forward) < 0f;
		}
	}
}