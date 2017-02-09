using UnityEngine;

public class Planet : MonoBehaviour {
	public Surface surface;
	public Water water;
	public Atmosphere atmosphere;

	[Header("Generation")]
	public bool generate;


	private void Start() {
		// height shader
		// slope shader
		// atmosphere + atmospheric haze
		// adapt water level to average surface height
		// exaggeration from average surface height (minimim, maximum)
		// make water level available in surface shader through updateMaterial() + water level property
		// script-based scale (minimum, maximum) + hard scale water & atmosphere vertices
		// script parameterization of materials (minimum, maximum, etc.)
		// color selection from gradient + luminance texture (RGB to HSL to RGB? or something simpler like tinting?) + water depth (bluer/darker when deeper)
		// random texture selection
		// take generated values and send them to shaders in updateMaterial() + generated values as properties
		// make script and shaders origin (or world space?) independent
		// moon and asteroid prefabs

		surface.initialize();
		water.initialize();
		atmosphere.initialize();

		if(generate) {
			surface.generate();
			water.generate();
			atmosphere.generate();

			surface.updateMaterial();
			water.updateMaterial();
			atmosphere.updateMaterial();
		}
	}


	private void Update() {
		if(Input.GetKeyDown(KeyCode.H)) {
			surface.generate();
		}

		if(Input.GetKeyDown(KeyCode.J)) {
			water.generate();
		}

		if(Input.GetKeyDown(KeyCode.K)) {
			atmosphere.generate();
		}

		if(Input.GetKeyDown(KeyCode.B)) {
			surface.updateMaterial();
		}

		if(Input.GetKeyDown(KeyCode.N)) {
			water.updateMaterial();
		}

		if(Input.GetKeyDown(KeyCode.M)) {
			atmosphere.updateMaterial();
		}

		if(Input.GetKeyDown(KeyCode.KeypadMinus)) {
			surface.crater();
		}

		if(Input.GetKeyDown(KeyCode.KeypadPlus)) {
			surface.fault();
		}

		if(Input.GetKeyDown(KeyCode.KeypadEnter)) {
			surface.smooth();
		}

		if(Input.GetKeyDown(KeyCode.KeypadPeriod)) {
			surface.calculateNormals();
		}

		if(Input.GetKeyDown(KeyCode.Keypad0)) {
			surface.calculateAverage();
		}

		if(Input.GetKeyDown(KeyCode.Equals)) {
			water.increaseLevel();
		}

		if(Input.GetKeyDown(KeyCode.Minus)) {
			water.decreaseLevel();
		}
	}
}
