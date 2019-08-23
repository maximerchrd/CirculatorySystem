using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
	public float snapRange = 0.3f;
	public Text descriptionText;
	public Material voidMaterial;
	public Material arteriesMaterial;
	public Material veinsMaterial;
	public Tilemap Tilemap;
	public Button arteryButton;
	public Button veinButton;
	
	public List<List<Vector3>> arteriesPointsList;
	public List<List<Vector3>> veinsPointsList;

	private Vector3Int cellPosInt;
	private List<List<Vector3Int>> arteriesPointsListInt;
	private List<List<Vector3Int>> veinsPointsListInt;
	
	private bool buttonDown = false;
	private List<GameObject> lines;
	private Vector3 pointerPos;
	private int snapped = 0;
	private bool drawArtery;
	private bool saveCurrentLine = false;
	private Vector2 touchDown = Vector2.zero;
	private float timeTouchStart = 0;
	
	// Use this for initialization
	void Start ()
	{
		WriteInstructions();
		lines = new List<GameObject>();		  
		arteriesPointsList = new List<List<Vector3>> ();
		veinsPointsList = new List<List<Vector3>> ();
		arteriesPointsListInt = new List<List<Vector3Int>>();
		veinsPointsListInt = new List<List<Vector3Int>>();
		DrawArtery();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
		//if (!buttonDown && Input.touchCount == 1)
		{
			buttonDown = true;
			touchDown = Input.mousePosition;
			//touchDown = Input.touches[0].position;
			timeTouchStart = Time.time;
			CreateLine();
		} 
		else if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
		//else if (buttonDown && Input.touchCount == 0)
		{
			Destroy(lines[lines.Count - 1]);
			lines.RemoveAt(lines.Count - 1);
			buttonDown = false;
			
			if (Time.time - timeTouchStart < 0.15f)
			{
				Vector3 worldPoint = Camera.main.ScreenToWorldPoint(touchDown);		
				worldPoint.z = 0;
				WriteInfosForElement(worldPoint);
			}
		}

		if (buttonDown)
		{
			pointerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			//pointerPos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
			pointerPos.z = 0;
			Vector3 snappedPoint = SnapToHexGrid(pointerPos, snapRange);
			LineRenderer line = lines[lines.Count - 1].GetComponent<LineRenderer>();
			float currentLineLength = (line.GetPosition(1) - line.GetPosition(0)).magnitude;
			
			if (snapped == 1 && saveCurrentLine && currentLineLength < 1f)
			{
				if (drawArtery && arteriesPointsList.Count > 0 && 
				    arteriesPointsList[arteriesPointsList.Count - 1].Count > 0 && snappedPoint !=
				    arteriesPointsList[arteriesPointsList.Count - 1][
					    arteriesPointsList[arteriesPointsList.Count - 1].Count - 1])
				{
					arteriesPointsList[arteriesPointsList.Count - 1].Add(snappedPoint);
					arteriesPointsListInt[arteriesPointsListInt.Count - 1].Add(cellPosInt);
					line.SetPosition(line.positionCount - 1, snappedPoint);
					CreateLine();
				} else if (!drawArtery && veinsPointsList.Count > 0 && 
				           veinsPointsList[veinsPointsList.Count - 1].Count > 0 && snappedPoint !=
				           veinsPointsList[veinsPointsList.Count - 1][
					           veinsPointsList[veinsPointsList.Count - 1].Count - 1])
				{
					veinsPointsList[veinsPointsList.Count - 1].Add(snappedPoint);
					veinsPointsListInt[veinsPointsListInt.Count - 1].Add(cellPosInt);
					line.SetPosition(line.positionCount - 1, snappedPoint);
					CreateLine();
				}
			}
			line.SetPosition (line.positionCount - 1, snappedPoint);
		}
		
	}

	private void CreateLine()
	{
		lines.Add(new GameObject());
		LineRenderer line = lines[lines.Count - 1].AddComponent<LineRenderer>();
		line.material = arteriesMaterial;
		if (drawArtery)
		{
			line.startWidth = 0.16f;
			line.endWidth = 0.16f;
		}
		else
		{
			line.startWidth = 0.09f;
			line.endWidth = 0.09f;
		}
		line.useWorldSpace = true;  
		line.positionCount = 2;
		
		pointerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//pointerPos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
		
		pointerPos.z = 0;
		pointerPos = SnapToHexGrid(pointerPos, snapRange);
		if (snapped < 1)
		{
			line.material = voidMaterial;
			saveCurrentLine = false;
		}
		else
		{
			line.material = arteriesMaterial;
			saveCurrentLine = true;
			if (drawArtery)
			{
				arteriesPointsList.Add(new List<Vector3>());
				arteriesPointsList[arteriesPointsList.Count - 1].Add(pointerPos);
					
				arteriesPointsListInt.Add(new List<Vector3Int>());
				arteriesPointsListInt[arteriesPointsListInt.Count - 1].Add(cellPosInt);
			}
			else
			{
				veinsPointsList.Add(new List<Vector3>());
				veinsPointsList[veinsPointsList.Count - 1].Add(pointerPos);
					
				veinsPointsListInt.Add(new List<Vector3Int>());
				veinsPointsListInt[veinsPointsListInt.Count - 1].Add(cellPosInt);
			}
		}
		line.SetPosition (0, pointerPos);
		line.SetPosition (1, pointerPos);
	}

	private Vector3 SnapToHexGrid(Vector3 pointerPosition, float tolerance)
	{
		cellPosInt = Tilemap.WorldToCell(pointerPosition);
		Vector3 cellPosition = cellPosInt;
		if (cellPosition.y % 2 == 1 || cellPosition.y % 2 == -1)
		{
			cellPosition = new Vector3(cellPosition.x + 0.5f, cellPosition.y, cellPosition.z);
		}
		cellPosition = new Vector3(cellPosition.x, cellPosition.y * 0.75f, cellPosition.z);

		Vector3 difference = pointerPosition - cellPosition;
		if (difference.magnitude < tolerance && Tilemap.GetTile(cellPosInt) != null)
		{
			snapped = 1;
			return cellPosition;
		}

		snapped = -1;
		return pointerPosition;
	}

	public void DrawArtery()
	{
		drawArtery = true;
		if (arteryButton != null)
		{
			arteryButton.GetComponent<Image>().color = new Color(0.9f, 0.369f, 0.369f);
			veinButton.GetComponent<Image>().color = Color.Lerp(Color.gray, Color.white, 0.8f);
		}
	}

	public void DrawVein()
	{
		drawArtery = false;
		veinButton.GetComponent<Image>().color = new Color(0.9f, 0.369f, 0.369f);
		arteryButton.GetComponent<Image>().color = Color.Lerp(Color.gray, Color.white, 0.8f);;
	}

	public void ClearLines()
	{
		arteriesPointsList.Clear();
		arteriesPointsListInt.Clear();
		veinsPointsList.Clear();
		veinsPointsListInt.Clear();
		foreach (var line in lines)
		{
			Destroy(line);
		}
		//lines.Clear();
	}

	public void CorrectLevel()
	{
		bool correct = false;

		if (SceneManager.GetActiveScene().name == "Level1")
		{
			string[] organs = { "lungs" };
			correct = Correction.CorrectionLevelA(Tilemap, arteriesPointsListInt, organs);
		} else if (SceneManager.GetActiveScene().name == "Level2")
		{
			string[] organs = { "lungs" };
			correct = Correction.CorrectionLevelB(Tilemap, arteriesPointsListInt, veinsPointsListInt, organs);
		} else if (SceneManager.GetActiveScene().name == "Level3")
		{
			string[] organs = { "lungs" };
			correct = Correction.CorrectionLevelC(Tilemap, arteriesPointsListInt, veinsPointsListInt, organs);
		} else if (SceneManager.GetActiveScene().name == "Level4")
		{
			string[] organs = { "lungs", "muscle" };
			correct = Correction.CorrectionLevelC(Tilemap, arteriesPointsListInt, veinsPointsListInt, organs);
		}
		
		if (correct)
		{
			gameObject.GetComponent<GameManager>().LevelPassed();
		}
		else
		{
			gameObject.GetComponent<GameManager>().LevelIncorrect();
		}
	}

	private void WriteInstructions()
	{
		if (SceneManager.GetActiveScene().name == "Level1")
		{
			descriptionText.text = "Each organ must be connected to the heart. Tap any element to get its description.";
		} else if (SceneManager.GetActiveScene().name == "Level2")
		{
			descriptionText.text = "Each organ must be connected to the heart by an artery AND a vein.";
		} else if (SceneManager.GetActiveScene().name == "Level3")
		{
			descriptionText.text = "Arteries carry blood OUT of the heart. Veins carry blood INTO the heart.";
		} else if (SceneManager.GetActiveScene().name == "Level4")
		{
			descriptionText.text = "Remember that each organ must be connected to the heart.";
		}
		Invoke("EraseInstructions", 7f);
	}

	private void EraseInstructions()
	{
		descriptionText.text = "";
	}

	private void WriteInfosForElement(Vector3 touchUp)
	{
		if (Tilemap.GetTile(Tilemap.WorldToCell(touchUp)) != null)
		{
			string tileName = Tilemap.GetTile(Tilemap.WorldToCell(touchUp)).name;
			if (tileName == "heart")
			{
				descriptionText.text = "Heart";
			} else if (tileName == "lungs")
			{
				descriptionText.text = "Lungs: connect them to the heart";
			} else if (tileName.Contains("flow"))
			{
				descriptionText.text = "Blood vessel";
			} else if (tileName.Contains("muscle"))
			{
				descriptionText.text = "Muscle: connect it to the heart";
			}
		}
		Invoke("EraseInstructions", 4f);
	}
}
