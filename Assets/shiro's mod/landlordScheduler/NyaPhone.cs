using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NyaPhone : MonoBehaviour, PhoneInfo
{

    public event OnAppLoadedHandler OnAppLoaded;
    public event OnHomePressHandler OnHomePress;

    public KMBombInfo BombInfo;
    public KMSelectable HomeButton;
    public TextMesh timetext;
    public TextMesh datetext;

    public Material screen;
    public KMSelectable phoneSelectable;
    public GameObject Apps;
    public GameObject phoneScreen;
    List<GameObject> appAreas = new List<GameObject>();

    int index = 0;

    List<string> loadedmodules = new List<string>();

    void Start()
    {
        phoneScreen = this.transform.Find("phoneScreen").gameObject;
        Apps = this.transform.Find("AppArea").gameObject;

        foreach (Transform t in Apps.transform)
        {
            appAreas.Add(t.gameObject);
        }

        HomeButton.OnInteract += delegate ()
        {
            home();
            return false;
        };

        string theDate = System.DateTime.Now.ToString("dddd MMMM dd");
        datetext.text = theDate;
        string theTime = System.DateTime.Now.ToString("HH:mm");
        timetext.text = theTime;

    }

    void FixedUpdate()
    {
        try
        {
            string theTime = System.DateTime.Now.ToString("HH:mm");
            timetext.text = theTime;
        }
        catch { }

    }

    void home()
    {
        OnHomePress();

        var materials = phoneScreen.GetComponent<MeshRenderer>().materials;
        materials[1] = screen;
        phoneScreen.GetComponent<MeshRenderer>().materials = materials;

        datetext.gameObject.SetActive(true);
        timetext.gameObject.SetActive(true);
        Apps.SetActive(true);
    }

    public void loadApp(GameObject app, string module)
    {
        if (index <= appAreas.Count() - 1 && !loadedmodules.Contains(module))
        {
            loadedmodules.Add(module);
            GameObject area = appAreas[index];

            area.name = "nyapp_" + module;

            foreach (Component c in app.GetComponents(typeof(Component)))
            {

                CopyComponent(c, area);

            }
            foreach (Transform t in app.transform)
            {
                t.SetParent(area.transform, false);
            }
            KMSelectable nyappSelectable = area.GetComponent<KMSelectable>();

            nyappSelectable.OnInteract += delegate ()
            {
                hideHome();
                return false;
            };


            index++;

            OnAppLoaded();

        }


    }

    void hideHome()
    {

        datetext.gameObject.SetActive(false);
        timetext.gameObject.SetActive(false);
        Apps.SetActive(false);
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

