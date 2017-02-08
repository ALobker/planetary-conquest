using UnityEngine;

public class Planet : MonoBehaviour {
	public Surface surface;
	public Water water;
	//public Atmosphere atmosphere;

	public bool generate;


	public void Start() {
		// height shader
		// slope shader
		// water depth
		// atmosphere + atmospheric haze
		// average surface height calculation + adapt water level + exaggeration (minimim, maximum)
		// make water level available in surface shader
		// script-based scale (minimum, maximum) + hard scale water & atmosphere vertices
		// script parameterization of materials
		// color selection from gradient + luminance texture (RGB to HSL to RGB? or something simpler like tinting?)
		// random texture selection
		// move stuff from Start() to generate() + call generate in Planet.Start() if checkbox ticked (first surface, then water, then atmosphere) + references in Planet
		// create update style method that takes generated values and send them to shaders + generated values as Properties
		// handle key presses in Planet
		// make script and shaders origin (or world space?) independent
		// moon and asteroid prefabs

		surface.initialize();
		//water.initalize();
		//atmosphere.initialize();

		if(generate) {
			surface.generate();
			//water.generate();
			//atmosphere.generate();

			surface.updateMaterial();
			//water.updateMaterial();
			//atmosphere.updateMaterial();
		}
	}


	public void Update() {
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
	}
}
