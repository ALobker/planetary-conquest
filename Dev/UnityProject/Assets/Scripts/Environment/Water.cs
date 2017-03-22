using UnityEngine;

public class Water : MonoBehaviour {
	public Planet planet;

	[Header("Water")]
	public float minimumLevel = 0.995f;
	public float maximumLevel = 1.005f;

	public float step = 0.001f;


	private float level = 1.0f;


	public float Level {
		get {
			return level;
		}
	}


	public void initialize() {
		// Enable the depth texture so we can do depth shading.
		Camera.main.depthTextureMode = DepthTextureMode.Depth;

		updateSize();
	}

	public void generate() {
		level = Random.Range(minimumLevel, maximumLevel);

		updateSize();
	}

	public void updateMaterial() {

	}


	public void increaseLevel() {
		level += step;

		updateSize();
	}

	public void decreaseLevel() {
		level -= step;

		updateSize();
	}


	private void updateSize() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;
		
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;

		// Surface is always handled before we are handled.
		float radius = planet.surface.Average * level;

		for(int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
			Vector3 vertex = vertices[vertexIndex];
			
			vertex = radius * vertex.normalized;

			vertices[vertexIndex] = vertex;
		}

		mesh.vertices = vertices;
	}
}
