using Firebase;
using Firebase.Analytics;
using UnityEngine;
using FieldDay;
public class IceCubeAnalytics : Singleton<IceCubeAnalytics>
{
    public static bool FirebaseEnabled { get; set; }
    public static int logVersion = 0;
    
	static string _DB_NAME = "ICECUBE";
	
    float seconds_from_start = 0f;
	
	OGDLog _ogdLog;
	
	FirebaseConsts _firebase;
	 
    private void Start()
    {
        // Try to initialize Firebase and fix dependencies (will always be false in editor)
        // If successful, set FirebaseEnabled flag to true allowing analytics to be sent
		/*Debug.Log("Initializing Firebase");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
	        var dependencyStatus = task.Result;
		    if (dependencyStatus == DependencyStatus.Available) {
				Debug.Log("Firebase initialized...");
                FirebaseEnabled = true;
	        } else {
		        Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
	        }
		});*/
		
		_ogdLog = new OGDLog(_DB_NAME, UnityEngine.Application.version);
		_ogdLog.UseFirebase(_firebase);
        //	_ogdLog.SetDebug(true);
    }

    #region Logging

	public void LogStartGame()
	{
        _ogdLog.BeginEvent("start");
        _ogdLog.EventParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
        _ogdLog.SubmitEvent();
		/*if (FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, new Parameter(FirebaseAnalytics.ParameterLevelName, "antarctica"), new Parameter("start", UnityEngine.Time.time-seconds_from_start));
                //new Parameter("app_version", logVersion));
        }	*/
	}

	public void LogFailedEnd()
	{
        _ogdLog.BeginEvent("failed");
        _ogdLog.EventParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
        _ogdLog.SubmitEvent();
		/*if (FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, new Parameter(FirebaseAnalytics.ParameterLevelName, "antarctica"), new Parameter("start", UnityEngine.Time.time-seconds_from_start));
                //new Parameter("app_version", logVersion));
        }	*/
	}
	
    public void LogHeadsetOn()
    {
        _ogdLog.BeginEvent("headset_on");
        _ogdLog.SubmitEvent();
        /*if(FirebaseEnabled)
        {
            seconds_from_start = UnityEngine.Time.time;
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventScreenView, FirebaseAnalytics.ParameterValue, "headset_on");
        }*/
    }

    public void LogLanguageSelected(string language)
    {
        _ogdLog.BeginEvent("language_selected");
        _ogdLog.EventParam("language", language);
        _ogdLog.EventParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
        _ogdLog.SubmitEvent();
        /*if(FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSelectItem, new Parameter("language_selected", language), new Parameter("seconds_from_start", UnityEngine.Time.time-seconds_from_start));
        }*/
    }

    public void LogObjectAssigned(string obj)
    {
        _ogdLog.BeginEvent("object_assigned");
        _ogdLog.EventParam("object", obj);
        _ogdLog.EventParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
        _ogdLog.SubmitEvent();
        
        /*if(FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSelectItem, new Parameter("object_assigned", obj), new Parameter("seconds_from_start", UnityEngine.Time.time-seconds_from_start));
        }*/
    }

    public void LogObjectSelected(string obj)
    {
        _ogdLog.BeginEvent("object_selected");
        _ogdLog.EventParam("gaze_point_name", obj);
        _ogdLog.EventParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
        _ogdLog.SubmitEvent();
       /* if(FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSelectItem, new Parameter("gaze_point_name", obj), new Parameter("seconds_from_start", UnityEngine.Time.time-seconds_from_start));
        }*/
    }

    public void LogSceneChanged(string scene)
    {
        _ogdLog.BeginEvent("scene_change");
        _ogdLog.EventParam("scene_name", scene);
        _ogdLog.EventParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
        _ogdLog.SubmitEvent();

        /*if (FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, new Parameter(FirebaseAnalytics.ParameterLevelName, scene), new Parameter("seconds_from_start", UnityEngine.Time.time-seconds_from_start));
                //new Parameter("app_version", logVersion));
        }*/	
    }

    public void LogAudioStarted(string clip)
    {
        _ogdLog.BeginEvent("script_audio_started");
        //_ogdLog.EventParam("clip_identifier", clip);
        _ogdLog.EventParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
        _ogdLog.SubmitEvent();
    }

    public void LogAudioComplete(string clip)
    {
        _ogdLog.BeginEvent("script_audio_complete");
        //_ogdLog.EventParam("clip_identifier", clip);
        _ogdLog.EventParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
        _ogdLog.SubmitEvent();
    }

    public void LogCaption(string caption)
    {
        _ogdLog.BeginEvent("caption_displayed");
        _ogdLog.EventParam("caption", caption);
        _ogdLog.EventParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
        _ogdLog.SubmitEvent();
    }

    public void LogObjectDisplayed(bool hasIndicator, string obj, Vector3 pos, Quaternion rot)
    {
        _ogdLog.BeginEvent("new_object_displayed");
        _ogdLog.EventParam("has_the_indicator", hasIndicator);
        _ogdLog.EventParam("object", obj);
        _ogdLog.EventParam("posX", pos.x);
        _ogdLog.EventParam("posY", pos.y);
        _ogdLog.EventParam("posZ", pos.z);
        _ogdLog.EventParam("rotX", rot.x);
        _ogdLog.EventParam("rotY", rot.y);
        _ogdLog.EventParam("rotZ", rot.z);
        _ogdLog.EventParam("rotW", rot.w);
        _ogdLog.EventParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
        _ogdLog.SubmitEvent();
    }
    #endregion // Logging
}
