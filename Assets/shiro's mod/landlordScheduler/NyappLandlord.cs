using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NyappLandlord : MonoBehaviour
{
	public KMAudio KMAudio;
	public KMBombInfo BombInfo;
	public KMSelectable nyapp;
	public Material appScreen;

	public TextMesh id;

	public TextMesh message;
	public TextMesh response;
	public TextMesh reply;

	bool loaded = false;

	int runningIndex = 0;
	bool running = false;
	int activation = 100;

	public TextMesh choice1;
	public TextMesh choice2;
	public TextMesh choice3;

	public List<landlordScheduler> modules;
	bool home = true;
	float striketimer = 0;

	string correctChoice = "";
	string reply1Correct = "";
	string reply1Wrong = "";
	string reply2Correct = "";
	string reply2Wrong = "";
	string reply3Correct = "";
	string reply3Wrong = "";

	string waitingreply = "";
	bool waitingresponse = false;

	Transform phoneScreenArea;

	void onHome()
	{
		id.gameObject.SetActive(false);
		message.transform.parent.gameObject.SetActive(false);
		response.transform.parent.transform.gameObject.SetActive(false);
		reply.transform.parent.transform.gameObject.SetActive(false);

		choice1.transform.parent.gameObject.SetActive(false);
		choice2.transform.parent.gameObject.SetActive(false);
		choice3.transform.parent.gameObject.SetActive(false);

		home = true;
	}

	public void onModulePass()
	{
		runningIndex--;
	}
	public void loadApp()
	{
		if (!loaded)
		{
			phoneScreenArea= GameObject.Find("NyaPhone(Clone)/screenArea").transform;
			
			activation = UnityEngine.Random.Range(10, 30);
			Debug.Log("======activation======" + activation);
			loaded = true;
			KMSelectable home = GameObject.Find("NyaPhone(Clone)/HomeButton").GetComponent<KMSelectable>();

			home.OnInteract += delegate ()
			{
				onHome();
				disableSelectables();
				return false;
			};

			nyapp = GetComponent<KMSelectable>();
			nyapp.OnInteract += delegate () { App(); return false; };

			
			choice1.transform.parent.SetParent(phoneScreenArea, false);
			choice2.transform.parent.SetParent(phoneScreenArea, false);
			choice3.transform.parent.SetParent(phoneScreenArea, false);

			message.transform.parent.SetParent(phoneScreenArea, false);
			response.transform.parent.SetParent(phoneScreenArea, false);
			reply.transform.parent.SetParent(phoneScreenArea, false);

			id.transform.SetParent(phoneScreenArea, false);
		}
		int idx = 0;
		foreach (landlordScheduler module in modules)
		{
			landlordScheduler script = module.GetComponent<landlordScheduler>();
			if (modules.Count > 1)
			{
				script.SetId(idx);
				idx++;

			}
		}
	}
	void Start()
	{
		onHome();
	}
	void FixedUpdate()
	{
		string timerText = BombInfo.GetFormattedTime();

		if (int.Parse(timerText.Substring(3)) == activation && !running && loaded)
		{
			string idString = modules[runningIndex].idText.text;
			id.text = "" + idString;
			running = true;
			loadScreen();
			getMessage();
			activation = UnityEngine.Random.Range(30, 50);
		}
		if (int.Parse(timerText.Substring(3)) == activation && running && response.text != "")
		{
			getReply();
			running = false;
			runningIndex++;
			if (runningIndex >= modules.Count) { runningIndex = 0; }
			activation = UnityEngine.Random.Range(10, 30);

		}
		if (striketimer > 30 && waitingresponse)
		{
			modules[runningIndex].BombModule.HandleStrike();
			waitingresponse = false;
		}
		striketimer += 1f/60f;
		
	}
	void loadScreen()
	{
		message.text = "";
		response.text = "";
		reply.text = "";

		message.transform.parent.gameObject.SetActive(false);
		response.transform.parent.gameObject.SetActive(false);
		reply.transform.parent.gameObject.SetActive(false);
	}
	void getMessage()
	{
		KMAudio.PlaySoundAtTransform("shiromeow", this.transform);
		striketimer = 0;
		waitingresponse = true;
		if (!home) { message.transform.parent.Find("message").gameObject.SetActive(true); }

		int randomDate = UnityEngine.Random.Range(0, modules[runningIndex].validDates.Count - 1);
		DateTime date = modules[runningIndex].validDates[randomDate];
		DateTime badDate = date.AddDays(UnityEngine.Random.Range(7, 180));
		int randomMessage = UnityEngine.Random.Range(0, 6);
		int randomLie = UnityEngine.Random.Range(0, 6);
		List<DateTime> toRemove = new List<DateTime>();

		Debug.Log("======dateCount======" + modules[runningIndex].validDates.Count);
		Debug.Log("======date======" + date.ToString("dd/MM/yyyy"));
		switch (randomMessage)
		{
			case 0:

				message.text = "Just to let you know, i'll need the\nroom on "
				+ (randomLie < 2 ? date.ToString("dddd") : badDate.ToString("dddd"))
				+ " also don't forget\nfor the "
				+ (randomLie == 3 ? date.ToString("dd") : badDate.ToString("dd"))
				+ " of "
				+ (randomLie > 3 ? date.ToString("MMMM") : badDate.ToString("MMMM"));

				choice1.text = "See you on " + (randomLie < 2 ? date.ToString("dddd") : badDate.ToString("dddd"));
				choice2.text = "Are you sure for the " + (randomLie == 3 ? date.ToString("dd") : badDate.ToString("dd"));
				choice3.text = "Was it " + (randomLie > 3 ? date.ToString("MMMM") : badDate.ToString("MMMM")) + " ?";

				foreach (DateTime d in modules[runningIndex].validDates)
				{
					if (randomLie < 2 && date.ToString("dddd") != d.ToString("dddd")) { toRemove.Add(d); }
					else if (randomLie == 3 && date.ToString("dd") != d.ToString("dd")) { toRemove.Add(d); }
					else if (randomLie > 3 && date.ToString("MMMM") != d.ToString("MMMM")) { toRemove.Add(d); }
				}
				foreach (DateTime d2 in toRemove)
				{
					modules[runningIndex].validDates.Remove(d2);
				}

				if (randomLie < 2) { correctChoice = choice1.text; }
				else if (randomLie == 3) { correctChoice = choice2.text; }
				else if (randomLie > 3) { correctChoice = choice3.text; }

				reply1Correct = "And be awake on time!";
				reply1Wrong = "[no reply]";
				reply2Correct = "I checked it's on the " + date.ToString("dd");
				reply2Wrong = "Yea im sure";
				reply3Correct = "Can you read?\ni said what i said";
				reply3Wrong = "No probably a typo\nit's for " + date.ToString("MMMM");

				break;
			case 1:

				message.text = "Clean up "
				+ (randomLie < 2 ? date.ToString("dddd") : badDate.ToString("dddd"))
				+ " someone is\ncoming in "
				+ (randomLie == 3 ? date.ToString("MMMM yyyy") : badDate.ToString("MMMM yyyy"));

				choice1.text = "Don't we have something on " + (randomLie < 2 ? date.ToString("dddd") : badDate.ToString("dddd"));
				choice2.text = "I'll be ready";
				choice3.text = "";

				foreach (DateTime d in modules[runningIndex].validDates)
				{
					if (randomLie < 2 && date.ToString("dddd") != d.ToString("dddd")) { toRemove.Add(d); }
					else if (randomLie == 3 && date.ToString("MMMM yyyy") != d.ToString("MMMM yyyy")) { toRemove.Add(d); }
				}
				foreach (DateTime d2 in toRemove)
				{
					modules[runningIndex].validDates.Remove(d2);
				}

				if (randomLie < 2) { correctChoice = choice1.text; }
				else if (randomLie == 3) { correctChoice = choice2.text; }


				reply1Correct = "Yea i knew that just be ready";
				reply1Wrong = "I would know if we had\nprobably dreaming things again";
				reply2Correct = "And clean yourself too";
				reply2Wrong = "[no reply]";
				reply3Correct = "";
				reply3Wrong = "";

				break;
			case 2:

				message.text = "I have a date for you "
				+ (randomLie < 3 ? date.ToString("MMMM") : badDate.ToString("MMMM"))
				+ "\ncan you make it or i need to\npostponed it within "
				+ (randomLie >= 3 ? date.ToString("yyyy") : badDate.ToString("yyyy"));

				choice1.text = "Good for me";
				choice2.text = "I prefer the later";
				choice3.text = "";


				foreach (DateTime d in modules[runningIndex].validDates)
				{
					if (randomLie < 3 && date.ToString("MMMM") != d.ToString("MMMM")) { toRemove.Add(d); }
					else if (randomLie >= 3 && date.ToString("yyyy") != d.ToString("yyyy")) { toRemove.Add(d); }
				}
				foreach (DateTime d2 in toRemove)
				{
					modules[runningIndex].validDates.Remove(d2);
				}

				if (randomLie < 3) { correctChoice = choice1.text; }
				else if (randomLie >= 3) { correctChoice = choice2.text; }


				reply1Correct = "Ok.";
				reply1Wrong = "[no reply]";
				reply2Correct = "Sure, be there";
				reply2Wrong = "Well too bad\ni didn't expect you to choose it";
				reply3Correct = "";
				reply3Wrong = "";

				break;
			case 3:

				message.text = "Why didn't you answer your phone\non "
				+ (randomLie < 3 ? date.ToString("dddd") : badDate.ToString("ddddd"))
				+ "! Be sure to be there\non the "
				+ (randomLie >= 3 ? date.ToString("dd") : badDate.ToString("dd"));

				choice1.text = "Don't we have something\non " + (randomLie < 3 ? date.ToString("dddd") : badDate.ToString("ddddd"));
				choice2.text = "I am free for this date";
				choice3.text = "";

				foreach (DateTime d in modules[runningIndex].validDates)
				{
					if (randomLie < 3 && date.ToString("dddd") != d.ToString("dddd")) { toRemove.Add(d); }
					else if (randomLie >= 3 && date.ToString("dd") != d.ToString("dd")) { toRemove.Add(d); }
				}
				foreach (DateTime d2 in toRemove)
				{
					modules[runningIndex].validDates.Remove(d2);
				}


				if (randomLie < 3) { correctChoice = choice1.text; }
				else if (randomLie >= 3) { correctChoice = choice2.text; }


				reply1Correct = "Oh yea we have\nprobably had that on my mind";
				reply1Wrong = "what are you talking about";
				reply2Correct = "Ok.";
				reply2Wrong = "[no reply]";
				reply3Correct = "";
				reply3Wrong = "";

				break;
			case 4:

				message.text = "Sorry to bother you i know\nit's been hard lately but i need to\nmake a visit for\na "
				+ (randomLie == 3 ? date.ToString("dddd") + " on " + date.ToString("MMMM") : badDate.ToString("dddd") + " on " + badDate.ToString("MMMM"));

				choice1.text = "Sure don't worry there is no issue";
				choice2.text = "Can we have another date ?";
				choice3.text = "";

				foreach (DateTime d in modules[runningIndex].validDates)
				{
					if (randomLie == 3 && date.ToString("dddd MMMM") != d.ToString("dddd MMMM")) { toRemove.Add(d); }
				}
				foreach (DateTime d2 in toRemove)
				{
					modules[runningIndex].validDates.Remove(d2);
				}

				if (randomLie == 3) { correctChoice = choice1.text; }
				else { correctChoice = choice2.text; }


				reply1Correct = "We can change it anytime just tell me";
				reply1Wrong = "Perfect see you there :)";
				reply2Correct = "Oh i don't know really...";
				reply2Wrong = "Of course let's change it right now :)";
				reply3Correct = "";
				reply3Wrong = "";

				break;
			case 5:

				message.text = "Move your ass the appointment\nis now ! [sent "
				+ (randomLie == 3 ? date.ToString("dd MMMM") : badDate.ToString("dd MMMM"))
				+ "]\n*That was for someone else don't\nmind that you date is still\n"
				+ (randomLie != 3 ? date.ToString("MMMM yyyy") : badDate.ToString("MMMM yyyy"));


				choice1.text = "Im so sorry i forgot it was today";
				choice2.text = "I have it on my calendar ! no worries";
				choice3.text = "";

				foreach (DateTime d in modules[runningIndex].validDates)
				{
					if (randomLie == 3 && date.ToString("dd MMMM") != d.ToString("dd MMMM")) { toRemove.Add(d); }
					else if (randomLie != 3 && date.ToString("MMMM yyyy") != d.ToString("MMMM yyyy")) { toRemove.Add(d); }
				}
				foreach (DateTime d2 in toRemove)
				{
					modules[runningIndex].validDates.Remove(d2);
				}

				if (randomLie == 3) { correctChoice = choice1.text; }
				else if (randomLie != 3) { correctChoice = choice2.text; }


				reply1Correct = "Yea be ready now that was a test\nnice you remenbered";
				reply1Wrong = "Read the next part\ngod i swear your so ***";
				reply2Correct = "Ok.";
				reply2Wrong = "And just as i thought you forgot\nit was actually today";
				reply3Correct = "";
				reply3Wrong = "";

				break;
		}
		Debug.Log("======dateCount======" + modules[runningIndex].validDates.Count);
	}


	void getReply()
	{

		KMAudio.PlaySoundAtTransform("shiromeow", this.transform);
		if (!home) { reply.transform.parent.gameObject.SetActive(true); }
		reply.text = waitingreply;
		waitingreply = "";
	}
	void emptyChoices()
	{
		choice1.text = "";
		choice2.text = "";
		choice3.text = "";
	}
	void disableSelectables()
	{

		KMSelectable selectable1 = phoneScreenArea.Find("selectable_1").gameObject.GetComponent<KMSelectable>();
		KMSelectable selectable2 = phoneScreenArea.Find("selectable_2").gameObject.GetComponent<KMSelectable>();
		KMSelectable selectable3 = phoneScreenArea.Find("selectable_3").gameObject.GetComponent<KMSelectable>();

		selectable1.gameObject.SetActive(false);
		selectable2.gameObject.SetActive(false);
		selectable3.gameObject.SetActive(false);

		choice1.transform.parent.gameObject.SetActive(false);
		choice2.transform.parent.gameObject.SetActive(false);
		choice3.transform.parent.gameObject.SetActive(false);

	}
	void choose1()
	{
		waitingresponse = false;
		if (choice1.text.Count() > 0)
		{
			string choice = choice1.text;
			response.text = "" + choice;
			if (correctChoice == choice1.text) { waitingreply = reply1Correct; }
			else { waitingreply = reply1Wrong; }
			response.transform.parent.gameObject.SetActive(true);
			emptyChoices(); disableSelectables();
		}


	}
	void choose2()
	{
		waitingresponse = false;
		if (choice2.text.Count() > 0)
		{
			string choice = choice2.text;
			response.text = "" + choice;
			if (correctChoice == choice2.text) { waitingreply = reply2Correct; }
			else { waitingreply = reply2Wrong; }
			response.transform.parent.gameObject.SetActive(true);
			emptyChoices(); disableSelectables();
		}

	}
	void choose3()
	{
		waitingresponse = false;
		if (choice3.text.Count() > 0)
		{
			string choice = choice3.text;
			response.text = "" + choice;
			if (correctChoice == choice3.text) { waitingreply = reply3Correct; }
			else { waitingreply = reply3Wrong; }
			response.transform.parent.gameObject.SetActive(true);
			emptyChoices(); disableSelectables();
		}

	}
	void OnEnable()
	{
		if (loaded)
		{
			nyapp = GetComponent<KMSelectable>();
			nyapp.OnInteract += delegate () { App(); return false; };
		}

	}

	void App()
	{
		home = false;

		GameObject nyaphone = GameObject.Find("phoneScreen");
		var materials = nyaphone.GetComponent<MeshRenderer>().materials;
		materials[1] = appScreen;
		nyaphone.GetComponent<MeshRenderer>().materials = materials;

		id.gameObject.SetActive(true);
		message.transform.parent.gameObject.SetActive(message.text.Count() > 0);
		response.transform.parent.gameObject.SetActive(response.text.Count() > 0);
		reply.transform.parent.gameObject.SetActive(reply.text.Count() > 0);

		KMSelectable selectable1 = phoneScreenArea.Find("selectable_1").gameObject.GetComponent<KMSelectable>();
		KMSelectable selectable2 = phoneScreenArea.Find("selectable_2").gameObject.GetComponent<KMSelectable>();
		KMSelectable selectable3 = phoneScreenArea.Find("selectable_3").gameObject.GetComponent<KMSelectable>();

		selectable1.gameObject.SetActive(choice1.text.Count() > 0);
		selectable2.gameObject.SetActive(choice2.text.Count() > 0);
		selectable3.gameObject.SetActive(choice3.text.Count() > 0);

		choice1.transform.parent.gameObject.SetActive(choice1.text.Count() > 0);
		choice2.transform.parent.gameObject.SetActive(choice2.text.Count() > 0);
		choice3.transform.parent.gameObject.SetActive(choice3.text.Count() > 0);

		selectable1.transform.position = choice1.transform.parent.position;
		selectable1.transform.localScale = choice1.transform.parent.localScale;
		selectable2.transform.position = choice2.transform.parent.position;
		selectable2.transform.localScale = choice2.transform.parent.localScale;
		selectable3.transform.position = choice3.transform.parent.position;
		selectable3.transform.localScale = choice3.transform.parent.localScale;

		selectable1 = phoneScreenArea.Find("selectable_1").gameObject.GetComponent<KMSelectable>();
		selectable2 = phoneScreenArea.Find("selectable_2").gameObject.GetComponent<KMSelectable>();
		selectable3 = phoneScreenArea.Find("selectable_3").gameObject.GetComponent<KMSelectable>();

		selectable1.OnInteract += delegate ()
		{
			choose1();
			return false;
		};

		selectable2.OnInteract += delegate ()
		{
			choose2();
			return false;
		};

		selectable3.OnInteract += delegate ()
		{
			choose3();
			return false;
		};


	}
	Component CopyComponent(Component original, GameObject destination)
	{
		try
		{
			System.Type type = original.GetType();
			Component copy = destination.AddComponent(type);
			// Copied fields can be restricted with BindingFlags
			System.Reflection.FieldInfo[] fields = type.GetFields();
			foreach (System.Reflection.FieldInfo field in fields)
			{
				field.SetValue(copy, field.GetValue(original));
			}
			return copy;
		}
		catch
		{
			return original;
		}

	}
}
