using UnityEngine;

public class Environment : MonoBehaviour {
	public Planet planet;

	public Planet instance;


	public void Start() {
		if(instance == null) {
			createPlanet();
		}
	}

	public void Update() {
		if(Input.GetKeyDown(KeyCode.P)) {
			destroyPlanet();

			createPlanet();
		}
	}


	private void createPlanet() {
		instance = Instantiate(planet, transform);

		instance.name = "Planet";
	}

	private void destroyPlanet() {
		Destroy(instance.gameObject);
	}
}
