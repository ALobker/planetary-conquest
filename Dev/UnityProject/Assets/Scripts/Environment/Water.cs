using UnityEngine;

public class Water : MonoBehaviour {
	private float level = 1.0f;

	public float minimumLevel = 0.99f;
	public float maximumLevel = 1.01f;

	public float step = 0.001f;

	
	public void Start() {
		// Enable the depth texture so we can do depth shading.
		Camera.main.depthTextureMode = DepthTextureMode.Depth;

		level = Random.Range(minimumLevel, maximumLevel);

		updateLevel();
	}


	public void Update() {
		if(Input.GetKeyDown(KeyCode.Equals)) {
			increaseLevel();
		}

		if(Input.GetKeyDown(KeyCode.Minus)) {
			decreaseLevel();
		}
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
