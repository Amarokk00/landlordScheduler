using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Rendering;

public class ShiroBirthday : MonoBehaviour
{
    public KMBombInfo BombInfo;
    public KMNeedyModule NeedyModule;
    public KMAudio KMAudio;
    public KMSelectable SolveButton;
    public GameObject confettis;
	public ParticleSystem confettisPS;
    public SpriteRenderer sprite;

    string lastTimerDigit = "!";

    void Start()
    {

        NeedyModule.OnNeedyActivation += OnNeedyActivation;
        NeedyModule.OnNeedyDeactivation += OnNeedyDeactivation;
        SolveButton.OnInteract += delegate () { Solve(); return false; };
        NeedyModule.OnTimerExpired += OnTimerExpired;
    }
	void FixedUpdate () {
        string timerText = BombInfo.GetFormattedTime();
        
        if (!timerText.EndsWith(lastTimerDigit)){
            if (confettis.activeSelf) {
                KMAudio.PlaySoundAtTransform("confetti", this.transform);
                sprite.flipX = !sprite.flipX;
            }
        }

        lastTimerDigit = timerText.Substring(timerText.Length-1);
	}
    public bool Solve()
    {
        if (confettis.activeSelf){
            float partyMeter = 0.1f * ((BombInfo.QueryWidgets("birthdayhat",null).Count()*2f)
            +(BombInfo.QueryWidgets("birthdaycake",null).Count()*2f)
            +((BombInfo.QueryWidgets("walter",null).Count()*1f)
            *((BombInfo.QueryWidgets("birthdayhat",null).Count()>=BombInfo.QueryWidgets("walter",null).Count())?1:-1))
            +((BombInfo.QueryWidgets("waltergroup",null).Count()*3f)
            *((BombInfo.QueryWidgets("birthdaycake",null).Count()>=BombInfo.QueryWidgets("waltergroup",null).Count())?1:-1)));

            bool correct = false;
            string timerText = BombInfo.GetFormattedTime();
            string timeToCheck = timerText[1].ToString();

            if (partyMeter > 0.3f){
                timeToCheck = timerText[3].ToString();
                
            }
        
            correct = (int.Parse(timeToCheck) % 2 != ((BombInfo.GetStrikes() > 0 ? BombInfo.GetStrikes() : 0) % 2));

            if (partyMeter >= 1f){
                correct = false;
            }

            if (correct){
                NeedyModule.OnPass();
                OnNeedyDeactivation();
            }else{
                NeedyModule.OnStrike();
            }
        }
        
        return false;
    }

    public void OnNeedyActivation()
    {
		confettis.SetActive(true);
        confettisPS.Play();
		
    }

    public void OnNeedyDeactivation()
    {
		confettis.SetActive(false);
        confettisPS.Stop();
    }

    public void OnTimerExpired()
    {
         float partyMeter = 0.1f * ((BombInfo.QueryWidgets("birthdayhat",null).Count()*2f)
            +(BombInfo.QueryWidgets("birthdaycake",null).Count()*2f)
            +((BombInfo.QueryWidgets("walter",null).Count()*1f)
            *((BombInfo.QueryWidgets("birthdayhat",null).Count()>=BombInfo.QueryWidgets("walter",null).Count())?1:-1))
            +((BombInfo.QueryWidgets("waltergroup",null).Count()*3f)
            *((BombInfo.QueryWidgets("birthdaycake",null).Count()>=BombInfo.QueryWidgets("waltergroup",null).Count())?1:-1)));

        confettis.SetActive(false);
        confettisPS.Stop();

        if(partyMeter >= 1f){
            NeedyModule.OnPass();
        }else{
            NeedyModule.OnStrike();
        }
        
    }

}

