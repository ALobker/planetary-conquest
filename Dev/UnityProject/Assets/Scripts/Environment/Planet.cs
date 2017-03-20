using UnityEngine;

public class Planet : MonoBehaviour {
	public Surface surface;
	public Water water;
	public Atmosphere atmosphere;

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
		// Underflattening/overflattening? Could create smoother beaches and flatter areas, as well as more pronounced mountains. Basically it's erosion. Use S curve to adjust scale from average height (i.e. like exaggeration). Requires minimum/maximum surface height to be calculated as well?
		// introduce noise in the height/slope shader? -> use toggle & allow no noise around water level (also a toggle) -> noise() apparently doesn't work on most graphic cards
		// atmosphere + atmospheric haze + queue fiddling + clouds? (but no noise()... :( )
		// script parameterization of materials (minimum, maximum, etc.) + remove all properties?
		// color selection from gradient + luminance texture (RGB to HSL to RGB? or something simpler like tinting? => hue setting (desaturate first?) or use fancier textures (toggle)) + water depth (bluer/darker when deeper)
		// random texture selection
		// moon and asteroid prefabs
		
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

		// Update the materials if generation took place.
		if(generateSurface) {
			surface.updateMaterial();
		}

		if(featuresWater && generateWater) {
			water.updateMaterial();
		}

		if(featuresAtmosphere && generateAtmosphere) {
			atmosphere.updateMaterial();
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
			if(Input.GetKeyDown(KeyCode.KeypadMinus)) {
				surface.crater();
			}

			if(Input.GetKeyDown(KeyCode.KeypadPlus)) {
				surface.fault();
			}

			if(Input.GetKeyDown(KeyCode.KeypadMultiply)) {
				surface.exaggerate();
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
			if(Input.GetKeyDown(KeyCode.LeftBracket)) {
				if(featuresAtmosphere) {
					atmosphere.increaseHeight();
				}
			}

			if(Input.GetKeyDown(KeyCode.RightBracket)) {
				if(featuresAtmosphere) {
					atmosphere.decreaseHeight();
				}
			}
		}
	}
}
