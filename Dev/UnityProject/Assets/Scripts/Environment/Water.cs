using UnityEngine;

public class Water : MonoBehaviour {
	public Planet planet;

	[Header("Water")]
	public float minimumLevel = 0.99f;
	public float maximumLevel = 1.01f;

	public float step = 0.001f;


	private float level = 1.0f;


	public void initialize() {
		// Enable the depth texture so we can do depth shading.
		Camera.main.depthTextureMode = DepthTextureMode.Depth;
	}

	public void generate() {
		level = Random.Range(minimumLevel, maximumLevel);

		updateLevel();
	}

	public void updateMaterial() {

	}


	public void increaseLevel() {
		level += step;

		updateLevel();
	}

	public void decreaseLevel() {
		level -= step;

		updateLevel();
	}


	private void updateLevel() {
		transform.localScale = new Vector3(level, level, level);
	}
}
