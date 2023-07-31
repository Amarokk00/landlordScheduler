using UnityEngine;

public delegate bool OnAppLoadedHandler();
public delegate bool OnHomePressHandler();

public interface PhoneInfo
{
	event OnAppLoadedHandler OnAppLoaded;
	event OnHomePressHandler OnHomePress;

	void loadApp(GameObject app, string module);
}

