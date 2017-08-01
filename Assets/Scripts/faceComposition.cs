using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class faceComposition : MonoBehaviour {

	public GameObject face;

	private Sprite[] heads;
	private Sprite[] ears;
	private Sprite[] eyes;
	private Sprite[] mouths;
	private Sprite[] noses;

	void Awake()
	{
		loadAll ();
	}

	void Start()
	{

		face = createRandomFace (face);
	}

	// Update is called once per frame
	void Update () 
	{
		
	}

	/*
	 * create random face
	 */ 
	GameObject createRandomFace(GameObject someFace)
	{
		for (int i=0; i< someFace.transform.childCount;i++)
		{
			foreach (RectTransform t in someFace.transform.GetChild(i).GetComponentsInChildren<RectTransform>())
			{
				Image image = t.gameObject.GetComponent<Image> ();
				Sprite sp = LoadRandom (t.name);
				image.sprite = sp;
			}
		}
		return someFace;
	}

	/*
	 *	Hacky face loading 
	 * 
	 */
	Sprite LoadRandom(string part)
	{
		int index=0;
		switch (part)
		{

		case "Face":
			index = Random.Range (0, heads.Length);
			return heads [index];
			break;
		case "Ear":
			index = Random.Range (0, ears.Length);
			return ears [index];
			break;
		case "Nose":
			index = Random.Range (0, noses.Length);
			return noses [index];
			break;
		case "Mouth":
			index = Random.Range (0, mouths.Length);
			return mouths [index];
			break;
		case "Eye":
			index = Random.Range (0, eyes.Length);
			return eyes [index];
			break;
		default:
			return new Sprite();
			break;
		}
	}


	void loadAll()
	{
		heads = Resources.LoadAll<Sprite> ("ImprovedGraphics/Heads");
		ears = Resources.LoadAll<Sprite> ("ImprovedGraphics/Ears");
		eyes = Resources.LoadAll<Sprite> ("ImprovedGraphics/Eyes");
		noses = Resources.LoadAll<Sprite> ("ImprovedGraphics/Noses");
		mouths = Resources.LoadAll<Sprite> ("ImprovedGraphics/Mouths");

	}

}
