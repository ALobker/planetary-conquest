﻿using UnityEngine;

public class Atmosphere : MonoBehaviour {
	public Planet planet;

	[Header("Atmosphere")]
	public float minimumHeight = 1.08f;
	public float maximumHeight = 1.125f;

	public float step = 0.01f;


	private float height = 1.0f;


	public float Height {
		get {
			return height;
		}
	}


	public void initialize() {
		// Enable the depth texture so we can do atmospheric haze.
		Camera.main.depthTextureMode = DepthTextureMode.Depth;

		updateSize();
	}

	public void generate() {
		height = Random.Range(minimumHeight, maximumHeight);

		updateSize();
	}

	public void updateMaterial() {
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		Material material = meshRenderer.material;

		material.SetFloat("SurfaceHeight", planet.surface.Average);
	}


	public void increaseHeight() {
		height += step;

		updateSize();
	}

	public void decreaseHeight() {
		height -= step;

		updateSize();
	}


	private void updateSize() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;
		
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;

		// Surface is always handled before we are handled.
		float radius = planet.surface.Average * height;

		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			Vector3 vertex = vertices[vertexIndex];
			
			vertex = radius * vertex.normalized;

			vertices[vertexIndex] = vertex;
		}

		mesh.vertices = vertices;
	}
}
