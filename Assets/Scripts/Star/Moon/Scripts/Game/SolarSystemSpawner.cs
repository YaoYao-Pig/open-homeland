using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemSpawner : MonoBehaviour {

	public CelestialBodyGenerator.ResolutionSettings resolutionSettings;


	private static SolarSystemSpawner _instance;
	public static SolarSystemSpawner Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<SolarSystemSpawner>();
				if (_instance == null)
				{
					GameObject g = new GameObject(typeof(SolarSystemSpawner).ToString());
					g.AddComponent<SolarSystemSpawner>();
				}
			}
			return _instance;
		}
		private set { }
	}
    void Awake()
    {
        //Spawn(0);
        if (_instance == null)
        {
			_instance = this;
        }
        else
        {
			Destroy(this);
        }
	
	}




    public void Spawn (int seed) {
		//Debug.Log("SolarSpawner");
		var sw = System.Diagnostics.Stopwatch.StartNew ();

		PRNG prng = new PRNG (seed);
		CelestialBody[] bodies = FindObjectsOfType<CelestialBody> ();


		Repository repository = GameData.Instance.GetCurrentRepository();
		var developerNet=repository.developerNetwork;
		foreach (var body in bodies) {
			if (body.bodyType == CelestialBody.BodyType.Sun) {
				continue;
			}

			BodyPlaceholder placeholder = body.gameObject.GetComponentInChildren<BodyPlaceholder> ();
			var template = placeholder.bodySettings;
			if(template.shape is MoonShape)
            {
				MoonShape t = (MoonShape)template.shape;
				Debug.Log("Height" + repository.GetHeighestDeveloperOpenRank());
				t.craterSettings.numCraters = (int)(repository.GetHeighestDeveloperOpenRank()*10);
				t.seed= (int)(repository.GetHeighestDeveloperOpenRank() * 10);
			}



			//Destroy (placeholder.gameObject);

			GameObject holder = GameObject.Find("Body Generator");
			if(holder==null)
				holder=new GameObject ("Body Generator");

			CelestialBodyGenerator generator;
            if (!holder.TryGetComponent<CelestialBodyGenerator>(out generator))
            {
				generator = holder.AddComponent<CelestialBodyGenerator>();
			}
			generator.transform.parent = body.transform;
			generator.gameObject.layer = body.gameObject.layer;
			generator.transform.localRotation = Quaternion.identity;
			generator.transform.localPosition = Vector3.zero;
			generator.transform.localScale = Vector3.one * body.radius;
			generator.resolutionSettings = resolutionSettings;

			generator.body = template;
			generator.Generate();

		}

		//Debug.Log ("Generation time: " + sw.ElapsedMilliseconds + " ms.");
	}

}