using UnityEngine;

public class Surface : MonoBehaviour {
	public float minimumScale = 0.8f;
	public float maximumScale = 1.25f;

	public float minimumDistance = 0.0f;
	public float maximumDistance = 10.0f;

	public int perturbRepetitions = 10;
	public int smoothRepetitions = 2;

	
	private int[][] neighbourTriangles;
	private int[][] neighbourVertices;


	public void Start() {
		// -fault plane
		// -normals
		// continents
		// "3D" texture
		// height shader
		// slope shader

		findNeighbours();

		for(int perturbCount = 0; perturbCount < perturbRepetitions; perturbCount++) {
			perturb();
		}

		for(int smoothCount = 0; smoothCount < smoothRepetitions; smoothCount++) {
			smooth();
		}

		calculateNormals();
	}


	public void Update() {
		if(Input.GetKeyDown(KeyCode.KeypadPlus)) {
			perturb();
		}

		if(Input.GetKeyDown(KeyCode.KeypadEnter)) {
			smooth();
		}

		if(Input.GetKeyDown(KeyCode.KeypadMinus)) {
			calculateNormals();
		}
	}


	private void findNeighbours() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;
		
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;

		// First pass: count the neighbours.
		int[] neighbourCounts = new int[vertices.Length];

		for(int trianglesIndex = 0; trianglesIndex < triangles.Length; trianglesIndex += 3) {
			int vertexIndex1 = triangles[trianglesIndex];
			int vertexIndex2 = triangles[trianglesIndex + 1];
			int vertexIndex3 = triangles[trianglesIndex + 2];

			neighbourCounts[vertexIndex1] += 1;
			neighbourCounts[vertexIndex2] += 1;
			neighbourCounts[vertexIndex3] += 1;
		}

		// Second pass: collect the neighbour triangles.
		int[] neighbourTriangleCounts = new int[vertices.Length];
		neighbourTriangles = new int[vertices.Length][];

		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			int neighbourCount = neighbourCounts[vertexIndex];

			neighbourTriangles[vertexIndex] = new int[neighbourCount];
		}

		for(int trianglesIndex = 0; trianglesIndex < triangles.Length; trianglesIndex += 3) {
			int vertexIndex1 = triangles[trianglesIndex];
			int vertexIndex2 = triangles[trianglesIndex + 1];
			int vertexIndex3 = triangles[trianglesIndex + 2];
			
			int neighbourTriangleCount1 = neighbourTriangleCounts[vertexIndex1];
			int neighbourTriangleCount2 = neighbourTriangleCounts[vertexIndex2];
			int neighbourTriangleCount3 = neighbourTriangleCounts[vertexIndex3];

			neighbourTriangleCounts[vertexIndex1] = neighbourTriangleCount1 + 1;
			neighbourTriangles[vertexIndex1][neighbourTriangleCount1] = trianglesIndex;

			neighbourTriangleCounts[vertexIndex2] = neighbourTriangleCount2 + 1;
			neighbourTriangles[vertexIndex2][neighbourTriangleCount2] = trianglesIndex;

			neighbourTriangleCounts[vertexIndex3] = neighbourTriangleCount3 + 1;
			neighbourTriangles[vertexIndex3][neighbourTriangleCount3] = trianglesIndex;
		}

		// Third pass: collect the neighbour vertices.
		int[] neighbourVertexCounts = new int[vertices.Length];
		neighbourVertices = new int[vertices.Length][];

		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			int neighbourCount = neighbourCounts[vertexIndex];

			neighbourVertices[vertexIndex] = new int[neighbourCount];
		}

		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			for(int neighbourTrianglesIndex = 0; neighbourTrianglesIndex < neighbourCounts[vertexIndex]; neighbourTrianglesIndex++) {
				int neighbourTriangleIndex = neighbourTriangles[vertexIndex][neighbourTrianglesIndex];

				int vertexIndex1 = triangles[neighbourTriangleIndex];
				int vertexIndex2 = triangles[neighbourTriangleIndex + 1];
				int vertexIndex3 = triangles[neighbourTriangleIndex + 2];
				
				int neighbourVertexCount = neighbourVertexCounts[vertexIndex];

				bool vertexIndex1Found = false;
				bool vertexIndex2Found = false;
				bool vertexIndex3Found = false;

				if(vertexIndex1 == vertexIndex) {
					vertexIndex1Found = true;
				}
				else if(vertexIndex2 == vertexIndex) {
					vertexIndex2Found = true;
				}
				else if(vertexIndex3 == vertexIndex) {
					vertexIndex3Found = true;
				}

				for(int neighbourVerticesIndex = 0; neighbourVerticesIndex < neighbourVertexCount; neighbourVerticesIndex++) {
					int neighbourVertexIndex = neighbourVertices[vertexIndex][neighbourVerticesIndex];

					if(neighbourVertexIndex == vertexIndex1) {
						vertexIndex1Found = true;
					}
					else if(neighbourVertexIndex == vertexIndex2) {
						vertexIndex2Found = true;
					}
					else if(neighbourVertexIndex == vertexIndex3) {
						vertexIndex3Found = true;
					}
				}

				if(!vertexIndex1Found) {
					neighbourVertices[vertexIndex][neighbourVertexCount] = vertexIndex1;
					neighbourVertexCount += 1;
				}

				if(!vertexIndex2Found) {
					neighbourVertices[vertexIndex][neighbourVertexCount] = vertexIndex2;
					neighbourVertexCount += 1;
				}

				if(!vertexIndex3Found) {
					neighbourVertices[vertexIndex][neighbourVertexCount] = vertexIndex3;
					neighbourVertexCount += 1;
				}

				neighbourVertexCounts[vertexIndex] = neighbourVertexCount;
			}
		}
	}


	private void perturb() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;

		Vector3[] vertices = mesh.vertices;

		Vector3 faultPlaneNormal = Random.onUnitSphere;
		Plane faultPlane = new Plane(faultPlaneNormal, 0.0f);

		float scale = Random.Range(minimumScale, maximumScale);

		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			Vector3 vertex = vertices[vertexIndex];

			float sign = faultPlane.GetSide(vertex) ? 1 : -1;
			float signedScale = Mathf.Pow(scale, sign);

			float distance = Mathf.Abs(faultPlane.GetDistanceToPoint(vertex));
			float clampedDistance = Mathf.Clamp(distance, minimumDistance, maximumDistance);

			float interpolation = (clampedDistance - minimumDistance) / (maximumDistance - minimumDistance);
			float interpolatedScale = signedScale + (1.0f - interpolation) * (1.0f - signedScale);

			vertex *= interpolatedScale;

			vertices[vertexIndex] = vertex;
		}

		mesh.vertices = vertices;
	}

	private void smooth() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;

		Vector3[] vertices = mesh.vertices;

		Vector3[] smoothedVertices = new Vector3[mesh.vertexCount];
		
		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			float total = vertices[vertexIndex].magnitude;

			int neighbourVertexCount = neighbourVertices[vertexIndex].Length;

			for(int neighbourVerticesIndex = 0; neighbourVerticesIndex < neighbourVertexCount; neighbourVerticesIndex++) {
				int neighbourVertexIndex = neighbourVertices[vertexIndex][neighbourVerticesIndex];

				total += vertices[neighbourVertexIndex].magnitude;
			}
			
			float radius = total / (neighbourVertexCount + 1);

			smoothedVertices[vertexIndex] = vertices[vertexIndex].normalized * radius;
		}

		mesh.vertices = smoothedVertices;
	}


	private void calculateNormals() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;
		
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;

		Vector3[] normals = new Vector3[mesh.vertexCount];
		
		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			Vector3 total = Vector3.zero;

			int neighbourTriangleCount = neighbourTriangles[vertexIndex].Length;

			for(int neighbourTrianglesIndex = 0; neighbourTrianglesIndex < neighbourTriangleCount; neighbourTrianglesIndex++) {
				int neighbourTriangleIndex = neighbourTriangles[vertexIndex][neighbourTrianglesIndex];

				int vertexIndex1 = triangles[neighbourTriangleIndex];
				int vertexIndex2 = triangles[neighbourTriangleIndex + 1];
				int vertexIndex3 = triangles[neighbourTriangleIndex + 2];

				Vector3 vertex1 = vertices[vertexIndex1];
				Vector3 vertex2 = vertices[vertexIndex2];
				Vector3 vertex3 = vertices[vertexIndex3];

				Vector3 right = vertex2 - vertex1;
				Vector3 left = vertex3 - vertex1;

				total += Vector3.Cross(right, left).normalized;
			}

			normals[vertexIndex] = Vector3.Normalize(total / neighbourTriangleCount);
		}

		mesh.normals = normals;
	}
}
