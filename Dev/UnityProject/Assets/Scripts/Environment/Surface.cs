﻿using UnityEngine;

public class Surface : MonoBehaviour {
	public Planet planet;

	[Header("Size")]
	public float minimumRadius = 1000.0f;
	public float maximumRadius = 1000.0f;

	[Header("Crater")]
	public int minimumNumberOfCraters = 100;
	public int maximumNumberOfCraters = 100;

	public float minimumCraterRadius = 0.06f;
	public float maximumCraterRadius = 0.6f;

	public float minimumCraterHeight = 0.995f;
	public float maximumCraterHeight = 1.005f;

	public float minimumCraterDistance = 0.0f;
	public float maximumCraterDistance = 0.02f;

	[Header("Fault")]
	public int minimumNumberOfFaults = 0;
	public int maximumNumberOfFaults = 0;

	public float minimumFaultHeight = 0.996f;
	public float maximumFaultHeight = 1.004f;

	public float minimumFaultDistance = 0.0f;
	public float maximumFaultDistance = 0.01f;

	[Header("Smooth")]
	public int minimumNumberOfSmooths = 2;
	public int maximumNumberOfSmooths = 2;

	[Header("Exaggeration")]
	public float minimumUnderExaggeration = 2.0f;
	public float maximumUnderExaggeration = 8.0f;

	public float minimumOverExaggeration = 2.0f;
	public float maximumOverExaggeration = 4.0f;

	[Header("Erosion")]
	public float minimumUnderErosion = 0.0f;
	public float maximumUnderErosion = 0.0f;

	public float minimumOverErosion = 2.0f;
	public float maximumOverErosion = 2.0f;

	
	private int[][] neighbourTriangles;
	private int[][] neighbourVertices;

	private float radius;

	private float minimum;
	private float maximum;

	private float average;


	public float Radius {
		get {
			return radius;
		}
	}

	public float Minimum {
		get {
			return minimum;
		}
	}

	public float Maximum {
		get {
			return maximum;
		}
	}

	public float Average {
		get {
			return average;
		}
	}


	public void initialize() {
		findNeighbours();
		initializeSize();

		calculateBounds();
	}

	public void generate() {
		int numberOfCraters = Random.Range(minimumNumberOfCraters, maximumNumberOfCraters);
		for(int craterNumber = 0; craterNumber < numberOfCraters; craterNumber++) {
			crater();
		}

		int numberOfFaults = Random.Range(minimumNumberOfFaults, maximumNumberOfFaults);
		for(int faultNumber = 0; faultNumber < numberOfFaults; faultNumber++) {
			fault();
		}
		
		calculateBounds();
		exaggerate();

		int numberOfSmooths = Random.Range(minimumNumberOfSmooths, maximumNumberOfSmooths);
		for(int smoothNumber = 0; smoothNumber < numberOfSmooths; smoothNumber++) {
			smooth();
		}

		calculateBounds();
		erode();

		calculateBounds();
		calculateNormals();
	}

	public void updateMaterial() {
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		Material material = meshRenderer.material;

		material.SetFloat("SurfaceRadius", radius);
		material.SetFloat("SurfaceHeight", average);

		if(planet.featuresWater) {
			material.SetFloat("WaterHeight", average * planet.water.Level);
		}
		else {
			material.SetFloat("WaterHeight", average);
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

	private void initializeSize() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;
		
		Vector3[] vertices = mesh.vertices;

		radius = Random.Range(minimumRadius, maximumRadius);

		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			Vector3 vertex = vertices[vertexIndex];
			
			vertex = radius * vertex.normalized;

			vertices[vertexIndex] = vertex;
		}

		mesh.vertices = vertices;
	}


	public void crater() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;

		Vector3[] vertices = mesh.vertices;
		
		Vector3 craterNormal = Random.onUnitSphere;

		float craterRadius = Random.Range(minimumCraterRadius, maximumCraterRadius) * radius;
		float craterHeight = Random.Range(minimumCraterHeight, maximumCraterHeight);
		
		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			Vector3 vertex = vertices[vertexIndex];

			// Calculate the distance from the vertex to the cylinder intersecting the crater.
			float dot = Vector3.Dot(craterNormal, vertex);
			Vector3 projection = dot * craterNormal;
			Vector3 radial = vertex - projection;

			float length = radial.magnitude;
			bool ahead = dot > 0.0f;
			bool inside = length < craterRadius;
			
			float sign = ahead && inside ? 1 : -1;
			float signedHeight = Mathf.Pow(craterHeight, sign);

			float minimumDistance = minimumCraterDistance * radius;
			float maximumDistance = maximumCraterDistance * radius;

			float distance = ahead ? Mathf.Abs(craterRadius - length) : maximumDistance;
			float clampedDistance = Mathf.Clamp(distance, minimumDistance, maximumDistance);

			float interpolation = (clampedDistance - minimumDistance) / (maximumDistance - minimumDistance);
			float interpolatedHeight = signedHeight + (1.0f - interpolation) * (1.0f - signedHeight);

			vertex *= interpolatedHeight;
			
			vertices[vertexIndex] = vertex;
		}

		mesh.vertices = vertices;
	}

	public void fault() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;

		Vector3[] vertices = mesh.vertices;

		Vector3 faultPlaneNormal = Random.onUnitSphere;
		Plane faultPlane = new Plane(faultPlaneNormal, 0.0f);

		float faultHeight = Random.Range(minimumFaultHeight, maximumFaultHeight) * radius;

		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			Vector3 vertex = vertices[vertexIndex];

			float sign = faultPlane.GetSide(vertex) ? 1 : -1;
			float signedHeight = Mathf.Pow(faultHeight, sign);

			float minimumDistance = minimumFaultDistance * radius;
			float maximumDistance = maximumFaultDistance * radius;

			float distance = Mathf.Abs(faultPlane.GetDistanceToPoint(vertex));
			float clampedDistance = Mathf.Clamp(distance, minimumDistance, maximumDistance);

			float interpolation = (clampedDistance - minimumDistance) / (maximumDistance - minimumDistance);
			float interpolatedHeight = signedHeight + (1.0f - interpolation) * (1.0f - signedHeight);

			vertex *= interpolatedHeight;

			vertices[vertexIndex] = vertex;
		}

		mesh.vertices = vertices;
	}

	public void smooth() {
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

	public void exaggerate() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;
		
		Vector3[] vertices = mesh.vertices;
		
		float underExaggeration = Random.Range(minimumUnderExaggeration, maximumUnderExaggeration);
		float overExaggeration = Random.Range(minimumOverExaggeration, maximumOverExaggeration);

		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			Vector3 vertex = vertices[vertexIndex];
			
			float exaggeration = vertex.magnitude < average ? underExaggeration : overExaggeration;
			float radius = average + exaggeration * (vertex.magnitude - average);
			vertex = vertex.normalized * radius;

			vertices[vertexIndex] = vertex;
		}

		mesh.vertices = vertices;
	}

	public void erode() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;
		
		Vector3[] vertices = mesh.vertices;
		
		float underErosion = Random.Range(minimumUnderErosion, maximumUnderErosion);
		float overErosion = Random.Range(minimumOverErosion, maximumOverErosion);

		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			Vector3 vertex = vertices[vertexIndex];
			
			float erosion = vertex.magnitude < average ? underErosion : overErosion;
			float limit = vertex.magnitude < average ? minimum : maximum;

			// This is always non-negative.
			float height = (vertex.magnitude - average) / (limit - average);

			// Transform the height using a curve that is steeper near the end, depending on the
			// strength of the erosion.
			height = Mathf.Exp(-erosion) * height / Mathf.Exp(-erosion * height);

			float radius = average + height * (limit - average);

			vertex = vertex.normalized * radius;

			vertices[vertexIndex] = vertex;
		}

		mesh.vertices = vertices;
	}


	public void calculateBounds() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;
		
		Vector3[] vertices = mesh.vertices;
		
		minimum = float.MaxValue;
		maximum = float.MinValue;

		float total = 0.0f;

		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			float height = vertices[vertexIndex].magnitude;

			if(height < minimum) {
				minimum = height;
			}

			if(height > maximum) {
				maximum = height;
			}

			total += height;
		}

		average = total / vertices.Length;
	}

	public void calculateNormals() {
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
