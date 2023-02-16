using Firebase;
using Firebase.Analytics;
using UnityEngine;
using FieldDay;
public class IceCubeAnalytics : Singleton<IceCubeAnalytics>
{
    public static bool FirebaseEnabled { get; set; }
    public static int logVersion = 1;
    
	static string _DB_NAME = "ICECUBE";
	
    float seconds_from_start = 0f;
	
	OGDLog _ogdLog;
	
	FirebaseConsts _firebase;
	
	bool _loggingEnabled = true;
	
	[System.Serializable]
	public class LogGazeData
	{
		public string rot;
	}
	  
	int _viewportDataCount = 0;
    const int MAX_VIEWPORT_DATA = 36;
    LogGazeData[] _viewportData = new LogGazeData[MAX_VIEWPORT_DATA];
	
	Camera _mainCamera = null;
	
    void Start()
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
		
		OGDLogConsts c = new OGDLogConsts();
		c.AppId = _DB_NAME;
		c.AppVersion = UnityEngine.Application.version;
		c.ClientLogVersion = logVersion;
		_ogdLog = new OGDLog(c);
		_ogdLog.UseFirebase(_firebase);
        //	_ogdLog.SetDebug(true);
		
		_mainCamera = Camera.main;
    }
	
	void OnDestroy()
	{
		if(_ogdLog != null)
		{
			_ogdLog.Dispose();
			_ogdLog = null;
		}
	}
	
    #region Logging

    private void LogGazeGameState()
    {
		if(_mainCamera == null)
		{
			_mainCamera = Camera.main;
		}
		
        Quaternion quat = _mainCamera.transform.rotation;

        _ogdLog.GameStateParam("rotX", quat.x);
        _ogdLog.GameStateParam("rotY", quat.y);
        _ogdLog.GameStateParam("rotZ", quat.z);
        _ogdLog.GameStateParam("rotW", quat.w);
    }
	
	public void LogStartGame(string scene)
	{
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("start");
			_ogdLog.SubmitEvent();
			
			_ogdLog.BeginGameState();
			_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
			_ogdLog.GameStateParam("scene_name", scene);
			LogGazeGameState();
			_ogdLog.SubmitGameState();
			
		}
		/*if (FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, new Parameter(FirebaseAnalytics.ParameterLevelName, "antarctica"), new Parameter("start", UnityEngine.Time.time-seconds_from_start));
                //new Parameter("app_version", logVersion));
        }	*/
	}

	public bool LogGaze(Vector3 p, Quaternion q, string scene, bool sendToServer=false)
	{
		if(_loggingEnabled)
		{
			if(_viewportDataCount < MAX_VIEWPORT_DATA)
			{
				if(_viewportData[_viewportDataCount] == null)
				{
					_viewportData[_viewportDataCount] = new LogGazeData();
				}

				_viewportData[_viewportDataCount].rot = (q.x.ToString("F3")+","+q.y.ToString("F3")+","+q.z.ToString("F3")+","+q.w.ToString("F3"));
				//_viewportData[_viewportDataCount].frame = gazeLogFrameCount;
				_viewportDataCount++;
			}
			else
			{
				sendToServer = true;
			}

            if(sendToServer)
            {
				string gazeData = "";
                for(int i = 0; i < _viewportDataCount; ++i)
                {
                    gazeData += JsonUtility.ToJson(_viewportData[i]);
                }
				
				//Debug.Log(gazeData);
				_ogdLog.BeginEvent("viewport_data");
				_ogdLog.EventParam("gaze_data_package", gazeData);
				_ogdLog.SubmitEvent();
				
				_ogdLog.BeginGameState();
				_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
				_ogdLog.GameStateParam("scene_name", scene);
				LogGazeGameState();
				_ogdLog.SubmitGameState();
				
				_viewportDataCount = 0;
				return true;
			}
		}
		
		return false;
		/*if (FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, new Parameter(FirebaseAnalytics.ParameterLevelName, "antarctica"), new Parameter("start", UnityEngine.Time.time-seconds_from_start));
                //new Parameter("app_version", logVersion));
        }	*/
	}

	public void LogFailedEnd(string scene="EXTREME")
	{
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("scene_failed");
			_ogdLog.SubmitEvent();
			
			_ogdLog.BeginGameState();
			_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
			_ogdLog.GameStateParam("scene_name", scene);
			LogGazeGameState();
			_ogdLog.SubmitGameState();
		}
		/*if (FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, new Parameter(FirebaseAnalytics.ParameterLevelName, "antarctica"), new Parameter("start", UnityEngine.Time.time-seconds_from_start));
                //new Parameter("app_version", logVersion));
        }	*/
	}
	
    public void LogHeadsetOn(string scene)
    {
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("headset_on");
			_ogdLog.SubmitEvent();
			
			_ogdLog.BeginGameState();
			_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
			_ogdLog.GameStateParam("scene_name", scene);
			LogGazeGameState();
			_ogdLog.SubmitGameState();
		}
        /*if(FirebaseEnabled)
        {
            seconds_from_start = UnityEngine.Time.time;
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventScreenView, FirebaseAnalytics.ParameterValue, "headset_on");
        }*/
    }

    public void LogHeadsetOff(string scene)
    {
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("headset_off");
			_ogdLog.SubmitEvent();
			
			_ogdLog.BeginGameState();
			_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
			_ogdLog.GameStateParam("scene_name", scene);
			LogGazeGameState();
			_ogdLog.SubmitGameState();
		}
        /*if(FirebaseEnabled)
        {
            seconds_from_start = UnityEngine.Time.time;
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventScreenView, FirebaseAnalytics.ParameterValue, "headset_on");
        }*/
    }

    public void LogLanguageSelected(string language, string scene)
    {
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("language_selected");
			_ogdLog.EventParam("language", language);
			_ogdLog.SubmitEvent();
			
			_ogdLog.BeginGameState();
			_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
			_ogdLog.GameStateParam("scene_name", scene);
			LogGazeGameState();
			_ogdLog.SubmitGameState();
		}
        /*if(FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSelectItem, new Parameter("language_selected", language), new Parameter("seconds_from_start", UnityEngine.Time.time-seconds_from_start));
        }*/
    }

    public void LogObjectAssigned(string obj, string scene, Vector3 pos)
    {
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("object_assigned");
			_ogdLog.EventParam("object", obj);
			_ogdLog.EventParam("xPos", pos.x);
			_ogdLog.EventParam("yPos", pos.y);
			_ogdLog.EventParam("zPos", pos.z);
			_ogdLog.SubmitEvent();
			
			_ogdLog.BeginGameState();
			_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
			_ogdLog.GameStateParam("scene_name", scene);
			LogGazeGameState();
			_ogdLog.SubmitGameState();
        }
        /*if(FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSelectItem, new Parameter("object_assigned", obj), new Parameter("seconds_from_start", UnityEngine.Time.time-seconds_from_start));
        }*/
    }

    public void LogObjectSelected(string obj, string scene, Vector3 pos)
    {
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("object_selected");
			_ogdLog.EventParam("object", obj);
			_ogdLog.EventParam("xPos", pos.x);
			_ogdLog.EventParam("yPos", pos.y);
			_ogdLog.EventParam("zPos", pos.z);
			_ogdLog.SubmitEvent();
			
			_ogdLog.BeginGameState();
			_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
			_ogdLog.GameStateParam("scene_name", scene);
			LogGazeGameState();
			_ogdLog.SubmitGameState();
		}
       /* if(FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSelectItem, new Parameter("gaze_point_name", obj), new Parameter("seconds_from_start", UnityEngine.Time.time-seconds_from_start));
        }*/
    }

    public void LogSceneChanged(string scene)
    {
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("scene_changed");
			_ogdLog.SubmitEvent();
			
			_ogdLog.BeginGameState();
			_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
			_ogdLog.GameStateParam("scene_name", scene);
			LogGazeGameState();
			_ogdLog.SubmitGameState();
		}

        /*if (FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, new Parameter(FirebaseAnalytics.ParameterLevelName, scene), new Parameter("seconds_from_start", UnityEngine.Time.time-seconds_from_start));
                //new Parameter("app_version", logVersion));
        }*/	
    }

	public void LogSceneBegin(string scene)
    {
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("scene_begin");
			_ogdLog.SubmitEvent();
			
			_ogdLog.BeginGameState();
			_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
			_ogdLog.GameStateParam("scene_name", scene);
			LogGazeGameState();
			_ogdLog.SubmitGameState();
		}

        /*if (FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, new Parameter(FirebaseAnalytics.ParameterLevelName, scene), new Parameter("seconds_from_start", UnityEngine.Time.time-seconds_from_start));
                //new Parameter("app_version", logVersion));
        }*/	
    }
	
	public void LogSceneEnd(string scene)
    {
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("scene_end");
			_ogdLog.SubmitEvent();
			
			_ogdLog.BeginGameState();
			_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
			_ogdLog.GameStateParam("scene_name", scene);
			LogGazeGameState();
			_ogdLog.SubmitGameState();
		}

        /*if (FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, new Parameter(FirebaseAnalytics.ParameterLevelName, scene), new Parameter("seconds_from_start", UnityEngine.Time.time-seconds_from_start));
                //new Parameter("app_version", logVersion));
        }*/	
    }


    public void LogAudioStarted(string clip, string scene)
    {
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("script_audio_started");
			_ogdLog.EventParam("clip_identifier", clip);
			_ogdLog.SubmitEvent();
			
			_ogdLog.BeginGameState();
			_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
			_ogdLog.GameStateParam("scene_name", scene);
			LogGazeGameState();
			_ogdLog.SubmitGameState();
		}
    }

    public void LogAudioComplete(string clip, string scene)
    {
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("script_audio_completed");
			_ogdLog.EventParam("clip_identifier", clip);
			_ogdLog.SubmitEvent();
			
			_ogdLog.BeginGameState();
			_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
			_ogdLog.GameStateParam("scene_name", scene);
			LogGazeGameState();
			_ogdLog.SubmitGameState();
		}
    }

    public void LogCaption(string caption, string scene)
    {
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("caption_displayed");
			_ogdLog.EventParam("caption", caption);
			_ogdLog.SubmitEvent();
			
			_ogdLog.BeginGameState();
			_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
			_ogdLog.GameStateParam("scene_name", scene);
			LogGazeGameState();
			_ogdLog.SubmitGameState();
		}
    }

    public void LogObjectDisplayed(bool hasIndicator, string obj, Vector3 pos, Quaternion rot, string scene)
    {
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("object_displayed");
			_ogdLog.EventParam("has_the_indicator", hasIndicator);
			_ogdLog.EventParam("object", obj);
			_ogdLog.EventParam("posX", pos.x);
			_ogdLog.EventParam("posY", pos.y);
			_ogdLog.EventParam("posZ", pos.z);
			_ogdLog.EventParam("rotX", rot.x);
			_ogdLog.EventParam("rotY", rot.y);
			_ogdLog.EventParam("rotZ", rot.z);
			_ogdLog.EventParam("rotW", rot.w);
			_ogdLog.SubmitEvent();
			
			_ogdLog.BeginGameState();
			_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_from_start);
			_ogdLog.GameStateParam("scene_name", scene);
			LogGazeGameState();
			_ogdLog.SubmitGameState();
		}
    }
    #endregion // Logging
}
