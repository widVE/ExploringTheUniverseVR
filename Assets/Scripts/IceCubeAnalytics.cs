using Firebase;
using Firebase.Analytics;
using UnityEngine;
using FieldDay;
using System.Globalization;
using System.Text;
using BeauUtil;
using FieldDay;
using System;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Collections;

[StructLayout(LayoutKind.Sequential)]
public struct LogGazeData
{
    public unsafe fixed float rot[4];

    public void Write(Quaternion rot) {
        unsafe {
            fixed (float* pRot = this.rot) {
                *(Quaternion*) pRot = rot;
            }
        }
    }
}

public class IceCubeAnalytics : Singleton<IceCubeAnalytics>
{
    public static bool FirebaseEnabled { get; set; }
    public static int logVersion = 4;
    
	static string _DB_NAME = "ICECUBE";
	
    float seconds_at_start = 0f;
	
	OGDLog _ogdLog;
	
	//FirebaseConsts _firebase;
	
	bool _loggingEnabled = true;
	  
	int _viewportDataCount = 0;
    const int MAX_VIEWPORT_DATA = 36;
    LogGazeData[] _viewportData = new LogGazeData[MAX_VIEWPORT_DATA];
	
	StringBuilder m_GazeBuilder = new StringBuilder(2048);
	StringBuilder m_RotBuilder = new StringBuilder(128);

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
		//_ogdLog.UseFirebase(_firebase);
        //	_ogdLog.SetDebug(true);
		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
		
		_mainCamera = Camera.main;
		
		seconds_at_start = UnityEngine.Time.time;
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
	
	void SetGameState(string scene)
	{
		_ogdLog.BeginGameState();
		_ogdLog.GameStateParam("seconds_from_launch", UnityEngine.Time.time-seconds_at_start);
		_ogdLog.GameStateParam("scene_name", scene);
		LogGazeGameState();
		_ogdLog.SubmitGameState();
	}
	
	
    private void LogGazeGameState()
    {
		if(_mainCamera == null)
		{
			_mainCamera = Camera.main;
		}
		
        Quaternion quat = _mainCamera.transform.rotation;

		m_RotBuilder.Clear().Append("\\\"rot\\\":[").AppendNoAlloc(quat.x, 3).Append(',').AppendNoAlloc(quat.y, 3).Append(',').AppendNoAlloc(quat.z, 3).Append(',').AppendNoAlloc(quat.w, 3).Append("]");
        //Debug.Log(m_RotBuilder.ToString());
		_ogdLog.GameStateParam("rot", m_RotBuilder.ToString());
        //_ogdLog.GameStateParam("rotY", quat.y);
        //_ogdLog.GameStateParam("rotZ", quat.z);
        //_ogdLog.GameStateParam("rotW", quat.w);
    }
	
	public void LogSessionStart()
	{
		if(_loggingEnabled)
		{
			_ogdLog.BeginEvent("session_start");
			_ogdLog.SubmitEvent();
		}	
	}
	
	public void LogStartGame(string scene)
	{
		if(_loggingEnabled)
		{	
			SetGameState(scene);
			
			_ogdLog.BeginEvent("game_start");
			_ogdLog.SubmitEvent();
		}
		
		/*if (FirebaseEnabled)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, new Parameter(FirebaseAnalytics.ParameterLevelName, "antarctica"), new Parameter("start", UnityEngine.Time.time-seconds_from_start));
                //new Parameter("app_version", logVersion));
        }	*/
	}
	
    static private unsafe void WriteGazeData(StringBuilder sb, string paramName, LogGazeData[] data, int count) {
        sb.Clear().Append("{\"").Append(paramName).Append("\":\"[");
        if (count > 0) {
            for (int i = 0; i < count; i++) {
                AppendGazeFrame(sb, data[i]);
                sb.Append(',');
            }
        }
        sb.Length--; // eliminate last comma
        sb.Append("]\"}");
    }

    static private unsafe void AppendGazeFrame(StringBuilder sb, LogGazeData data) {
        sb.Append("{\\\"rot\\\":[").AppendNoAlloc(data.rot[0], 3).Append(',').AppendNoAlloc(data.rot[1], 3).Append(',').AppendNoAlloc(data.rot[2], 3).Append(',').AppendNoAlloc(data.rot[3], 3).Append("]}");
    }

	public bool LogGaze(Vector3 p, Quaternion q, string scene, bool sendToServer=false)
	{
		if(_loggingEnabled)
		{
			if(_viewportDataCount < MAX_VIEWPORT_DATA)
			{
				/*if(_viewportData[_viewportDataCount] == null)
				{
					_viewportData[_viewportDataCount] = new LogGazeData();
				}*/

				_viewportData[_viewportDataCount].Write(q);
				//_viewportData[_viewportDataCount].rot = (q.x.ToString("F3")+","+q.y.ToString("F3")+","+q.z.ToString("F3")+","+q.w.ToString("F3"));
				//_viewportData[_viewportDataCount].frame = gazeLogFrameCount;
				_viewportDataCount++;
			}
			else
			{
				sendToServer = true;
			}

            if(sendToServer)
            {
				/*string gazeData = "";
                for(int i = 0; i < _viewportDataCount; ++i)
                {
                    gazeData += JsonUtility.ToJson(_viewportData[i]);
                }*/
				
				WriteGazeData(m_GazeBuilder, "gaze_data_package", _viewportData, _viewportDataCount);
				
				SetGameState(scene);
		
				_ogdLog.Log("viewport_data", m_GazeBuilder);
				
				//Debug.Log(gazeData);
				/*_ogdLog.BeginEvent("viewport_data");
				_ogdLog.EventParam("gaze_data_package", gazeData);
				_ogdLog.SubmitEvent();*/
				
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
			SetGameState(scene);
			
			_ogdLog.BeginEvent("scene_failed");
			_ogdLog.SubmitEvent();
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
			
			SetGameState(scene);
			
			_ogdLog.BeginEvent("headset_on");
			_ogdLog.SubmitEvent();
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
			SetGameState(scene);
			
			_ogdLog.BeginEvent("headset_off");
			_ogdLog.SubmitEvent();
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
			SetGameState(scene);
			
			_ogdLog.BeginEvent("language_selected");
			_ogdLog.EventParam("language", language);
			_ogdLog.SubmitEvent();
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
			SetGameState(scene);
			
			_ogdLog.BeginEvent("object_assigned");
			_ogdLog.EventParam("object", obj);
			_ogdLog.EventParam("xPos", pos.x);
			_ogdLog.EventParam("yPos", pos.y);
			_ogdLog.EventParam("zPos", pos.z);
			_ogdLog.SubmitEvent();
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
			SetGameState(scene);
			
			_ogdLog.BeginEvent("object_selected");
			_ogdLog.EventParam("object", obj);
			_ogdLog.EventParam("xPos", pos.x);
			_ogdLog.EventParam("yPos", pos.y);
			_ogdLog.EventParam("zPos", pos.z);
			_ogdLog.SubmitEvent();
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
			SetGameState(scene);
			
			_ogdLog.BeginEvent("scene_changed");
			_ogdLog.SubmitEvent();
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
			SetGameState(scene);
			
			_ogdLog.BeginEvent("scene_begin");
			_ogdLog.SubmitEvent();
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
			SetGameState(scene);
			
			_ogdLog.BeginEvent("scene_end");
			_ogdLog.SubmitEvent();
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
			SetGameState(scene);
			
			_ogdLog.BeginEvent("script_audio_started");
			_ogdLog.EventParam("clip_identifier", clip);
			_ogdLog.SubmitEvent();
		}
    }

    public void LogAudioComplete(string clip, string scene)
    {
		if(_loggingEnabled)
		{
			SetGameState(scene);
			
			_ogdLog.BeginEvent("script_audio_completed");
			_ogdLog.EventParam("clip_identifier", clip);
			_ogdLog.SubmitEvent();
		}
    }

    public void LogCaption(string caption, string scene)
    {
		if(_loggingEnabled)
		{
			SetGameState(scene);
			
			_ogdLog.BeginEvent("caption_displayed");
			_ogdLog.EventParam("caption", caption);
			_ogdLog.SubmitEvent();
		}
    }

    public void LogObjectDisplayed(bool hasIndicator, string obj, Vector3 pos, Quaternion rot, string scene)
    {
		if(_loggingEnabled)
		{
			SetGameState(scene);
			
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
		}
    }
    #endregion // Logging
}
