using Firebase;
using Firebase.Analytics;
using UnityEngine;

public class IceCubeAnalytics : Singleton<IceCubeAnalytics>
{
    public static bool FirebaseEnabled { get; set; }
    public static int logVersion = 0;
    
    float seconds_from_start = 0f;

    private void Start()
    {
        // Try to initialize Firebase and fix dependencies (will always be false in editor)
        // If successful, set FirebaseEnabled flag to true allowing analytics to be sent
		Debug.Log("Initializing Firebase");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
	        var dependencyStatus = task.Result;
		    if (dependencyStatus == DependencyStatus.Available) {
				Debug.Log("Firebase initialized...");
                FirebaseEnabled = true;
	        } else {
		        Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
	        }
		});
    }

    #region Logging

	public void LogStartGame()
	{
		if (FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, FirebaseAnalytics.ParameterLevelName, "antarctica");
                //new Parameter("app_version", logVersion));
        }	
	}

    public void LogHeadsetOn()
    {
        if(FirebaseEnabled)
        {
            seconds_from_start = UnityEngine.Time.time;
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventScreenView, FirebaseAnalytics.ParameterValue, "headset_on");
        }
    }

    public void LogLanguageSelected(string language)
    {
        if(FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSelectItem, new Parameter("language_selected", language), new Parameter("seconds_from_start", UnityEngine.Time.time-seconds_from_start));
        }
    }
    #endregion // Logging
}
