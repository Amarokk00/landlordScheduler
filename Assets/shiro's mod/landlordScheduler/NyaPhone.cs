using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NyaPhone : MonoBehaviour
{
    public KMBombInfo BombInfo;
    public KMSelectable HomeButton;
    public TextMesh timetext;
    public TextMesh datetext;
    public List<GameObject> appAreas;
    public Material screen;
    public GameObject Apps;
    public GameObject phoneScreen;
    public KMSelectable phoneSelectable;
    public GameObject screenArea;

    int index = 0;

    public delegate bool OnAppLoadedHandler();
    public OnAppLoadedHandler OnAppLoaded;

    public delegate bool OnHomePressHandler();
    public OnHomePressHandler OnHomePress;

    void Start()
    {

        screenArea = Instantiate(screenArea);
        HomeButton.OnInteract += delegate ()
        {
            home();
            return false;
        };

        string theDate = System.DateTime.Now.ToString("dddd MMMM dd");
        datetext.text = theDate;
        string theTime = System.DateTime.Now.ToString("HH:mm");
        timetext.text = theTime;

        searchApps();
    }

    void searchApps()
    {
        List<string> modules = BombInfo.GetModuleIDs();
        modules = modules.Distinct().ToList();

        foreach (string module in modules)
        {
            GameObject nyapp = GameObject.Find(module + "(Clone)/nyapp");
            if (nyapp != null)
            {
                loadApp(nyapp.gameObject, module);
            }
        }
        try
        {
            foreach (string module in modules)
            {
                GameObject nyapp = GameObject.Find(module + "/nyapp");
                if (nyapp != null)
                {
                    loadApp(nyapp.gameObject, module);
                }
            }
        }
        catch
        {
            

        }
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

    void loadApp(GameObject app, string module)
    {
        if (index <= appAreas.Count() - 1)
        {
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
