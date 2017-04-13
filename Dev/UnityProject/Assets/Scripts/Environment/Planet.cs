// Lightmap UVs for baked lighting...
// Fix tiny artifacts related to normals (floating point precision issues? renormalization of tiny values?)

// introduce noise in the height/slope shader? -> use toggle & allow no noise around water level (also a toggle) -> noise() apparently doesn't work on most graphic cards
// introduce clouds in atmosphere shader? (but no noise()... :( )

// use scaling parameters in water shader, use substances/water shading, use density integration?

// clean up water/atmosphere shader like surface shader

// script parameterization of materials (minimum, maximum, etc.) + remove all properties?
// color selection from gradient + saturation and value channels in surface texture, remove metallic channel + add extra layers + water depth (bluer/darker when deeper)
// random texture selection

// moon and asteroid prefabs

using UnityEngine;

public class Planet : MonoBehaviour {
	public Surface surface;
	public Water water;
	public Atmosphere atmosphere;

	[Header("Planet")]
	public bool generateOnStart = true;
	
	[Header("Features")]
	public bool featuresWater = false;
	public bool featuresAtmosphere = false;

	[Header("Generation")]
	public bool generateSurface = true;
	public bool generateWater = true;
	public bool generateAtmosphere = true;

	[Header("Debug")]
	public bool enableInputControls = false;


	private void Start() {
		if(generateOnStart) {
			generate();
		}
	}


	private void Update() {
		// This is a debug feature.
		if(enableInputControls) {
			// Generation controls.
			if(Input.GetKeyDown(KeyCode.H)) {
				surface.generate();
			}

			if(Input.GetKeyDown(KeyCode.J)) {
				if(featuresWater) {
					water.generate();
				}
			}

			if(Input.GetKeyDown(KeyCode.K)) {
				if(featuresAtmosphere) {
					atmosphere.generate();
				}
			}

			// Update material controls.
			if(Input.GetKeyDown(KeyCode.B)) {
				surface.updateMaterial();
			}

			if(Input.GetKeyDown(KeyCode.N)) {
				if(featuresWater) {
					water.updateMaterial();
				}
			}

			if(Input.GetKeyDown(KeyCode.M)) {
				if(featuresAtmosphere) {
					atmosphere.updateMaterial();
				}
			}

			// Individual surface generation controls.
			if(Input.GetKeyDown(KeyCode.KeypadPlus)) {
				surface.crater();
			}

			if(Input.GetKeyDown(KeyCode.KeypadMinus)) {
				surface.fault();
			}

			if(Input.GetKeyDown(KeyCode.KeypadMultiply)) {
				surface.exaggerate();
			}

			if(Input.GetKeyDown(KeyCode.KeypadEnter)) {
				surface.smooth();
			}

			if(Input.GetKeyDown(KeyCode.KeypadDivide)) {
				surface.erode();
			}

			if(Input.GetKeyDown(KeyCode.Keypad0)) {
				surface.calculateBounds();
			}

			if(Input.GetKeyDown(KeyCode.KeypadPeriod)) {
				surface.calculateNormals();
			}

			// Individual water generation controls.
			if(Input.GetKeyDown(KeyCode.Equals)) {
				if(featuresWater) {
					water.increaseLevel();
				}
			}

			if(Input.GetKeyDown(KeyCode.Minus)) {
				if(featuresWater) {
					water.decreaseLevel();
				}
			}

			// Individual atmosphere generation controls.
			if(Input.GetKeyDown(KeyCode.RightBracket)) {
				if(featuresAtmosphere) {
					atmosphere.increaseHeight();
				}
			}

			if(Input.GetKeyDown(KeyCode.LeftBracket)) {
				if(featuresAtmosphere) {
					atmosphere.decreaseHeight();
				}
			}

			// Update collision controls.
			if(Input.GetKeyDown(KeyCode.Comma)) {
				surface.updateCollision();
			}

			if(Input.GetKeyDown(KeyCode.Period)) {
				if(featuresWater) {
					water.updateCollision();
				}
			}
		}
	}


	public void generate() {
		// Perform initialization.
		surface.initialize();

		if(featuresWater) {
			water.initialize();
		}

		if(featuresAtmosphere) {
			atmosphere.initialize();
		}

		// Perform generation if requested.
		if(generateSurface) {
			surface.generate();
		}

		if(featuresWater && generateWater) {
			water.generate();
		}

		if(featuresAtmosphere && generateAtmosphere) {
			atmosphere.generate();
		}

		// Update the materials if appropriate.
		if(generateSurface) {
			surface.updateMaterial();
		}

		if(featuresWater) {
			water.updateMaterial();
		}

		if(featuresAtmosphere) {
			atmosphere.updateMaterial();
		}
	}
}
