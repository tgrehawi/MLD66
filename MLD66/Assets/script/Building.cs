using UnityEngine;

public class Building : MonoBehaviour {

	State _state;
	public State state {
		get {
			return _state;
		}
		set {
			_state = value;
		}
	}

	Material material;
	new Renderer renderer;
	new Collider collider;

	void Awake() {
		renderer = GetComponent<Renderer>();
		if (renderer != null) {
			material = renderer.material;
		}
		collider = GetComponent<Collider>();
	}

	public enum State { Placing, Building, Active }

	void Update() {
		Builder builder = Builder.main;
		switch (state) {
			//building is being placed by player
			case State.Placing:
				builder.busy = true;
				collider.enabled = false;
				RaycastHit hit;
				Ray ray = CameraRig.main.camera.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, builder.validLayers)) {
					renderer.enabled = true;
					transform.position = hit.point;
					transform.forward = hit.normal;
				}
				else {
					renderer.enabled = false;
				}
				bool valid = PositionValid(transform.position);
				renderer.material = valid ? builder.validPlacingMaterial : builder.invalidPlacingMaterial;
				if (Input.GetMouseButtonDown((int) MouseButton.Left) && valid && renderer.enabled) {
					state = State.Building;
					builder.busy = false;
				}
				if (Input.GetMouseButtonDown((int) MouseButton.Right)) {
					Destroy(gameObject);
					builder.busy = false;
				}
				break;
			//building is constructing or deconstructing
			case State.Building:
				collider.enabled = true;
				renderer.enabled = true;
				renderer.material = material;
				break;
			//building is finished
			case State.Active:
				collider.enabled = true;
				renderer.enabled = true;
				renderer.material = material;
				break;
		}
	}

	public virtual bool PositionValid(Vector3 position) {
		return Builder.main.PositionValid(position);
	}

}
