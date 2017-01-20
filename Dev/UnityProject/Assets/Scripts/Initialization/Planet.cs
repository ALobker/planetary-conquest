using UnityEngine;

public class Planet : MonoBehaviour {
	public float minimumScale = 0.8f;
	public float maximumScale = 1.25f;

	public float minimumDistance = 0.0f;
	public float maximumDistance = 10.0f;


	public void Start() {
		perturb();
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

		for(int index = 0; index < vertices.Length; index++) {
			Vector3 vertex = vertices[index];

			float sign = faultPlane.GetSide(vertex) ? 1 : -1;

			float actualScale = Mathf.Pow(scale, sign);

			float distance = Mathf.Abs(faultPlane.GetDistanceToPoint(vertex));

			float clampedDistance = Mathf.Clamp(distance, minimumDistance, maximumDistance);

			float interpolation = (clampedDistance - minimumDistance) / (maximumDistance - minimumDistance);

			float actualInterpolation = actualScale + (1.0f - interpolation) * (1.0f - actualScale);

			vertex *= actualInterpolation;

			vertices[index] = vertex;
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

		for(int index = 0; index < vertices.Length; index++) {
			neighbours[index] = new int[] { 0, -1, -1, -1, -1, -1, -1 };
		}

		for(int index = 0; index < triangles.Length; index += 3) {
			int index1 = triangles[index];
			int index2 = triangles[index + 1];
			int index3 = triangles[index + 2];
			
			int neighbourCount1 = neighbours[index1][0];
			int neighbourCount2 = neighbours[index2][0];
			int neighbourCount3 = neighbours[index3][0];

			bool index1ContainsIndex2 = false;
			bool index1ContainsIndex3 = false;
			for(int indexIndex = 1; indexIndex < 7; indexIndex++) {
				if(neighbours[index1][indexIndex] == index2) {
					index1ContainsIndex2 = true;
				}
				if(neighbours[index1][indexIndex] == index3) {
					index1ContainsIndex3 = true;
				}
			}

			bool index2ContainsIndex1 = false;
			bool index2ContainsIndex3 = false;
			for(int indexIndex = 1; indexIndex < 7; indexIndex++) {
				if(neighbours[index2][indexIndex] == index1) {
					index2ContainsIndex1 = true;
				}
				if(neighbours[index2][indexIndex] == index3) {
					index2ContainsIndex3 = true;
				}
			}

			bool index3ContainsIndex1 = false;
			bool index3ContainsIndex2 = false;
			for(int indexIndex = 1; indexIndex < 7; indexIndex++) {
				if(neighbours[index3][indexIndex] == index1) {
					index3ContainsIndex1 = true;
				}
				if(neighbours[index3][indexIndex] == index2) {
					index3ContainsIndex2 = true;
				}
			}

			if(!index1ContainsIndex2) {
				neighbours[index1][neighbourCount1 + 1] = index2;
				neighbourCount1 += 1;
			}
			if(!index1ContainsIndex3) {
				neighbours[index1][neighbourCount1 + 1] = index3;
				neighbourCount1 += 1;
			}
			neighbours[index1][0] = neighbourCount1;

			if(!index2ContainsIndex1) {
				neighbours[index2][neighbourCount2 + 1] = index1;
				neighbourCount2 += 1;
			}
			if(!index2ContainsIndex3) {
				neighbours[index2][neighbourCount2 + 1] = index3;
				neighbourCount2 += 1;
			}
			neighbours[index2][0] = neighbourCount2;

			if(!index3ContainsIndex1) {
				neighbours[index3][neighbourCount3 + 1] = index1;
				neighbourCount3 += 1;
			}
			if(!index3ContainsIndex2) {
				neighbours[index3][neighbourCount3 + 1] = index2;
				neighbourCount3 += 1;
			}
			neighbours[index3][0] = neighbourCount3;
		}

		for(int index = 0; index < vertices.Length; index++) {
			Vector3 vertex = vertices[index];

			Vector3 total = vertex;

			int neighbourCount = neighbours[index][0];

			for(int neighbourIndexIndex = 1; neighbourIndexIndex < neighbourCount + 1; neighbourIndexIndex++) {
				int neighbourIndex = neighbours[index][neighbourIndexIndex];

				Vector3 neighbour = vertices[neighbourIndex];

				total += neighbour;
			}

			total /= (neighbourCount + 1);

			smoothedVertices[index] = total;
		}

		mesh.vertices = smoothedVertices;
	}
}
