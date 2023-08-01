using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Rendering;


public class landlordScheduler : MonoBehaviour
{
    public NyappLandlord app;

    public KMBombModule BombModule;

    public TextMesh DayText;
    public KMSelectable DayMinusButton;
    public KMSelectable DayPlusButton;
    int Day = 30;

    public TextMesh MonthText;
    public KMSelectable MonthMinusButton;
    public KMSelectable MonthPlusButton;
    int Month = 12;

    public TextMesh YearText;
    public KMSelectable YearMinusButton;
    public KMSelectable YearPlusButton;
    int Year = 2023;

    public TextMesh idText;
    public KMSelectable Submit;

    int todayDay = 27;
    int todayMonth = 7;
    int todayYear = 2023;

    public List<DateTime> validDates = new List<DateTime>();
    GameObject phone;

    void ActivateModule()
    {
        try
        {
            app = GameObject.Find("nyapp_landlordScheduler").GetComponent<NyappLandlord>();
            app.modules.Add(this);
            app.loadApp();
        }
        catch
        {
            try
            {
                phone = GameObject.Find("NyaPhone(Clone)");
                loadApp();

                app = GameObject.Find("nyapp_landlordScheduler").GetComponent<NyappLandlord>();
                app.modules.Add(this);
                app.loadApp();
            }
            catch (Exception e)
            {
                Debug.Log("[Landlord Scheduler Error Loading Phone] " + e);
                BombModule.HandlePass();
            }
        }
    }

    void Start()
    {

        GetComponent<KMBombModule>().OnActivate += ActivateModule;
        System.Random random = new System.Random();

        for (int i = 0; i < 10; i++)
        {
            DateTime rngDate = DateTime.Now.AddDays(random.Next(1, 1095));
            validDates.Add(rngDate);
        }

        todayDay = int.Parse(System.DateTime.Now.ToString("dd"));
        Day = todayDay;
        todayMonth = int.Parse(System.DateTime.Now.ToString("MM"));
        Month = todayMonth;
        todayYear = int.Parse(System.DateTime.Now.ToString("yyyy"));
        Year = todayYear;

        UpdateDate();

        DayMinusButton.OnInteract += delegate ()
        {
            Day--;
            UpdateDate();
            return false;
        };
        DayPlusButton.OnInteract += delegate ()
        {
            Day++;
            UpdateDate();
            return false;
        };

        MonthMinusButton.OnInteract += delegate ()
        {
            Month--;
            UpdateDate();
            return false;
        };
        MonthPlusButton.OnInteract += delegate ()
        {
            Month++;
            UpdateDate();
            return false;
        };

        YearMinusButton.OnInteract += delegate ()
        {
            Year--;
            UpdateDate();
            return false;
        };

        YearPlusButton.OnInteract += delegate ()
        {
            Year++;
            UpdateDate();
            return false;
        };

        Submit.OnInteract += delegate ()
        {
            SubmitDate();
            return false;
        };
    }

    public void SetId(int id)
    {
        idText.text = "" + id;
    }

    void SubmitDate()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        GetComponent<KMSelectable>().AddInteractionPunch();

        foreach (DateTime date in validDates)
        {
            if (date.Year == Year && date.Month == Month && date.Day == Day)
            {
                BombModule.HandlePass();
                app = GameObject.Find("nyapp_landlordScheduler").GetComponent<NyappLandlord>();
                app.modules.Remove(this);
                app.onModulePass();
                return;
            }
        }

        BombModule.HandleStrike();

    }

    void UpdateDate()
    {
        UpdateDay();
        UpdateMonth();
        UpdateYear();
    }

    void UpdateDay()
    {
        if (Day <= 0)
        {
            Day = 1;
        }
        if (Month == 2 && Day > 28)
        {
            Day = 28;
        }
        else if (Month % 2 == 0 && Day > 30)
        {
            Day = 30;
        }
        else if (Month % 2 != 0 && Day > 31)
        {
            Day = 31;
        }
        if (Year <= todayYear && Month <= todayMonth && Day <= todayDay)
        {
            Day = todayDay;
        }
        if (("" + Day).Count() <= 1)
        {
            DayText.text = "0" + Day;
        }
        else
        {
            DayText.text = "" + Day;
        }
    }

    void UpdateMonth()
    {
        if (Month <= 0)
        {
            Month = 1;
        }
        if (Month > 12)
        {
            Month = 12;
        }
        if (Year <= todayYear && Month <= todayMonth)
        {
            Month = todayMonth;
        }
        if (("" + Month).Count() <= 1)
        {
            MonthText.text = "0" + Month;
        }
        else
        {
            MonthText.text = "" + Month;
        }
    }
    void UpdateYear()
    {
        if (Year <= todayYear)
        {
            Year = todayYear;
        }
        if (Year >= todayYear + 3)
        {
            Year = todayYear + 3;
        }
        YearText.text = "" + Year;
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

    public void loadApp()
    {
        Transform Apps = phone.transform.Find("AppArea");
        GameObject area = null;

        foreach (Transform t in Apps)
        {
            if (!t.name.Contains("nyapp"))
            {
                area = t.gameObject;
                break;
            }
        }

        if (area != null)
        {
            area.name = "nyapp_landlordScheduler";

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
        }
    }

    void hideHome()
    {
        GameObject.Find("NyaPhone(Clone)/Date").SetActive(false);
        GameObject.Find("NyaPhone(Clone)/Time").SetActive(false);
        GameObject.Find("NyaPhone(Clone)/AppArea").SetActive(false);
    }
}

