using UnityEngine;

public class Planet : MonoBehaviour {
	public float minimumScale = 0.8f;
	public float maximumScale = 1.25f;

	public float minimumDistance = 0.0f;
	public float maximumDistance = 10.0f;

	public int perturbRepetitions = 10;
	public int smoothRepetitions = 2;


	public void Start() {
		for(int perturbCount = 0; perturbCount < perturbRepetitions; perturbCount++) {
			perturb();
		}

		for(int smoothCount = 0; smoothCount < smoothRepetitions; smoothCount++) {
			smooth();
		}
	}


	public void Update() {
		if(Input.GetKeyDown(KeyCode.KeypadPlus)) {
			perturb();
		}

		if(Input.GetKeyDown(KeyCode.KeypadEnter)) {
			smooth();
		}
	}


	private void perturb() {
		// fault plane
		// normals
		// "3D" texture
		// height shader
		// slope shader

		Vector3 faultPlaneNormal = Random.onUnitSphere;
		Plane faultPlane = new Plane(faultPlaneNormal, 0.0f);

		float scale = Random.Range(minimumScale, maximumScale);

		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;

		Vector3[] vertices = mesh.vertices;

		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			Vector3 vertex = vertices[vertexIndex];

			float sign = faultPlane.GetSide(vertex) ? 1 : -1;

			float actualScale = Mathf.Pow(scale, sign);

			float distance = Mathf.Abs(faultPlane.GetDistanceToPoint(vertex));

			float clampedDistance = Mathf.Clamp(distance, minimumDistance, maximumDistance);

			float interpolation = (clampedDistance - minimumDistance) / (maximumDistance - minimumDistance);

			float actualInterpolation = actualScale + (1.0f - interpolation) * (1.0f - actualScale);

			vertex *= actualInterpolation;

			vertices[vertexIndex] = vertex;
		}

		mesh.vertices = vertices;
	}

	
	private void smooth() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;

		Vector3[] vertices = mesh.vertices;
		Vector3[] smoothedVertices = mesh.vertices;

		int[] triangles = mesh.triangles;

		int[][] neighbours = new int[vertices.Length][];
		
		// Each neighbour is included twice, so include the vertex itself twice in the average as well.
		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			neighbours[vertexIndex] = new int[] { 2, vertexIndex, vertexIndex, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
		}

		// Each neighbour is included twice, but it saves on a lot of logic without affecting the average.
		for(int triangeIndex = 0; triangeIndex < triangles.Length; triangeIndex += 3) {
			int vertexIndex1 = triangles[triangeIndex];
			int vertexIndex2 = triangles[triangeIndex + 1];
			int vertexIndex3 = triangles[triangeIndex + 2];
			
			int neighbourCount1 = neighbours[vertexIndex1][0];
			int neighbourCount2 = neighbours[vertexIndex2][0];
			int neighbourCount3 = neighbours[vertexIndex3][0];
			
			neighbours[vertexIndex1][0] = neighbourCount1 + 2;
			neighbours[vertexIndex1][neighbourCount1 + 1] = vertexIndex2;
			neighbours[vertexIndex1][neighbourCount1 + 2] = vertexIndex3;
			
			neighbours[vertexIndex2][0] = neighbourCount2 + 2;
			neighbours[vertexIndex2][neighbourCount2 + 1] = vertexIndex1;
			neighbours[vertexIndex2][neighbourCount2 + 2] = vertexIndex3;
			
			neighbours[vertexIndex3][0] = neighbourCount3 + 2;
			neighbours[vertexIndex3][neighbourCount3 + 1] = vertexIndex1;
			neighbours[vertexIndex3][neighbourCount3 + 2] = vertexIndex2;
		}

		// Calculate the average.
		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			float total = 0.0f;

			int neighbourCount = neighbours[vertexIndex][0];

			for(int neighbourIndexIndex = 1; neighbourIndexIndex < neighbourCount + 1; neighbourIndexIndex++) {
				int neighbourIndex = neighbours[vertexIndex][neighbourIndexIndex];

				total += vertices[neighbourIndex].magnitude;
			}
			
			float radius = total / neighbourCount;

			smoothedVertices[vertexIndex] = vertices[vertexIndex].normalized * radius;
		}

		mesh.vertices = smoothedVertices;
	}
}
