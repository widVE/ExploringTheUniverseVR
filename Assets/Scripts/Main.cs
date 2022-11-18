using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using System.IO;

public class Main : MonoBehaviour
{

    public class gaze_trigger
    {
        public Vector3 position = new Vector3(0f, 0f, 0f);
        public float t_max = 1f; //The time required to confirm a gaze
        public float t_max_numb = 1f; //The time required to wait before confirming another gaze

        public float t_in = 0f; //grows to max when in, 0 when out
        public float t_numb = 0f; //countdown when distance should be ignored
        public bool just_in = false; //1 when newly in
        public bool just_triggered = false; //1 when in for > threshhold
        public float range = 0.3f; //required distance for activation

        //derived
        public float shrink = 0f;
        public float rot = 0f;

        public gaze_trigger()
        { }

        public bool in_range(Vector3 ptr)
        {
            return Vector3.Distance(this.position, ptr) < this.range;
        }

        public bool tick(Vector3 ptr, float dt)
        {
            this.just_in = false;
            this.just_triggered = false;

            bool ret = false;
            if (this.t_numb <= 0 && this.in_range(ptr))
            {
                if (this.t_in == 0) this.just_in = true; //breaking news!!
                this.t_in += dt;
                if (this.t_in >= this.t_max)
                {
                    this.just_triggered = true;
                    this.t_numb = this.t_max_numb;
                    this.t_in = this.t_max;
                }
                ret = true;
            }
            else
            {
                this.t_in = 0;
                this.t_numb -= dt;
                ret = false;
            }

            this.shrink = (this.t_in / this.t_max);
            this.rot = (this.t_in / this.t_max) * 5f * 360.0f;

            return ret;
        }

        public void reset()
        {
            this.t_in = 0f;
            this.t_numb = 0f;
            this.just_in = false;
            this.just_triggered = false;
        }

    }

    public string[] credit_strings = new string[]
    {
    string.Empty, //needs empty string at beginning so it starts off empty
    "Me",
    "You",
    "Him",
    "Her",
    "Them",
    "Everybody",
    "Else",
    string.Empty, //needs empty string at end so it eventually shuts up
    };

    int credits_i;
    float credits_t;
    float max_credits_t = 5f;

    float twopi = 3.14159265f * 2f;

	[SerializeField]
	bool ResetOnHMDRemoved = true;
	bool HeadsetPaused = false;
	[SerializeField]
	Transform ARParent;
	[SerializeField]
	Transform ARAnchor;
	[SerializeField]
	float IceFarClip = 500;
	[SerializeField]
	float SpaceFarClip = 1500;
	[SerializeField]
	bool MouseRotatesCamera = false;
	GameObject camera_house;
	OVRManager ovr_manager;
    GameObject main_camera;
    Skybox main_camera_skybox;
    GameObject portal_projection;
    GameObject portal;
    GameObject portal_camera_next;
    Skybox portal_camera_next_skybox;
    GameObject helmet;
    GameObject helmet_light;
    Light helmet_light_light;
    GameObject cam_reticle;
    GameObject cam_spinner;
    float reticle_d;
    GameObject gaze_projection;
    GameObject gaze_reticle;
    GameObject spec_projection;
    GameObject spec_viz_reticle;
    GameObject spec_gam_reticle;
    GameObject spec_neu_reticle;
    GameObject spec_sel_reticle;
    GameObject lang_esp_reticle;
    GameObject lang_eng_reticle;
    GameObject lang_por_reticle;
    GameObject lang_sel_reticle;
    GameObject gazeray;
    GameObject gazeball;
    GameObject grid;
	EventPlayer grid_eventPlayer;
    bool event_player_logged;
	GameObject ar_group;
    GameObject ar_camera_project;
    GameObject ar_camera_static;
    GameObject[] ar_maps;
    GameObject ar_alert;
    GameObject ar_timer;
    TextMesh ar_timer_text;
    GameObject credits_0;
    GameObject credits_1;
    TextMesh credits_text_0;
    TextMesh credits_text_1;
    GameObject subtitles;
	TMPro.TextMeshPro subtitles_text;
    GameObject dropdown;
    GameObject lang_esp_reticle1;
    GameObject lang_eng_reticle1;
    GameObject lang_por_reticle1;
    GameObject lang_sel_reticle1;
    GameObject languagesel;
    GameObject startsel;
    GameObject menuscreen;
    GameObject languageop;
	bool languageAnalyticsSent = false;
    bool logged_spec_viz = false;
    bool logged_spec_gam = false;
    bool logged_spec_neu = false;
    GameObject startbutton;
    GameObject start_text;
    //TextMesh st_textmesh;
	TMPro.TextMeshPro start_tmp;
    GameObject lang_text;
    //TextMesh language_textmesh;
	TMPro.TextMeshPro language_tmp;
	GameObject language;

    int MAX_LABELS = 5;
    GameObject[] ar_label_lefts;
    GameObject[] ar_label_left_kids;
    GameObject[] ar_label_left_quads;
    TextMesh[] ar_label_left_texts;
    GameObject[] ar_label_rights;
    GameObject[] ar_label_right_kids;
    GameObject[] ar_label_right_quads;
    TextMesh[] ar_label_right_texts;
    GameObject[] ar_label_bhs;
    GameObject[] ar_label_bh_kids;
    GameObject[] ar_label_bh_quads;
    TextMesh[] ar_label_bh_texts;
    GameObject[] ar_progresses;
    GameObject[] ar_progress_offsets;
    LineRenderer[] ar_progress_lines;
    GameObject[] ar_label_checks;
    GameObject[] ar_spec_checks;

    GameObject[] icecube;
    GameObject[] pluto;
    GameObject[] vearth;
    GameObject[] milky;
    GameObject[] nearth;
    GameObject[] blackhole;
    GameObject[] esun;
    GameObject[] earth;
    //GameObject stars;
    //GameObject starsscale;

    int grid_w = 10;
    int grid_h = 10;
    int grid_d = 10;
    float grid_oyoff = 0f;
    float grid_yoff = -1000f;
    float grid_yvel = 0f;
    float grid_yacc = 0f;
    float default_grid_s;
    float[,,] grid_s;
    //GameObject[,,] grid_bulbs;

    Vector3 default_portal_scale;
    Vector3 default_portal_position;
    Vector3 default_look_ahead;
    Vector3 look_ahead;
    Vector3 lazy_look_ahead;
    Vector3 very_lazy_look_ahead;
    Vector3 super_lazy_look_ahead;
    Vector3 player_head;

    public int starting_scene = 0;

    public Material alpha_material;
    public GameObject star_prefab;
    public GameObject ar_label_left_prefab;
    public GameObject ar_label_right_prefab;
    public GameObject ar_label_bh_prefab;
    public GameObject ar_progress_prefab;
    public GameObject grid_string_prefab;
    public GameObject grid_bulb_prefab;
    public GameObject ar_check_prefab;

    public Color scene0_helmet_color;
    public Color scene1_helmet_color;
    public Color scene2_helmet_color;
    public Color scene3_helmet_color;
    public Color scene4_helmet_color;
    Color[] helmet_colors; //wrangle into array in Start for easier accessibility

    public float extreme_camera_shake = 0.2f;
    public float extreme_helmet_shake = 0.001f;

    [Range(0.0f, 1.0f)]
    public float ice_viz_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float ice_gam_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float ice_neu_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float ice_end_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float voyager_viz_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float voyager_gam_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float voyager_neu_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float voyager_end_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float nothing_viz_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float nothing_gam_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float nothing_neu_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float nothing_end_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float extreme_viz_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float extreme_gam_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float extreme_neu_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float extreme_end_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float earth_viz_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float earth_gam_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float earth_neu_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float earth_end_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float credits_viz_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float credits_gam_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float credits_neu_voice_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float credits_end_voice_vol = 1.0f;

    [Range(0.0f, 1.0f)]
    public float ice_viz_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float ice_gam_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float ice_neu_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float voyager_viz_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float voyager_gam_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float voyager_neu_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float nothing_viz_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float nothing_gam_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float nothing_neu_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float extreme_viz_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float extreme_gam_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float extreme_neu_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float earth_viz_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float earth_gam_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float earth_neu_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float credits_viz_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float credits_gam_music_vol = 1.0f;
    [Range(0.0f, 1.0f)]
    public float credits_neu_music_vol = 1.0f;

    //public GoogleAnalyticsV4 ga;

    int alpha_id;
    float flash_alpha;
    int time_mod_twelve_pi_id;
    float time_mod_twelve_pi;
    int jitter_id;
    float jitter;
    float jitter_countdown;
    int jitter_state;
    float jitter_min_downtime = 0.1f;
    float jitter_max_downtime = 1f;
    float jitter_min_uptime = 0.1f;
    float jitter_max_uptime = 0.4f;

    float alert_t;
    float timer_t;

    GameObject arrow;
    AudioSource voiceover_audiosource;
    AudioSource espAudio;
    AudioSource porAudio;
    bool voiceover_was_playing;
    string[,] voiceover_files;
    string[,] engVoiceover_files;
    string[,] espVoiceover_files;
    string[,] porVoiceover_files;
    AudioClip[,] voiceovers;
    //AudioClip[,] engVoiceovers; //unused??
    AudioClip[,] espVoiceovers;
    AudioClip[,] porVoiceovers;
	bool[,] voiceoversLoaded;
	bool[,] espVoiceoversLoaded;
	bool[,] porVoiceoversLoaded;
	float[,] voiceover_vols;
    float[,] voiceover_volsON;
    float[,] voiceover_volsOFF;
    int MAX_SUBTITLES_PER_CLIP = 50;
    string[,,] subtitle_strings;
    float[,,] subtitle_cues_delta;
    float[,,] subtitle_cues_absolute;
    int subtitle_i;
    bool end;
    float subtitle_t;
    int subtitle_spec;
    int subtitle_pause_i_ice_0 = 0;
    int subtitle_pause_i_ice_1 = 0;
    int subtitle_pause_i_ice_2 = 0;
    bool advance_passed_ice_0 = false;
    bool advance_passed_ice_1 = false;
    int subtitle_pause_i_voyager_0 = 0;
    int subtitle_pause_i_voyager_1 = 0;
    int subtitle_pause_i_voyager_2 = 0;
    bool advance_passed_voyager_0 = false;
    bool advance_passed_voyager_1 = false;
    bool advance_paused = false;
    bool hmd_mounted = false;
    //for initial screen
    bool language_selected = false;
    bool lang_menu = false;
    bool starting = false;
    bool[,] voiceovers_played;
    AudioSource music_audiosource;
    bool music_was_playing;
	//string[,] music_files;
	//AudioClip[,] musics;
	[SerializeField]
	AudioClip[] viz_music;
	[SerializeField]
	AudioClip[] gam_music;
	[SerializeField]
	AudioClip[] neu_music;
	//bool[,] musics_loaded;
    float[,] music_vols;
    AudioSource[] sfx_audiosource;
    AudioSource warp_audiosource_ptr; //<- PTR! DON'T ALLOCATE!
    int n_sfx_audiosources;
    int sfx_audiosource_i;
    string[] sfx_files = new string[]
    {
    "sfx/alert_fast",
    "sfx/alert_slow",
    "sfx/complete",
    "sfx/select",
    "sfx/warp",
    "sfx/warp_click"
    };
    //very clunky, but whatever
    enum SFX
    {
        ALERT_FAST,
        ALERT_SLOW,
        COMPLETE,
        SELECT,
        WARP,
        WARP_CLICK,
        COUNT
    };
    AudioClip[] sfxs;
	bool[] sfxs_loaded;
    float[] sfx_vols;

    int audio_started_spec;
    int audio_started_subtitle;
    int audio_started_scene;

    AudioSource PlaySFX(SFX s)
    {
        if (sfx_audiosource[sfx_audiosource_i].isPlaying) sfx_audiosource[sfx_audiosource_i].Stop();
		int sfx_i = (int)s;
		if (!sfxs_loaded[sfx_i])
		{	//sound effects are loaded as needed
			sfxs[sfx_i] = Resources.Load<AudioClip>(sfx_files[sfx_i]);
			sfxs_loaded[sfx_i] = true;
		}
        sfx_audiosource[sfx_audiosource_i].clip = sfxs[sfx_i];
        sfx_audiosource[sfx_audiosource_i].volume = sfx_vols[sfx_i];
        sfx_audiosource[sfx_audiosource_i].Play();
        AudioSource used_source = sfx_audiosource[sfx_audiosource_i];
        sfx_audiosource_i = (sfx_audiosource_i + 1) % n_sfx_audiosources;
        return used_source;
    }

	AudioClip GetMusicClip(int scene, int spec)
	{
		switch (spec)
		{
			case (int)SPEC.VIZ:
				return viz_music[scene];
			case (int)SPEC.GAM:
				return gam_music[scene];
			case (int)SPEC.NEU:
				return neu_music[scene];
			default:
				return null;
		}
		//if (!musics_loaded[i, j])
		//{
		//	musics[i, j] = Resources.Load<AudioClip>(music_files[i, j]);
		//	musics_loaded[i, j] = true;
		//}
		//return musics[i, j];
	}

	Material GetSkybox(int scene, int spec)
	{
		switch (spec)
		{
			case (int)SPEC.VIZ:
				return viz_skyboxes[scene];
			case (int)SPEC.GAM:
				return gam_skyboxes[scene];
			case (int)SPEC.NEU:
				return neu_skyboxes[scene];
			default:
				return null;
		}
	}

    void MapVols()
    {
        voiceover_vols[(int)SCENE.ICE, (int)SPEC.VIZ] = ice_viz_voice_vol;
        voiceover_vols[(int)SCENE.ICE, (int)SPEC.GAM] = ice_gam_voice_vol;
        voiceover_vols[(int)SCENE.ICE, (int)SPEC.NEU] = ice_neu_voice_vol;
        voiceover_vols[(int)SCENE.ICE, (int)SPEC.COUNT] = ice_end_voice_vol;
        voiceover_vols[(int)SCENE.VOYAGER, (int)SPEC.VIZ] = voyager_viz_voice_vol;
        voiceover_vols[(int)SCENE.VOYAGER, (int)SPEC.GAM] = voyager_gam_voice_vol;
        voiceover_vols[(int)SCENE.VOYAGER, (int)SPEC.NEU] = voyager_neu_voice_vol;
        voiceover_vols[(int)SCENE.VOYAGER, (int)SPEC.COUNT] = voyager_end_voice_vol;
        voiceover_vols[(int)SCENE.NOTHING, (int)SPEC.VIZ] = nothing_viz_voice_vol;
        voiceover_vols[(int)SCENE.NOTHING, (int)SPEC.GAM] = nothing_gam_voice_vol;
        voiceover_vols[(int)SCENE.NOTHING, (int)SPEC.NEU] = nothing_neu_voice_vol;
        voiceover_vols[(int)SCENE.NOTHING, (int)SPEC.COUNT] = nothing_end_voice_vol;
        voiceover_vols[(int)SCENE.EXTREME, (int)SPEC.VIZ] = extreme_viz_voice_vol;
        voiceover_vols[(int)SCENE.EXTREME, (int)SPEC.GAM] = extreme_gam_voice_vol;
        voiceover_vols[(int)SCENE.EXTREME, (int)SPEC.NEU] = extreme_neu_voice_vol;
        voiceover_vols[(int)SCENE.EXTREME, (int)SPEC.COUNT] = extreme_end_voice_vol;
        voiceover_vols[(int)SCENE.EARTH, (int)SPEC.VIZ] = earth_viz_voice_vol;
        voiceover_vols[(int)SCENE.EARTH, (int)SPEC.GAM] = earth_gam_voice_vol;
        voiceover_vols[(int)SCENE.EARTH, (int)SPEC.NEU] = earth_neu_voice_vol;
        voiceover_vols[(int)SCENE.EARTH, (int)SPEC.COUNT] = earth_end_voice_vol;
        voiceover_vols[(int)SCENE.CREDITS, (int)SPEC.VIZ] = credits_viz_voice_vol;
        voiceover_vols[(int)SCENE.CREDITS, (int)SPEC.GAM] = credits_gam_voice_vol;
        voiceover_vols[(int)SCENE.CREDITS, (int)SPEC.NEU] = credits_neu_voice_vol;
        voiceover_vols[(int)SCENE.CREDITS, (int)SPEC.COUNT] = credits_end_voice_vol;

        music_vols[(int)SCENE.ICE, (int)SPEC.VIZ] = ice_viz_music_vol;
        music_vols[(int)SCENE.ICE, (int)SPEC.GAM] = ice_gam_music_vol;
        music_vols[(int)SCENE.ICE, (int)SPEC.NEU] = ice_neu_music_vol;
        music_vols[(int)SCENE.VOYAGER, (int)SPEC.VIZ] = voyager_viz_music_vol;
        music_vols[(int)SCENE.VOYAGER, (int)SPEC.GAM] = voyager_gam_music_vol;
        music_vols[(int)SCENE.VOYAGER, (int)SPEC.NEU] = voyager_neu_music_vol;
        music_vols[(int)SCENE.NOTHING, (int)SPEC.VIZ] = nothing_viz_music_vol;
        music_vols[(int)SCENE.NOTHING, (int)SPEC.GAM] = nothing_gam_music_vol;
        music_vols[(int)SCENE.NOTHING, (int)SPEC.NEU] = nothing_neu_music_vol;
        music_vols[(int)SCENE.EXTREME, (int)SPEC.VIZ] = extreme_viz_music_vol;
        music_vols[(int)SCENE.EXTREME, (int)SPEC.GAM] = extreme_gam_music_vol;
        music_vols[(int)SCENE.EXTREME, (int)SPEC.NEU] = extreme_neu_music_vol;
        music_vols[(int)SCENE.EARTH, (int)SPEC.VIZ] = earth_viz_music_vol;
        music_vols[(int)SCENE.EARTH, (int)SPEC.GAM] = earth_gam_music_vol;
        music_vols[(int)SCENE.EARTH, (int)SPEC.NEU] = earth_neu_music_vol;
        music_vols[(int)SCENE.CREDITS, (int)SPEC.VIZ] = credits_viz_music_vol;
        music_vols[(int)SCENE.CREDITS, (int)SPEC.GAM] = credits_gam_music_vol;
        music_vols[(int)SCENE.CREDITS, (int)SPEC.NEU] = credits_neu_music_vol;

        if (voiceover_was_playing)
        {
            if (LocalizationManager.instance.spanish)
            {
                espAudio.volume = voiceover_vols[cur_scene_i, cur_spec_i];
            }
            else if (LocalizationManager.instance.portuguese)
            {
                porAudio.volume = voiceover_vols[cur_scene_i, cur_spec_i];
            }
            else
            {
                voiceover_audiosource.volume = voiceover_vols[cur_scene_i, cur_spec_i];
            }
        }

        if (music_was_playing)
            music_audiosource.volume = music_vols[cur_scene_i, cur_spec_i];
    }

	//string[,] skybox_files;
	//Material[,] skyboxes;
	//Skyboxes are stored in the smae order of the SCENE enum
	[SerializeField]
	Material[] viz_skyboxes;
	[SerializeField]
	Material[] gam_skyboxes;
	[SerializeField]
	Material[] neu_skyboxes;

    Vector3[] scene_centers = new Vector3[]
    {
    new Vector3(0f, 0f, 0f), //ice
    new Vector3(0f, 0f, 0f), //voyager
    new Vector3(0f, 0f, 0f), //nothing
    new Vector3(0f, 0f, 0f), //extreme
    new Vector3(0f, 0f, 0f), //earth
    new Vector3(0f, 0f, 0f), //credits
    };

    float[] scene_rots = new float[]
    {
    0f, //ice
    0f, //voyager
    0f, //nothing
    0f, //extreme
    0f, //earth
    0f, //credits
    };
    float[] scene_rot_deltas = new float[]
    {
    0.0f, //ice
    0.0f, //voyager
    0.0f, //nothing
    0.0f, //extreme
    0.0f, //earth
    0.0f, //credits
    };

    public enum SCENE
    {
        ICE,
        VOYAGER,
        NOTHING,
        EXTREME,
        EARTH,
        CREDITS,
        COUNT
    };

    public enum SPEC
    {
        VIZ,
        GAM,
        NEU,
        COUNT
    };

    int cur_scene_i;
    int next_scene_i;
    int cur_spec_i;
    int[,] layers;
    string[] spec_names;
    string[] scene_names;
    string[,] layer_names;
    GameObject[,] scene_groups;
    float[,] ta; //"time alive" (timed amt in scene/spectrum pairs)
    int[] blackhole_spec_triggered;
    float scan_t = 5f; //time required before you can consider a spectrum "scanned"
    int default_layer;

    bool mouse_captured;
    bool mouse_just_captured;
    float mouse_x;
    float mouse_y;

    float in_portal_motion;
    float out_portal_motion;
    float max_portal_motion;

    float in_fail_motion;
    float out_fail_motion;
    float max_fail_motion;

    gaze_trigger advance_trigger;
    gaze_trigger spec_trigger;
    gaze_trigger warp_trigger;
    gaze_trigger blackhole_trigger;
    gaze_trigger language_trigger;
    gaze_trigger language1_trigger;
    gaze_trigger languagepor_trigger;
    gaze_trigger start_trigger;
    gaze_trigger menulanguage;

    float dumb_delay_t_max; //prevent anything interesting til this point
    float dumb_delay_t;

    Vector3 gaze_pt;
    Vector3 anti_gaze_pt;
    Vector2 cam_euler;
    Vector2 gaze_cam_euler;
    Vector2 anti_gaze_cam_euler;
    Vector2 spec_euler;
    Vector2 lang_euler;

    Vector2 getEuler(Vector3 v)
    {
        float plane_dist = new Vector2(v.x, v.z).magnitude;
        return new Vector2(Mathf.Atan2(v.y, plane_dist), -1 * (Mathf.Atan2(v.z, v.x) - Mathf.PI / 2));
    }

    Vector2 getCamEuler(Vector3 v)
    {
        return getEuler(v - main_camera.transform.position);
    }

    Quaternion rotationFromEuler(Vector2 euler)
    {
        return Quaternion.Euler(-Mathf.Rad2Deg * euler.x, Mathf.Rad2Deg * euler.y, 0);
    }

	public void TriggerRestart()
	{
		HandleHMDUnmounted();
		HandleHMDMounted();
	}

    void HandleHMDMounted()
    {
        reStart();
        SetSpec((int)SPEC.VIZ, false);
        SetupScene();
        hmd_mounted = true;
        menuscreen.SetActive(true);
        //may need to restart analytics...

        IceCubeAnalytics.Instance.LogHeadsetOn(((SCENE)cur_scene_i).ToString());
    }

    void HandleHMDUnmounted()
    {
		if (Application.platform != RuntimePlatform.Android)
        {
			StreamWriter w;
			using (w = File.AppendText("play.log"))
			{
				string time = System.DateTime.Now.ToLongTimeString();
				string date = System.DateTime.Now.ToShortDateString();

				w.WriteLine(date + ", " + time + ", " + next_scene_i);
				w.Close();
			}
		}
        reStart();
        SetSpec((int)SPEC.VIZ, false);
        SetupScene();
        hmd_mounted = false;
        menuscreen.SetActive(false);
        arrow.SetActive(false);
	}

    void UpdateCaptions()
    {
        int i;
        int j;
        int k;

        //prepopulate with defaults
        for (i = 0; i < (int)SCENE.COUNT; i++)
            for (j = 0; j < (int)SPEC.COUNT + 1; j++)
                for (k = 0; k < MAX_SUBTITLES_PER_CLIP; k++)
                {
                    subtitle_strings[i, j, k] = string.Empty;
                    subtitle_cues_delta[i, j, k] = 0.0001f;
                    //subtitle_cues_absolute[i, j, k] = 0.0001f;
                }

        //ICE
        i = (int)SCENE.ICE;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 0.001f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica");//"Hey! Come in!"; 
        subtitle_cues_delta[i, j, k] = 1f; //1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica01");//"... Hello? ..."; 
        subtitle_cues_delta[i, j, k] = 1f; //2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica02");//"Sorry, we're still getting the kinks worked out of this new suit."; 
        subtitle_cues_delta[i, j, k] = 2.9f;//4.9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica03");//"Let me know if this is working-"; 
        subtitle_cues_delta[i, j, k] = 1.5f;//6.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica04");//"I'm booting up the augmented reality overlay in your helmet now..."; 
        subtitle_cues_delta[i, j, k] = 4f;//10.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica05");//"Ok. Can you look up at the gaze point for me?";
        subtitle_cues_delta[i, j, k] = 2.5f;//12.9
        subtitle_pause_i_ice_0 = k;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica06");//"Great! Now look at the one at your feet.";
        subtitle_cues_delta[i, j, k] = 2f;//14.9
        subtitle_pause_i_ice_1 = k;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica07");//"Alright! Everything seems to be in order.";
        subtitle_cues_delta[i, j, k] = 2.25f;//17.15
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica08");//"Welcome to Ice Cube!";
        subtitle_cues_delta[i, j, k] = 2f;//19.15
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica09");//"I'm glad you could make it all the way";
        subtitle_cues_delta[i, j, k] = 1.5f;//20.65
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica10");//"down to antarctica for this mission.";
        subtitle_cues_delta[i, j, k] = 1.5f;//22.15
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica11");//"Before we send you off,";
        subtitle_cues_delta[i, j, k] = 2f;//24.15
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica12");//"let's brief you on why you're here:";
        subtitle_cues_delta[i, j, k] = 2f;//26.15
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica13");//"The Ice Cube Research Facility detects neutrino particles";
        subtitle_cues_delta[i, j, k] = 2.75f;//28.9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica14");//"sent from deep out in space.";
        subtitle_cues_delta[i, j, k] = 1.2f;//30.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica15");// "I'll show you the sensors on your helmet overlay.";
        subtitle_cues_delta[i, j, k] = 2f;//32.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica16");//"See that grid below the facility?";
        subtitle_cues_delta[i, j, k] = 2f;//34.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica17");// "Each dot is a sensor that detects light";
        subtitle_cues_delta[i, j, k] = 3f;//37.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica18");// "from a passing neutrino particle.";
        subtitle_cues_delta[i, j, k] = 2f;//39.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica19");//"Look! It's just detected one now!";
        subtitle_cues_delta[i, j, k] = 3f;//42.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica20");// "This is great timing-";
        subtitle_cues_delta[i, j, k] = 2f;//44.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica21");// "we'll use the sensor data";
        subtitle_cues_delta[i, j, k] = 1f;//45.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica22");// "to pinpoint where this came from in outer space...";
        subtitle_cues_delta[i, j, k] = 2f;//47.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica23");// "annnnnd... done!";
        subtitle_cues_delta[i, j, k] = 2f;//49.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica24");// "Now your job is to follow this trajectory";
        subtitle_cues_delta[i, j, k] = 3f;//52.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica25");// "out into space to find the source";
        subtitle_cues_delta[i, j, k] = 2f;//54.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica26");// "You're going to use your suit's Impossible Drive";
        subtitle_cues_delta[i, j, k] = 3f;//57.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica27");// "and follow the path of the neutrino-";
        subtitle_cues_delta[i, j, k] = 2f;//59.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica28");// "All you have do to is";
        subtitle_cues_delta[i, j, k] = 1f;//60.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica29");// "look at the gaze point at the end of the path...";
        subtitle_cues_delta[i, j, k] = 2f;//62.1
        k++;
        subtitle_cues_delta[i, j, k] = 2f;//64.1
        k++;

        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //VOYAGER
        i = (int)SCENE.VOYAGER;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager");// "Hello? ...";
        subtitle_cues_delta[i, j, k] = 0.5f;//1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager01");// "You still there?";
        subtitle_cues_delta[i, j, k] = 0.5f;//1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager02");// "Did you make it in one piece?";
        subtitle_cues_delta[i, j, k] = 1f;//2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager03");// "Take a second to look around and find your bearings-";
        subtitle_cues_delta[i, j, k] = 3f;//5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager04");// "it's probably pretty cool to be";
        subtitle_cues_delta[i, j, k] = 2f;//7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager05");// "further out in space than any other human has ever been!";
        subtitle_cues_delta[i, j, k] = 2f;//9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager06");// "Now you have a job to do.";
        subtitle_cues_delta[i, j, k] = 2f;//11
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager07");// "Follow the path of the neutrino";
        subtitle_cues_delta[i, j, k] = 2f;//13
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager08");// "that was detected by Ice Cube";
        subtitle_cues_delta[i, j, k] = 1f;//14
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager09");// "to discover the source.";
        subtitle_cues_delta[i, j, k] = 2f;//16
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager10");// "While we're waiting for the Impossible Drive to recharge,";
        subtitle_cues_delta[i, j, k] = 3f;//19
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager11");// "let's go over some other features of your suit.";
        subtitle_cues_delta[i, j, k] = 2f;//21
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager12");// "If you look at your feet,";
        subtitle_cues_delta[i, j, k] = 2f;//23
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager13");// "you can use the gaze points to";
        subtitle_cues_delta[i, j, k] = 1f;//24
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager14");// "switch out your helmet's view.";
        subtitle_cues_delta[i, j, k] = 2f;//26
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager15");// "Go ahead-";
        subtitle_cues_delta[i, j, k] = 1f;//27
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager16");// "look at your feet and switch to X-ray view.";
        subtitle_cues_delta[i, j, k] = 3f;//30
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.GAM;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager17");// "Pretty great, right?";
        subtitle_cues_delta[i, j, k] = 2f;//2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager18");// "Look around- check out the galaxy!";
        subtitle_cues_delta[i, j, k] = 2f;//4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager19");// "This is what the universe looks like when we see with X-rays.";
        subtitle_cues_delta[i, j, k] = 4f;//8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager20");// "Your helmet is detecting X-rays";
        subtitle_cues_delta[i, j, k] = 2f;//10
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager21");// "in the same way your eye would normally detect light.";
        subtitle_cues_delta[i, j, k] = 2f;//12
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager22");// "Can you look at Pluto for a second?";
        subtitle_cues_delta[i, j, k] = 2f;//14
        subtitle_pause_i_voyager_0 = k;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager23");// "See how it's just a big black ball?";
        subtitle_cues_delta[i, j, k] = 2f;//16
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager24");// "That's because X-rays don't pass through it.";
        subtitle_cues_delta[i, j, k] = 3f;//19
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager25");// "Now, let's switch to neutrino vision.";
        subtitle_cues_delta[i, j, k] = 2f;//21
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.NEU;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("alright");// "Alright- look back to Pluto:";
        subtitle_cues_delta[i, j, k] = 2f;//2
        subtitle_pause_i_voyager_1 = k;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino01");// "where'd it go?!";
        subtitle_cues_delta[i, j, k] = 1f;//3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino02");// "Pluto seems to have disappeared!";
        subtitle_cues_delta[i, j, k] = 1f;//4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino03");// "Your helmet is now only sensing neutrino particles.";
        subtitle_cues_delta[i, j, k] = 3f;//7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino04");// "Neutrinos pass through just about everything,";
        subtitle_cues_delta[i, j, k] = 3f;//10
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino05");// "Even whole planets!";
        subtitle_cues_delta[i, j, k] = 1f;//11
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino06");// "It's like Pluto doesn't even exist to them!";
        subtitle_cues_delta[i, j, k] = 3f;//13
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino07");// "When you're ready,";
        subtitle_cues_delta[i, j, k] = 0.5f;//15
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino08");// "Look to the gaze point at the end of the neutrino path.";
        subtitle_cues_delta[i, j, k] = 2.5f;//18
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //NOTHING
        i = (int)SCENE.NOTHING;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey");// "We're getting some... pretty intense readings.";
        subtitle_cues_delta[i, j, k] = 2f;//2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey01");// "You're... really far out in space.";
        subtitle_cues_delta[i, j, k] = 3f;//5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey02");// "Ok- time to brief you with the details of your mission.";
        subtitle_cues_delta[i, j, k] = 3f;//8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey03");// "As you've seen, we've given your suit the ability";
        subtitle_cues_delta[i, j, k] = 3f;//11
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey04");// "to see in three different ways:";
        subtitle_cues_delta[i, j, k] = 1f;//12
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey05");// "Visible light, X-ray vision, and neutrino detection.";
        subtitle_cues_delta[i, j, k] = 4f;//16
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey06");// "The first two have been used for decades to look out into space.";
        subtitle_cues_delta[i, j, k] = 4f;//20
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey07");// "But if we want to see really far,";
        subtitle_cues_delta[i, j, k] = 2f;//22
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey08");// "Neutrinos are the only thing that will work.";
        subtitle_cues_delta[i, j, k] = 2f;//24
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey09");// "That's why we need the Ice Cube observatory.";
        subtitle_cues_delta[i, j, k] = 3f;//27
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey10");// "The arrays of sensors in antartica";
        subtitle_cues_delta[i, j, k] = 3f;//30
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey11");// "allow us to detect neutrinos from deep space.";
        subtitle_cues_delta[i, j, k] = 2f;//32
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey12");// "That helps us map out parts of the universe";
        subtitle_cues_delta[i, j, k] = 3f;//35
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey13");// "invisible to other telescopes.";
        subtitle_cues_delta[i, j, k] = 1f;//36
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey14");// "The question you have to answer is:";
        subtitle_cues_delta[i, j, k] = 3f;//39
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey15");// "What sent the neutrino that";
        subtitle_cues_delta[i, j, k] = 2f;//41
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey16");// "Ice Cube detected back at Earth?";
        subtitle_cues_delta[i, j, k] = 2f;//43
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey17");// "When you find the source at the end of your journey,";
        subtitle_cues_delta[i, j, k] = 3f;//46
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey18");// "you'll need to collect data from it";
        subtitle_cues_delta[i, j, k] = 1f;//47
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey19");// "using each of the three methods we've given you.";
        subtitle_cues_delta[i, j, k] = 2f;//49
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey20");// "You'll use your visible light,";
        subtitle_cues_delta[i, j, k] = 2f;//51
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey21");// "X-ray, and neutrino view to collect these readings.";
        subtitle_cues_delta[i, j, k] = 4f;//55
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey22");// "Ok. Things might get dicey going forward.";
        subtitle_cues_delta[i, j, k] = 4f;//59
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey23");// "Good luck.";
        subtitle_cues_delta[i, j, k] = 3f;//61
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //EXTREME
        i = (int)SCENE.EXTREME;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("hi");// "Hello?";
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("ask");// "Do you read me?";
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("discover");// "You've discovered a black hole!";
        subtitle_cues_delta[i, j, k] = 3f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("scan");// "You need to scan it with each";
        subtitle_cues_delta[i, j, k] = 2f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("scan01");// "of your vision modules quickly!";
        subtitle_cues_delta[i, j, k] = 2f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("getout");// "Then get OUT of there!";
        subtitle_cues_delta[i, j, k] = 3f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("light");// "Make sure you've selected visibile light vision-";
        subtitle_cues_delta[i, j, k] = 2f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("collect");// "look at the black hole and collect visible light readings!";
        subtitle_cues_delta[i, j, k] = 4f;
        k++;
        subtitle_cues_delta[i, j, k] = 2f;
        k++;

        j = (int)SPEC.GAM;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("collect01");// "Look up at the black hole, and collect the X-ray readings!";
        subtitle_cues_delta[i, j, k] = 4f;
        k++;
        subtitle_cues_delta[i, j, k] = 2f;
        k++;

        j = (int)SPEC.NEU;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("collect02");// "Look back at the black hole, and collect the neutrino readings!";
        subtitle_cues_delta[i, j, k] = 4f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("nice");// "You did it! We have the data!";
        subtitle_cues_delta[i, j, k] = 3f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("path");// "Now follow the neutrino path back to Earth!";
        subtitle_cues_delta[i, j, k] = 3f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //EARTH
        i = (int)SCENE.EARTH;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("wow");// "Wow! You did it!";
        subtitle_cues_delta[i, j, k] = 2f;//2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("wow01");// "I... can't believe you're alive!";
        subtitle_cues_delta[i, j, k] = 0.5f;//2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("congrats");//  "....I mean, congratulations, agent!";
        subtitle_cues_delta[i, j, k] = 1.5f;//4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("source");// "It looks like the source";
        subtitle_cues_delta[i, j, k] = 2f;//6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("source01");// "of the neutrino particle we detected with Ice Cube";
        subtitle_cues_delta[i, j, k] = 3f;//9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("source02");// "was a black hole!";
        subtitle_cues_delta[i, j, k] = 1f;//10
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("bh");// "Black holes are one of the strangest,";
        subtitle_cues_delta[i, j, k] = 2f;//12
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole01");// "most extreme objects in the whole universe!";
        subtitle_cues_delta[i, j, k] = 2f;//14
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole02");// "Did you know that black holes can have the mass";
        subtitle_cues_delta[i, j, k] = 3f;//17
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("sn");// "of several million suns?";
        subtitle_cues_delta[i, j, k] = 2f;//19
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole03");// "One spoonful of black hole could weigh as much";
        subtitle_cues_delta[i, j, k] = 3f;//22
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole04");// "as a whole planet!";
        subtitle_cues_delta[i, j, k] = 1f;//23
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole05");// "They also emit high energy neutrinos";
        subtitle_cues_delta[i, j, k] = 3f;//26
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole06");// "that travel millions of lightyears back to Earth.";
        subtitle_cues_delta[i, j, k] = 2f;//28
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole07");// "It would have gone totally unnoticed";
        subtitle_cues_delta[i, j, k] = 3f;//31
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole08");// "if it weren't for the scientists at Ice Cube.";
        subtitle_cues_delta[i, j, k] = 2f;//33
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole09");// "Black holes are very hard to detect-";
        subtitle_cues_delta[i, j, k] = 3f;//36
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole10");// "well- because they're black!";
        subtitle_cues_delta[i, j, k] = 1f;//37
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole11");// "It's impossible to see something black on a black background of space!";
        subtitle_cues_delta[i, j, k] = 4f;//41
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("thankyou");// "Fortunately, IceCube has found a way to observe them using neutrinos!";
        subtitle_cues_delta[i, j, k] = 4f;//45
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("end");// "Well, that's mission complete on our end.";
        subtitle_cues_delta[i, j, k] = 4f;//49
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("bye");// "Until next time!";
        subtitle_cues_delta[i, j, k] = 1f;//50
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.GAM;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.NEU;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //ar_label_left_texts[0].text = LocalizationManager.instance.GetLocalizedValue("earth");
        //ar_label_left_texts[1].text = LocalizationManager.instance.GetLocalizedValue("earth");
        //ar_label_left_texts[2].text = LocalizationManager.instance.GetLocalizedValue("blackhole");

        //ar_label_right_texts[0].text = LocalizationManager.instance.GetLocalizedValue("milkyway");
        //ar_label_right_texts[3].text = LocalizationManager.instance.GetLocalizedValue("sun");

        //ar_label_bh_texts[1].text = LocalizationManager.instance.GetLocalizedValue("xray");
  
		ar_label_left_texts[0].text = LocalizationManager.instance.GetLocalizedValue("earth");//"EARTH
		ar_label_left_texts[1].text = LocalizationManager.instance.GetLocalizedValue("earth");//"EARTH
		ar_label_left_texts[2].text = LocalizationManager.instance.GetLocalizedValue("blackhole");
		ar_label_left_texts[3].text = "ICECUBE";
		
		ar_label_right_texts[0].text = "ICECUBE";
		ar_label_right_texts[1].text = LocalizationManager.instance.GetLocalizedValue("pluto");//"PLUTO";
		ar_label_right_texts[2].text = LocalizationManager.instance.GetLocalizedValue("milkyway");
		ar_label_right_texts[3].text = LocalizationManager.instance.GetLocalizedValue("sun");

        start_tmp.text = LocalizationManager.instance.GetLocalizedValue("start");
        language_tmp.text = LocalizationManager.instance.GetLocalizedValue("language");

        ar_label_bh_texts[0].text = LocalizationManager.instance.GetLocalizedValue("visible");
		ar_label_bh_texts[1].text = LocalizationManager.instance.GetLocalizedValue("xray");
		ar_label_bh_texts[2].text = LocalizationManager.instance.GetLocalizedValue("neutrino");

        //gen absolutes from deltas
        for (i = 0; i < (int)SCENE.COUNT; i++)
            for (j = 0; j < (int)SPEC.COUNT + 1; j++)
                for (k = 1; k < MAX_SUBTITLES_PER_CLIP; k++)
                {
                    subtitle_cues_absolute[i, j, k] = subtitle_cues_absolute[i, j, k - 1] + subtitle_cues_delta[i, j, k - 1];
                }

        ar_alert.GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("alert");
        ar_maps[0].transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("map");
        ar_maps[1].transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("map01");
        spec_viz_reticle.transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("visible");
        spec_gam_reticle.transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("xray");
        spec_neu_reticle.transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("neutrino");
    }


    void UpdateSpanishCues()
    {
        int i;
        int j;
        int k;

        //prepopulate with defaults
        for (i = 0; i < (int)SCENE.COUNT; i++)
            for (j = 0; j < (int)SPEC.COUNT + 1; j++)
                for (k = 0; k < MAX_SUBTITLES_PER_CLIP; k++)
                {
                    subtitle_strings[i, j, k] = string.Empty;
                    subtitle_cues_delta[i, j, k] = 0.0001f;
                    //subtitle_cues_absolute[i, j, k] = 0.0001f;
                }

        //ICE
        i = (int)SCENE.ICE;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 0.001f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica");//"�Ey! �Est�s ah�?"; 
        subtitle_cues_delta[i, j, k] = 1.2f; //1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica01");//"... �Hola? ..."; 
        subtitle_cues_delta[i, j, k] = 1f; //2.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica02");//"Perd�n, todav�a estamos tratando de arreglar algunos peque�os"; 
        subtitle_cues_delta[i, j, k] = 2.9f;//4.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica03");//defectos del nuevo traje. Dime si funciona-; 
        subtitle_cues_delta[i, j, k] = 2f;//6.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("test");//Dime si funciona-; 
        subtitle_cues_delta[i, j, k] = 1.5f;//7.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica04");//"Ahora estoy encendiendo la capa de realidad aumentada en tu casco ..."; 
        subtitle_cues_delta[i, j, k] = 3.9f;//11.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica05");//"Ok. �Puedes mirar hacia arriba y buscar el punto de mira?";
        subtitle_cues_delta[i, j, k] = 3.9f;//15.4
        subtitle_pause_i_ice_0 = k;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica06");//�Genial! Ahora mira hacia el que est� a tus pies.;
        subtitle_cues_delta[i, j, k] = 3f;//18.4
        subtitle_pause_i_ice_1 = k;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica07");//�Perfecto! Todo parece estar en orden.;
        subtitle_cues_delta[i, j, k] = 2.4f;//20.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica08");//�Bienvenido a IceCube!;
        subtitle_cues_delta[i, j, k] = 2f;//22.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica09");//Me alegro de que hayas podido llegar;
        subtitle_cues_delta[i, j, k] = 1.7f;//24.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica10");//"hasta la Ant�rtida para esta misi�n.";
        subtitle_cues_delta[i, j, k] = 2f;//26.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica11");//"Antes de que te mandemos a tu misi�n,";
        subtitle_cues_delta[i, j, k] = 2f;//28.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica12");//"deja que te expliquemos por que est�s aqu�:";
        subtitle_cues_delta[i, j, k] = 2.6f;//31.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica13");//"El Observatorio de Neutrinos IceCube detecta neutrinos que llegan";
        subtitle_cues_delta[i, j, k] = 3.5f;//34.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica14");//"desde lo profundo del espacio.";
        subtitle_cues_delta[i, j, k] = 1.7f;//36.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica15");// "Te ense�ar� los sensores en la cubierta de tu casco.";
        subtitle_cues_delta[i, j, k] = 3f;//39.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica16");//"�Ves esa cuadr�cula detr�s del edificio azul?";
        subtitle_cues_delta[i, j, k] = 3f;//42.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica17");// "Cada punto es un sensor que detecta luz";
        subtitle_cues_delta[i, j, k] = 2f;//44.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica18");// "de un neutrino pasajero.";
        subtitle_cues_delta[i, j, k] = 1.7f;//46.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica19");//"�Mira! �Acaba de detectar una ahora!";
        subtitle_cues_delta[i, j, k] = 3f;//49.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica20");// "Este es un buen momento-";
        subtitle_cues_delta[i, j, k] = 2f;//51.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica21");// "usaremos los datos de Ice Cube";
        subtitle_cues_delta[i, j, k] = 2f;//53.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica22");// "para determinar de d�nde vino este neutrino";
        subtitle_cues_delta[i, j, k] = 2.4f;//55.9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica23");// "desde el espacio exterior...";
        subtitle_cues_delta[i, j, k] = 1.2f;//57.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica24");// "yyyy... �hecho!";
        subtitle_cues_delta[i, j, k] = 2.5f;//59.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica25");// "Ahora tu trabajo es seguir esta trayectoria";
        subtitle_cues_delta[i, j, k] = 3f;//61.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica26");// "hacia el espacio y encontrar la fuente.";
        subtitle_cues_delta[i, j, k] = 3f;//62.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica27");// "Vas a usar la Impulsi�n Imposible de tu traje";
        subtitle_cues_delta[i, j, k] = 2.5f;//65.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica28");// "y la trayectoria del neutrino-";
        subtitle_cues_delta[i, j, k] = 1.8f;//67.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica29");// "Todo lo que tienes que hacer es mirar hacia";
        subtitle_cues_delta[i, j, k] = 2f;//69.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica30");//  el punto de la mira al final de la trayectoria...";
        subtitle_cues_delta[i, j, k] = 3f;//72.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica31");// "Intentalo ahora";
        subtitle_cues_delta[i, j, k] = 4f;//74.8
        k++;
        subtitle_cues_delta[i, j, k] = 3f;
        k++;

        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //VOYAGER
        i = (int)SCENE.VOYAGER;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager");// "�Hola? ...";
        subtitle_cues_delta[i, j, k] = 0.5f;// 0.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager01");// "�Sigues ah�?";
        subtitle_cues_delta[i, j, k] = 1f;// 1.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager02");// "�Llegaste en una pieza?";
        subtitle_cues_delta[i, j, k] = 1.3f;// 2.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager03");// "Echa un segundo vistazo a tu alrededor y ori�ntate-";
        subtitle_cues_delta[i, j, k] = 4.3f;// 4.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager04");// "�Es genial estar m�s lejos en el espacio";
        subtitle_cues_delta[i, j, k] = 2f;// 6.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager05");// "de lo que cualquier otro humano ha estado!";
        subtitle_cues_delta[i, j, k] = 3.1f;// 8.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager06");// "Ahora tienes un trabajo por hacer.";
        subtitle_cues_delta[i, j, k] = 2f;// 9.9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager07");// "Sigue la trayectoria del neutrino";
        subtitle_cues_delta[i, j, k] = 2f;// 10.9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager08");// "detectado por IceCube para descubrir la fuente.";
        subtitle_cues_delta[i, j, k] = 3.2f;// 11.9
        k++;

        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager10");// "Mientras esperas que se cargue la Impulsi�n Imposible,";
        subtitle_cues_delta[i, j, k] = 3.2f;// 15.9 - 23
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager11");// "vamos a repasar algunas funciones de tu traje.";
        subtitle_cues_delta[i, j, k] = 3.1f;//21
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager12");// "Si miras a tus pies,"
        subtitle_cues_delta[i, j, k] = 1.4f;//23
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager13");// "puedes usar los puntos de mira para" },
        subtitle_cues_delta[i, j, k] = 2f;//24
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager14");// "cambiar la visi�n de tu casco." },
        subtitle_cues_delta[i, j, k] = 3f;//26
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager15");// "Adelante-"
        subtitle_cues_delta[i, j, k] = 1f;//27
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager16");// "Mira abajo hacia tus pies y cambia la visi�n a rayos X." ;
        subtitle_cues_delta[i, j, k] = 4f;//30
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.GAM;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager17");// "Bueno, �verdad? �;
        subtitle_cues_delta[i, j, k] = 1f;//2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager18");// "�Mira a tu alrededor �;
        subtitle_cues_delta[i, j, k] = 1f;//2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager19");// "�chale un vistazo a la galaxia!";
        subtitle_cues_delta[i, j, k] = 2.3f;//4.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager20");// As� es como se ve el universo cuando lo miramos con rayos X";
        subtitle_cues_delta[i, j, k] = 3.8f;//8.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager21");// "Tu casco est� detectando rayos X";
        subtitle_cues_delta[i, j, k] = 2.8f;//11.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager22");// "de la misma manera que tu ojo";
        subtitle_cues_delta[i, j, k] = 2f;//13.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager23");// "normalmente detectar�a la luz.";
        subtitle_cues_delta[i, j, k] = 2f;//15.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager24");// "�Puedes mirar a Pluto por un segundo?";
        subtitle_cues_delta[i, j, k] = 4f;//17.3
        subtitle_pause_i_voyager_0 = k;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager25");// "�Ves como es una gran bola negra?";
        subtitle_cues_delta[i, j, k] = 2.8f;//20.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager26");// "Esto se debe a que los rayos X no pasan a trav�s suyo.";
        subtitle_cues_delta[i, j, k] = 3f;//23.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager27");// "Ahora cambia a la visi�n con neutrinos.";
        subtitle_cues_delta[i, j, k] = 4f;//25.3
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.NEU;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("alright");// "Bien- Ahora mira de nuevo a Pluto:";
        subtitle_cues_delta[i, j, k] = 2f;//2
        subtitle_pause_i_voyager_1 = k;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino01");// "��A d�nde se fue?!";
        subtitle_cues_delta[i, j, k] = 2f;//3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino02");// "�Parece como que Pluto desapareci�!";
        subtitle_cues_delta[i, j, k] = 2.7f;//4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino03");// "Tu casco ahora solo detecta neutrinos..";
        subtitle_cues_delta[i, j, k] = 3f;//7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino04");// "Los neutrinos pasan a trav�s de casi todo";
        subtitle_cues_delta[i, j, k] = 2.3f;//10
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino05");// "hasta de planetas enteros.!";
        subtitle_cues_delta[i, j, k] = 2f;//11
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino06");// "�Es como si Pluto ni existiera para ellos!";
        subtitle_cues_delta[i, j, k] = 2.8f;//13
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino07");// "Cuando est�s listo,";
        subtitle_cues_delta[i, j, k] = 1.8f;//15
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino08");// "mira al punto de mira al final de la trayectoria del neutrino";
        subtitle_cues_delta[i, j, k] = 4f;//18
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino09");// "y seguiras la particula detectada en IceCube";
        subtitle_cues_delta[i, j, k] = 3f;//18
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //NOTHING
        i = (int)SCENE.NOTHING;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 0.5f;//.5 was 1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey");// "Nos est�n llegando algunos.. resultados bastante intensos ";
        subtitle_cues_delta[i, j, k] = 4f;//4.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey0");// "Est�s... muy lejos en el espacio.";
        subtitle_cues_delta[i, j, k] = 3.1f;//7.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey01");// "Tiempo para informarte de los detalles de tu misi�n.";
        subtitle_cues_delta[i, j, k] = 4f;//11.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey02");//  "Como has visto, 
        subtitle_cues_delta[i, j, k] = 1.2f;//12.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey03");//"le hemos dado a tu traje la habilidad de ver en tres diferentes maneras:
        subtitle_cues_delta[i, j, k] = 4.4f;//17.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey030");// luz visible, visi�n de rayos X, y detecci�n de neutrinos. 
        subtitle_cues_delta[i, j, k] = 4.8f;//22
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey04");// "Las primeras dos se han usado durante d�cadas para observar el espacio.";
        subtitle_cues_delta[i, j, k] = 4.8f;//26.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey05");// "Pero si queremos de VERDAD ver lejos,";
        subtitle_cues_delta[i, j, k] = 2.9f;//29.7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey06");// "los neutrinos son los �nicos que funcionan.";
        subtitle_cues_delta[i, j, k] = 2.8f;//32.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey07");// "Por eso necesitamos el observatorio de IceCube."; empieza
        subtitle_cues_delta[i, j, k] = 2.9f;//35.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey08");// "Esa matriz de sensores en la Ant�rtidastring.Empty;
        subtitle_cues_delta[i, j, k] = 2.6f;//38
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey09");// "nos permite detectar neutrinos desde el espacio sideral,.";
        subtitle_cues_delta[i, j, k] = 3.5f;//41.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey10");// "cosa que nos ayuda a cartografiar ";
        subtitle_cues_delta[i, j, k] = 2.1f;//43.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey11");// "partes del universo invisibles para otros telescopios.";
        subtitle_cues_delta[i, j, k] = 3.5f;//47.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey12");// "La pregunta que tienes que responder es";
        subtitle_cues_delta[i, j, k] = 3.1f;//50.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey13");// "�Qu� creo el neutrino que IceCube detect� en la Tierra?";
        subtitle_cues_delta[i, j, k] = 3.6f;//53.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey14");// "Cuando encuentres la fuente al final de tu viaje -";
        subtitle_cues_delta[i, j, k] = 2.6f;//56.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey15");// "necesitar�s tomar datos de la fuente";
        subtitle_cues_delta[i, j, k] = 1.8f;//58.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey16");// "usando cada uno de los tres m�todos que te hemos dado.";
        subtitle_cues_delta[i, j, k] = 3f;//61.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey17");// "Usar�s la luz visible, los rayos X y los neutrino";
        subtitle_cues_delta[i, j, k] = 4.5f;//66
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey18");// "para reunir estas tres lecturas.";
        subtitle_cues_delta[i, j, k] = 2.1f;//68.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey19");//  "OK, las cosas podr�an ponerse peligrosas de ahora en adelante.";
        subtitle_cues_delta[i, j, k] = 4.7f;//72.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey20");// "Buena suerte.";
        subtitle_cues_delta[i, j, k] = 1.7f;//4.5
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //EXTREME
        i = (int)SCENE.EXTREME;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("hi");// �Hola?
        subtitle_cues_delta[i, j, k] = 1f; //1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("ask");// "�Me escuchas?" },
        subtitle_cues_delta[i, j, k] = 1.6f; //2.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("discover");// �Has descubierto un agujero negro!"
        subtitle_cues_delta[i, j, k] = 2.9f; //5.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("scan");//  "Tienes que escanearlo con cada m�dulo de visi�n R�PIDAMENTE."
        subtitle_cues_delta[i, j, k] = 3.5f; //9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("scan01");// �Despu�s sal de ah� cuando antes!
        subtitle_cues_delta[i, j, k] = 2.4f; //11.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("getout");// �Aseg�rate que has seleccionado la visi�n de luz visible-
        subtitle_cues_delta[i, j, k] = 3.5f; // 14.9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("collect");// mira al agujero negro y toma datos con luz visible!
        subtitle_cues_delta[i, j, k] = 3f; //17.9
        k++;
        subtitle_cues_delta[i, j, k] = 2f;
        k++;

        j = (int)SPEC.GAM;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("collect01");// �Mira arriba hacia el agujero negro para tomar los datos con rayos X!
        subtitle_cues_delta[i, j, k] = 4f;
        k++;
        subtitle_cues_delta[i, j, k] = 2f;
        k++;

        j = (int)SPEC.NEU;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("collect02");// �Mira arriba hacia el agujero negro para tomar datos con neutrinos!"
        subtitle_cues_delta[i, j, k] = 3.3f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("nice");//"�Lo lograste! �Tenemos los datos!"
        subtitle_cues_delta[i, j, k] = 3.3f; //3.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("path");// �Ahora sigue la trayectoria del neutrino de regreso a la Tierra!"
        subtitle_cues_delta[i, j, k] = 3.4f; //6.7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("fast");// rapido
        subtitle_cues_delta[i, j, k] = 1.3f; //8
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //EARTH
        i = (int)SCENE.EARTH;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 0.5f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("wow");// �Guau! �Lo lograste! Yo...
        subtitle_cues_delta[i, j, k] = 2f;//2.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("wow01");// 
        subtitle_cues_delta[i, j, k] = 3.2f;//5.7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("congrats");// 
        subtitle_cues_delta[i, j, k] = 1.8f;//7.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("source");//  
        subtitle_cues_delta[i, j, k] = 5f;//12.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("source01");// 
        subtitle_cues_delta[i, j, k] = 4.8f;//17.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("source02");// 
        subtitle_cues_delta[i, j, k] = 3.3f;//20.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("bh");//
        subtitle_cues_delta[i, j, k] = 2f;//22.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole01");//
        subtitle_cues_delta[i, j, k] = 3.8f;//26.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole02");//
        subtitle_cues_delta[i, j, k] = 1.2f;//27.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("sn");// 
        subtitle_cues_delta[i, j, k] = 2.7f;//30.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole03");// 
        subtitle_cues_delta[i, j, k] = 3.7f;//34
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole04");// 
        subtitle_cues_delta[i, j, k] = 2.8f;//36.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole05");// 
        subtitle_cues_delta[i, j, k] = 2.8f;//39.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole06");// 
        subtitle_cues_delta[i, j, k] = 3.3f;//42.9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole07");// 
        subtitle_cues_delta[i, j, k] = 4.1f;//47
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole08");// 
        subtitle_cues_delta[i, j, k] = 1.5f;//48.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole09");//
        subtitle_cues_delta[i, j, k] = 3.5f;//52
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole10");// 
        subtitle_cues_delta[i, j, k] = 4.6f;//56.6
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.GAM;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.NEU;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        ar_label_left_texts[0].text = LocalizationManager.instance.GetLocalizedValue("earth");//"EARTH
		ar_label_left_texts[1].text = LocalizationManager.instance.GetLocalizedValue("earth");//"EARTH
		ar_label_left_texts[2].text = LocalizationManager.instance.GetLocalizedValue("blackhole");
		ar_label_left_texts[3].text = "ICECUBE";
		
		ar_label_right_texts[0].text = "ICECUBE";
		ar_label_right_texts[1].text = LocalizationManager.instance.GetLocalizedValue("pluto");//"PLUTO";
		ar_label_right_texts[2].text = LocalizationManager.instance.GetLocalizedValue("milkyway");
		ar_label_right_texts[3].text = LocalizationManager.instance.GetLocalizedValue("sun");

        start_tmp.text = LocalizationManager.instance.GetLocalizedValue("start");
        language_tmp.text = LocalizationManager.instance.GetLocalizedValue("language");

        ar_label_bh_texts[0].text = LocalizationManager.instance.GetLocalizedValue("visible");
		ar_label_bh_texts[1].text = LocalizationManager.instance.GetLocalizedValue("xray");
		ar_label_bh_texts[2].text = LocalizationManager.instance.GetLocalizedValue("neutrino");


        ar_alert.GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("alert");
        ar_maps[0].transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("map");
        ar_maps[1].transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("map01");
        spec_viz_reticle.transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("visible");
        spec_gam_reticle.transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("xray");
        spec_neu_reticle.transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("neutrino");

        //gen absolutes from deltas
        for (i = 0; i < (int)SCENE.COUNT; i++)
            for (j = 0; j < (int)SPEC.COUNT + 1; j++)
                for (k = 1; k < MAX_SUBTITLES_PER_CLIP; k++)
                {
                    subtitle_cues_absolute[i, j, k] = subtitle_cues_absolute[i, j, k - 1] + subtitle_cues_delta[i, j, k - 1];
                }

    }

    //Portuguese
    void UpdatePortugueseCues()
    {
        int i;
        int j;
        int k;

        //prepopulate with defaults
        for (i = 0; i < (int)SCENE.COUNT; i++)
            for (j = 0; j < (int)SPEC.COUNT + 1; j++)
                for (k = 0; k < MAX_SUBTITLES_PER_CLIP; k++)
                {
                    subtitle_strings[i, j, k] = string.Empty;
                    subtitle_cues_delta[i, j, k] = 0.0001f;
                    //subtitle_cues_absolute[i, j, k] = 0.0001f;
                }

        //ICE
        i = (int)SCENE.ICE;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 0.001f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica");
        subtitle_cues_delta[i, j, k] = 1.9f; //2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica01");
        subtitle_cues_delta[i, j, k] = 2.6f; //4.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica02");
        subtitle_cues_delta[i, j, k] = 2.9f;//7.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica03"); 
        subtitle_cues_delta[i, j, k] = 2f;//9.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica04");
        subtitle_cues_delta[i, j, k] = 3.9f;//13.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica05");
        subtitle_cues_delta[i, j, k] = 4.3f;//17.7
        subtitle_pause_i_ice_0 = k;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica06");
        subtitle_cues_delta[i, j, k] = 5.2f;//22.9
        subtitle_pause_i_ice_1 = k;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica07");
        subtitle_cues_delta[i, j, k] = 2.4f;//25.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica08");
        subtitle_cues_delta[i, j, k] = 2.2f;//27.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica09");
        subtitle_cues_delta[i, j, k] = 3.1f;//30.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica10");
        subtitle_cues_delta[i, j, k] = 2.6f;//33.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica11");
        subtitle_cues_delta[i, j, k] = 2.4f;//35.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica12");
        subtitle_cues_delta[i, j, k] = 3.6f;//38.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica13");
        subtitle_cues_delta[i, j, k] = 4.4f;//43
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica14");
        subtitle_cues_delta[i, j, k] = 2.5f;//45.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica15");
        subtitle_cues_delta[i, j, k] = 3.1f;//48.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica16");
        subtitle_cues_delta[i, j, k] = 3.7f;//52.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica17");
        subtitle_cues_delta[i, j, k] = 3.6f;//55.9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica18");
        subtitle_cues_delta[i, j, k] = 2f;//57.9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica19");
        subtitle_cues_delta[i, j, k] = 2.9f;//60.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica20");
        subtitle_cues_delta[i, j, k] = 2f;//62.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica21");
        subtitle_cues_delta[i, j, k] = 2.2f;//65
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica22");
        subtitle_cues_delta[i, j, k] = 2.5f;//68.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica23");
        subtitle_cues_delta[i, j, k] = 1.8f;//70.3 
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica24"); 
        subtitle_cues_delta[i, j, k] = 3.1f;//73.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica25");
        subtitle_cues_delta[i, j, k] = 4.2f;//77.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica26");
        subtitle_cues_delta[i, j, k] = 4.3f;//81.9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica27");
        subtitle_cues_delta[i, j, k] = 1.7f;//83.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica28"); 
        subtitle_cues_delta[i, j, k] = 1.9f;//85.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica29");
        subtitle_cues_delta[i, j, k] = 4.5f;//90
        k++;
        subtitle_cues_delta[i, j, k] = 2f;
        k++;

        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //DONE
        //VOYAGER
        i = (int)SCENE.VOYAGER;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 0f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager");
        subtitle_cues_delta[i, j, k] = 1.5f;// 1.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager01");
        subtitle_cues_delta[i, j, k] = 1.6f;// 3.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager02");
        subtitle_cues_delta[i, j, k] = 2.5f;// 5,6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager03");
        subtitle_cues_delta[i, j, k] = 5.6f;// 11.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager04");
        subtitle_cues_delta[i, j, k] = 2.4f;// 13.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager05");
        subtitle_cues_delta[i, j, k] = 3.2f;// 16.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager06");
        subtitle_cues_delta[i, j, k] = 3.7f;// 20.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager07");
        subtitle_cues_delta[i, j, k] = 2.3f;// 22.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager08");
        subtitle_cues_delta[i, j, k] = 1.9f;// 24.7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager09");
        subtitle_cues_delta[i, j, k] = 3f;// 27.7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager10");
        subtitle_cues_delta[i, j, k] = 3.5f;// 31.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager11");
        subtitle_cues_delta[i, j, k] = 4.2f;//35.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager12");
        subtitle_cues_delta[i, j, k] = 2.3f;//37.7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager13");
        subtitle_cues_delta[i, j, k] = 2.1f;//39.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager14");
        subtitle_cues_delta[i, j, k] = 2.1f;//41.9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager15");
        subtitle_cues_delta[i, j, k] = 1.3f;//43.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager16");
        subtitle_cues_delta[i, j, k] = 3.8f;//47
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //DONE
        j = (int)SPEC.GAM;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 0f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager17");
        subtitle_cues_delta[i, j, k] = 1.4f;//1.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager18");
        subtitle_cues_delta[i, j, k] = 2.4f;//3.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager19");
        subtitle_cues_delta[i, j, k] = 3.9f;//7.7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager20");
        subtitle_cues_delta[i, j, k] = 2.9f;//10.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager21");
        subtitle_cues_delta[i, j, k] = 3.1f;//13.7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager22");
        subtitle_cues_delta[i, j, k] = 4.2f;//17.9
        subtitle_pause_i_voyager_0 = k;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager23");
        subtitle_cues_delta[i, j, k] = 3.3f;//21.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager24");
        subtitle_cues_delta[i, j, k] = 3.1f;//24.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager25");
        subtitle_cues_delta[i, j, k] = 2.8f;//27.8
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //DONE
        j = (int)SPEC.NEU;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 0f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("alright");
        subtitle_cues_delta[i, j, k] = 2.4f;//2.4
        subtitle_pause_i_voyager_1 = k;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino01");
        subtitle_cues_delta[i, j, k] = 1f;//3.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino02");
        subtitle_cues_delta[i, j, k] = 1.8f;//5.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino03");
        subtitle_cues_delta[i, j, k] = 4.7f;//9
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino04");
        subtitle_cues_delta[i, j, k] = 1.7f;//11.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino05");
        subtitle_cues_delta[i, j, k] = 1.5f;//13.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino06");
        subtitle_cues_delta[i, j, k] = 3.2f;//16.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino07");
        subtitle_cues_delta[i, j, k] = 1.7f;//18
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino08");
        subtitle_cues_delta[i, j, k] = 2.5f;//20.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino09");
        subtitle_cues_delta[i, j, k] = 3.2f;//23.7
        k++;
        subtitle_cues_delta[i, j, k] = 0.5f;
        k++;

        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //NOTHING
        //DONE
        i = (int)SCENE.NOTHING;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 0f;//.5 was 1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey");
        subtitle_cues_delta[i, j, k] = 3.3f;//3.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey01");
        subtitle_cues_delta[i, j, k] = 3f;// 6.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey02");
        subtitle_cues_delta[i, j, k] = 3.4f;//10 
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey03");
        subtitle_cues_delta[i, j, k] = 4f;//14
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey04");
        subtitle_cues_delta[i, j, k] = 2.2f;//16.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey05");
        subtitle_cues_delta[i, j, k] = 4f;//20.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey06");
        subtitle_cues_delta[i, j, k] = 4.6f;// 24.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey07");
        subtitle_cues_delta[i, j, k] = 2.4f;// 27.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey08");
        subtitle_cues_delta[i, j, k] = 3.8f;//31
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey09");
        subtitle_cues_delta[i, j, k] = 3.6f;//34.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey10");
        subtitle_cues_delta[i, j, k] = 2.6f;//37.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey11");
        subtitle_cues_delta[i, j, k] = 3.5f;//40.7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey12");
        subtitle_cues_delta[i, j, k] = 3f;//43.7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey13");
        subtitle_cues_delta[i, j, k] = 2.1f;//45.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey14");
        subtitle_cues_delta[i, j, k] = 2.4f;//48.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey15");
        subtitle_cues_delta[i, j, k] = 1.9f;//50.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey16");
        subtitle_cues_delta[i, j, k] = 2.5f;//52.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey17");
        subtitle_cues_delta[i, j, k] = 3f;//55.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey18");
        subtitle_cues_delta[i, j, k] = 2.5f;//58.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey19");
        subtitle_cues_delta[i, j, k] = 3.2f;//61.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey20");
        subtitle_cues_delta[i, j, k] = 2.7f;//64
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey21");
        subtitle_cues_delta[i, j, k] = 2.7f;//66.7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey22");
        subtitle_cues_delta[i, j, k] = 2.1f;//68.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey23");
        subtitle_cues_delta[i, j, k] = 3.3f;//72.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey24");
        subtitle_cues_delta[i, j, k] = 1.1f;//73.2
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //EXTREME
        //DONE
        i = (int)SCENE.EXTREME;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 0f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("hi");
        subtitle_cues_delta[i, j, k] = 1f; //1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("ask");
        subtitle_cues_delta[i, j, k] = 1.4f; //2.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("discover");
        subtitle_cues_delta[i, j, k] = 2f;//4.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("scan");
        subtitle_cues_delta[i, j, k] = 2f;//6.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("scan01");
        subtitle_cues_delta[i, j, k] = 2.2f;//8.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("getout");
        subtitle_cues_delta[i, j, k] = 1.6f;//10.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("light");
        subtitle_cues_delta[i, j, k] = 3.2f;//13.4
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("collect");
        subtitle_cues_delta[i, j, k] = 3.3f;//16.7
        k++;
        subtitle_cues_delta[i, j, k] = 2f;
        k++;

        //DONE
        j = (int)SPEC.GAM;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("collect01");
        subtitle_cues_delta[i, j, k] = 3.9f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //DONE
        j = (int)SPEC.NEU;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("collect02");
        subtitle_cues_delta[i, j, k] = 3f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        //DONE
        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("nice");
        subtitle_cues_delta[i, j, k] = 1.6f; //2.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("path");
        subtitle_cues_delta[i, j, k] = 3.4f; //5.6
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;


        //EARTH
        i = (int)SCENE.EARTH;
        j = (int)SPEC.VIZ;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 0.3f;
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("wow");
        subtitle_cues_delta[i, j, k] = 1.7f;//2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("wow01");
        subtitle_cues_delta[i, j, k] = 2.5f;// 4.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("congrats");
        subtitle_cues_delta[i, j, k] = 2.8f;//7.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("source");
        subtitle_cues_delta[i, j, k] = 4f;//11.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("source01");
        subtitle_cues_delta[i, j, k] = 1.4f;//12.7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("bh");
        subtitle_cues_delta[i, j, k] = 2.4f;//15.1
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole01");
        subtitle_cues_delta[i, j, k] = 2.9f;//18
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole02");
        subtitle_cues_delta[i, j, k] = 2.7f;//20.7
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("sn");
        subtitle_cues_delta[i, j, k] = 2.1f; //22.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole03");
        subtitle_cues_delta[i, j, k] = 2.2f; //25
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole04");// 
        subtitle_cues_delta[i, j, k] = 1.8f; //26.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole05");// 
        subtitle_cues_delta[i, j, k] = 2.5f; //29.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole06");// 
        subtitle_cues_delta[i, j, k] = 2.5f; //31.8
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole07");// 
        subtitle_cues_delta[i, j, k] = 2.3f;//34.5
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole08");// 
        subtitle_cues_delta[i, j, k] = 2.7f; //37.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole09");//
        subtitle_cues_delta[i, j, k] = 3.1f;//40.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole10");// 
        subtitle_cues_delta[i, j, k] = 1.9f; //42.2
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole11");// 
        subtitle_cues_delta[i, j, k] = 3.4f; //45.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("thankyou");// 
        subtitle_cues_delta[i, j, k] = 1f; //46.6
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("thankyou02");// 
        subtitle_cues_delta[i, j, k] = 3.4f; //50
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("end");// 
        subtitle_cues_delta[i, j, k] = 3.3f; //53.3
        k++;
        subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("bye");// 
        subtitle_cues_delta[i, j, k] = 1.1f; // 54.4
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.GAM;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.NEU;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        j = (int)SPEC.COUNT;
        k = 0;
        subtitle_strings[i, j, k] = string.Empty;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;
        subtitle_cues_delta[i, j, k] = 1f;
        k++;

        ar_label_left_texts[0].text = LocalizationManager.instance.GetLocalizedValue("earth");//"EARTH
		ar_label_left_texts[1].text = LocalizationManager.instance.GetLocalizedValue("earth");//"EARTH
		ar_label_left_texts[2].text = LocalizationManager.instance.GetLocalizedValue("blackhole");
		ar_label_left_texts[3].text = "ICECUBE";
		
		ar_label_right_texts[0].text = "ICECUBE";
		ar_label_right_texts[1].text = LocalizationManager.instance.GetLocalizedValue("pluto");//"PLUTO";
		ar_label_right_texts[2].text = LocalizationManager.instance.GetLocalizedValue("milkyway");
		ar_label_right_texts[3].text = LocalizationManager.instance.GetLocalizedValue("sun");

        start_tmp.text = LocalizationManager.instance.GetLocalizedValue("start");
        language_tmp.text = LocalizationManager.instance.GetLocalizedValue("language");

        ar_label_bh_texts[0].text = LocalizationManager.instance.GetLocalizedValue("visible");
		ar_label_bh_texts[1].text = LocalizationManager.instance.GetLocalizedValue("xray");
		ar_label_bh_texts[2].text = LocalizationManager.instance.GetLocalizedValue("neutrino");


        ar_alert.GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("alert");
        ar_maps[0].transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("map");
        ar_maps[1].transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("map01");
        spec_viz_reticle.transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("visible");
        spec_gam_reticle.transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("xray");
        spec_neu_reticle.transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("neutrino");

        //gen absolutes from deltas
        for (i = 0; i < (int)SCENE.COUNT; i++)
            for (j = 0; j < (int)SPEC.COUNT + 1; j++)
                for (k = 1; k < MAX_SUBTITLES_PER_CLIP; k++)
                {
                    subtitle_cues_absolute[i, j, k] = subtitle_cues_absolute[i, j, k - 1] + subtitle_cues_delta[i, j, k - 1];
                }

    }

    void reStart()
    {
		Unpause();
        for (int i = 0; i < (int)SCENE.COUNT; i++)
        {
            for (int j = 0; j < (int)SPEC.COUNT + 1; j++)
            {
                voiceovers_played[i, j] = false;
            }
        }
        voiceover_was_playing = false;
        music_was_playing = false;
        language_selected = false;
        logged_spec_viz = false;
        logged_spec_gam = false;
        logged_spec_neu = false;
		event_player_logged = false;
        lang_menu = false;
        starting = false;
        voiceover_audiosource.Stop();
        espAudio.Stop();
        porAudio.Stop();
        music_audiosource.Stop();
		languageAnalyticsSent = false;

        subtitles_text.text = string.Empty;
        subtitle_i = 0;
        subtitle_t = 0;
        subtitle_spec = (int)SPEC.VIZ;

        grid_yoff = -1000f;
        grid_yvel = 0f;
        grid_yacc = 0f;
        grid.transform.position = new Vector3(grid.transform.position.x, grid_oyoff + grid_yoff, grid.transform.position.z);

        //auto skip these
        voiceovers_played[(int)SCENE.ICE, (int)SPEC.NEU] = true;
        voiceovers_played[(int)SCENE.ICE, (int)SPEC.GAM] = true;
        voiceovers_played[(int)SCENE.NOTHING, (int)SPEC.NEU] = true;
        voiceovers_played[(int)SCENE.NOTHING, (int)SPEC.GAM] = true;
        voiceovers_played[(int)SCENE.EARTH, (int)SPEC.NEU] = true;
        voiceovers_played[(int)SCENE.EARTH, (int)SPEC.GAM] = true;
        voiceovers_played[(int)SCENE.CREDITS, (int)SPEC.NEU] = true;
        voiceovers_played[(int)SCENE.CREDITS, (int)SPEC.GAM] = true;

        for (int i = 0; i < (int)SCENE.COUNT; i++)
            for (int j = 0; j < (int)SPEC.COUNT; j++)
                ta[i, j] = 0;

        for (int i = 0; i < (int)SPEC.COUNT; i++)
            blackhole_spec_triggered[i] = 0;

        for (int i = 0; i < (int)SCENE.COUNT; i++)
            scene_rots[i] = 0f;

        flash_alpha = 0;
        time_mod_twelve_pi = 0;
        jitter = 0;
        jitter_countdown = 0;
        jitter_state = 0;

        next_scene_i = starting_scene;
        cur_scene_i = next_scene_i;
        next_scene_i = (next_scene_i + 1) % ((int)SCENE.COUNT);
        cur_spec_i = 0;

        mouse_captured = false;
        mouse_just_captured = true;
        mouse_x = Screen.width / 2;
        mouse_y = Screen.height / 2;

        in_portal_motion = 0;
        out_portal_motion = 0;
        max_portal_motion = 1;

        in_fail_motion = 0;
        out_fail_motion = 0;
        max_fail_motion = 1;

        dumb_delay_t_max = 3f;
        dumb_delay_t = 0f;

        credits_i = 0;
        credits_t = 0f;

        advance_paused = false;
		//gaze_reticle.SetActive(false);
		cam_reticle.SetActive(true);
		gazeray.SetActive(false);
        gazeball.SetActive(false);
        ar_alert.SetActive(false);
        ar_timer.SetActive(false);
        lang_sel_reticle1.SetActive(false);
        languagesel.SetActive(false);
        startsel.SetActive(false);
        menuscreen.SetActive(true);

        advance_trigger.reset();
        spec_trigger.reset();
        warp_trigger.reset();
        blackhole_trigger.reset();
        language_trigger.reset();
        language1_trigger.reset();
        languagepor_trigger.reset();
        start_trigger.reset();
        menulanguage.reset();

		IceCubeAnalytics.Instance.LogStartGame();
        //Debug.Log("Log start game");
    }

	void Pause()
	{
		Time.timeScale = 0;
		if (voiceover_audiosource.isPlaying || espAudio.isPlaying || porAudio.isPlaying)
		{
			voiceover_audiosource.Pause();
			espAudio.Pause();
			porAudio.Pause();
			music_audiosource.Pause();
		}
		HeadsetPaused = true;
	}

	void Unpause()
	{
		Time.timeScale = 1;
		voiceover_audiosource.UnPause();
		espAudio.UnPause();
		porAudio.UnPause();
		music_audiosource.UnPause();
		HeadsetPaused = false;
	}

	void Start()
	{
		if (ResetOnHMDRemoved)
		{
			OVRManager.HMDMounted += HandleHMDMounted;
			OVRManager.HMDUnmounted += HandleHMDUnmounted;
		}
		else //pause game when headset is off?
		{	//pausing the timeScale breaks things... audio gets out of sync with rest of game
			OVRManager.HMDMounted += Unpause;
			OVRManager.HMDUnmounted += Pause;
		}
		//Application.runInBackground = true;
	
        scene_names = new string[(int)SCENE.COUNT];
        for (int i = 0; i < (int)SCENE.COUNT; i++)
        {
            string name = string.Empty;
            switch (i)
            {
                case (int)SCENE.ICE:
                    name = "Ice";
                    break;
                case (int)SCENE.VOYAGER:
                    name = "Voyager";
                    break;
                case (int)SCENE.NOTHING:
                    name = "Nothing";
                    break;
                case (int)SCENE.EXTREME:
                    name = "Extreme";
                    break;
                case (int)SCENE.EARTH:
                    name = "Earth";
                    break;
                case (int)SCENE.CREDITS:
                    name = "Credits";
                    break;
            }
            scene_names[i] = name;
        }

        spec_names = new string[(int)SPEC.COUNT];
        for (int i = 0; i < (int)SPEC.COUNT; i++)
        {
            string name = string.Empty;
            switch (i)
            {
                case (int)SPEC.VIZ:
                    name = "Viz";
                    break;
                case (int)SPEC.GAM:
                    name = "Gam";
                    break;
                case (int)SPEC.NEU:
                    name = "Neu";
                    break;
            }
            spec_names[i] = name;
        }

        layer_names = new string[(int)SCENE.COUNT, (int)SPEC.COUNT];
        for (int i = 0; i < (int)SCENE.COUNT; i++)
            for (int j = 0; j < (int)SPEC.COUNT; j++)
                layer_names[i, j] = "Set_" + scene_names[i] + "_" + spec_names[j];

        scene_groups = new GameObject[(int)SCENE.COUNT, (int)SPEC.COUNT];
        for (int i = 0; i < (int)SCENE.COUNT; i++)
            for (int j = 0; j < (int)SPEC.COUNT; j++)
                scene_groups[i, j] = GameObject.Find(layer_names[i, j]);

        layers = new int[(int)SCENE.COUNT, (int)SPEC.COUNT];
        for (int i = 0; i < (int)SCENE.COUNT; i++)
            for (int j = 0; j < (int)SPEC.COUNT; j++)
                layers[i, j] = LayerMask.NameToLayer(layer_names[i, j]);

        voiceover_files = new string[(int)SCENE.COUNT, (int)SPEC.COUNT + 1];
        engVoiceover_files = new string[(int)SCENE.COUNT, (int)SPEC.COUNT + 1];
        espVoiceover_files = new string[(int)SCENE.COUNT, (int)SPEC.COUNT + 1];
        porVoiceover_files = new string[(int)SCENE.COUNT, (int)SPEC.COUNT + 1];

        for (int i = 0; i < (int)SCENE.COUNT; i++)
        {
            for (int j = 0; j < (int)SPEC.COUNT + 1; j++)
            {
                if (j == (int)SPEC.COUNT)
                {
                    voiceover_files[i, j] = "Voiceover/End/" + scene_names[i];
                    espVoiceover_files[i, j] = "Voiceover/Esp/End/" + scene_names[i];
                    porVoiceover_files[i, j] = "Voiceover/Por/End/" + scene_names[i];
                }
                else
                {
                    voiceover_files[i, j] = "Voiceover/" + spec_names[j] + "/" + scene_names[i];
                    espVoiceover_files[i, j] = "Voiceover/Esp/" + spec_names[j] + "/" + scene_names[i];
                    porVoiceover_files[i, j] = "Voiceover/Por/" + spec_names[j] + "/" + scene_names[i];
                }
            }
        }

        voiceovers = new AudioClip[(int)SCENE.COUNT, (int)SPEC.COUNT + 1];
        porVoiceovers = new AudioClip[(int)SCENE.COUNT, (int)SPEC.COUNT + 1];
        espVoiceovers = new AudioClip[(int)SCENE.COUNT, (int)SPEC.COUNT + 1];
		voiceoversLoaded = new bool[voiceovers.GetLength(0), voiceovers.GetLength(1)];
		porVoiceoversLoaded = new bool[porVoiceovers.GetLength(0), porVoiceovers.GetLength(1)];
		espVoiceoversLoaded = new bool[espVoiceovers.GetLength(0), espVoiceovers.GetLength(1)];
		subtitle_strings = new string[(int)SCENE.COUNT, (int)SPEC.COUNT + 1, MAX_SUBTITLES_PER_CLIP];
        subtitle_cues_delta = new float[(int)SCENE.COUNT, (int)SPEC.COUNT + 1, MAX_SUBTITLES_PER_CLIP];
        subtitle_cues_absolute = new float[(int)SCENE.COUNT, (int)SPEC.COUNT + 1, MAX_SUBTITLES_PER_CLIP];
        subtitle_i = 0;
        subtitle_t = 0;
        subtitle_spec = 0;
        voiceovers_played = new bool[(int)SCENE.COUNT, (int)SPEC.COUNT + 1];
        voiceover_vols = new float[(int)SCENE.COUNT, (int)SPEC.COUNT + 1];

        voiceover_volsON = new float[(int)SCENE.COUNT, (int)SPEC.COUNT + 1];
        voiceover_volsOFF = new float[(int)SCENE.COUNT, (int)SPEC.COUNT + 1];

        for (int i = 0; i < (int)SCENE.COUNT; i++)
        {
            for (int j = 0; j < (int)SPEC.COUNT + 1; j++)
            {
				//load clips as they are needed, to avoid giant stutter at start
				//voiceovers[i, j] = Resources.Load<AudioClip>(voiceover_files[i, j]);
				//porVoiceovers[i, j] = Resources.Load<AudioClip>(porVoiceover_files[i, j]);
				//espVoiceovers[i, j] = Resources.Load<AudioClip>(espVoiceover_files[i, j]);
				voiceoversLoaded[i, j] = false;
				porVoiceoversLoaded[i, j] = false;
				espVoiceoversLoaded[i, j] = false;

				voiceover_vols[i, j] = 1.0f;
            }
        }

        //manually fill out subtitles
        {
            int i;
            int j;
            int k;
            //prepopulate with defaults
            for (i = 0; i < (int)SCENE.COUNT; i++)
                for (j = 0; j < (int)SPEC.COUNT + 1; j++)
                    for (k = 0; k < MAX_SUBTITLES_PER_CLIP; k++)
                    {
                        subtitle_strings[i, j, k] = string.Empty;
                        subtitle_cues_delta[i, j, k] = 0.0001f;
                        subtitle_cues_absolute[i, j, k] = 0.0001f;
                    }

            //populating subtitiles
            i = (int)SCENE.ICE;
            j = (int)SPEC.VIZ;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 0.001f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica");//"Hey! Come in!"; 
            subtitle_cues_delta[i, j, k] = 1f; //1
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica01");//"... Hello? ..."; 
            subtitle_cues_delta[i, j, k] = 1f; //2
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica02");//"Sorry, we're still getting the kinks worked out of this new suit."; 
            subtitle_cues_delta[i, j, k] = 2.9f;//4
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica03");//"Let me know if this is working-"; 
            subtitle_cues_delta[i, j, k] = 1.5f;//6
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica04");//"I'm booting up the augmented reality overlay in your helmet now..."; 
            subtitle_cues_delta[i, j, k] = 4f;//10
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica05");//"Ok. Can you look up at the gaze point for me?";
            subtitle_cues_delta[i, j, k] = 2.5f;//13
            subtitle_pause_i_ice_0 = k;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica06");//"Great! Now look at the one at your feet.";
            subtitle_cues_delta[i, j, k] = 2f;//15
            subtitle_pause_i_ice_1 = k;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica07");//"Alright! Everything seems to be in order.";
            subtitle_cues_delta[i, j, k] = 2.25f;//16
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica08");//"Welcome to Ice Cube!";
            subtitle_cues_delta[i, j, k] = 2f;//18
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica09");//"I'm glad you could make it all the way";
            subtitle_cues_delta[i, j, k] = 1.5f;//20
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica10");//"down to antarctica for this mission.";
            subtitle_cues_delta[i, j, k] = 1.5f;//22
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica11");//"Before we send you off,";
            subtitle_cues_delta[i, j, k] = 2f;//24
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica12");//"let's brief you on why you're here:";
            subtitle_cues_delta[i, j, k] = 2f;//25
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica13");//"The Ice Cube Research Facility detects neutrino particles";
            subtitle_cues_delta[i, j, k] = 2.75f;//29
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica14");//"sent from deep out in space.";
            subtitle_cues_delta[i, j, k] = 1.2f;//30
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica15");// "I'll show you the sensors on your helmet overlay.";
            subtitle_cues_delta[i, j, k] = 2f;//32
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica16");//"See that grid below the facility?";
            subtitle_cues_delta[i, j, k] = 2f;//35
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica17");// "Each dot is a sensor that detects light";
            subtitle_cues_delta[i, j, k] = 3f;//38
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica18");// "from a passing neutrino particle.";
            subtitle_cues_delta[i, j, k] = 2f;//39
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica19");//"Look! It's just detected one now!";
            subtitle_cues_delta[i, j, k] = 3f;//42
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica20");// "This is great timing-";
            subtitle_cues_delta[i, j, k] = 2f;//44
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica21");// "we'll use the sensor data";
            subtitle_cues_delta[i, j, k] = 1f;//45
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica22");// "to pinpoint where this came from in outer space...";
            subtitle_cues_delta[i, j, k] = 2f;//47
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica23");// "annnnnd... done!";
            subtitle_cues_delta[i, j, k] = 2f;//49
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica24");// "Now your job is to follow this trajectory";
            subtitle_cues_delta[i, j, k] = 3f;//52
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica25");// "out into space to find the source";
            subtitle_cues_delta[i, j, k] = 2f;//54
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica26");// "You're going to use your suit's Impossible Drive";
            subtitle_cues_delta[i, j, k] = 3f;//57
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica27");// "and follow the path of the neutrino-";
            subtitle_cues_delta[i, j, k] = 2f;//59
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica28");// "All you have do to is";
            subtitle_cues_delta[i, j, k] = 1f;//60
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("antarctica29");// "look at the gaze point at the end of the path...";
            subtitle_cues_delta[i, j, k] = 2f;//62
            subtitle_pause_i_ice_2 = k;
            k++;
            subtitle_cues_delta[i, j, k] = 2f;
            k++;

            j = (int)SPEC.COUNT;
            k = 0;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            //VOYAGER
            i = (int)SCENE.VOYAGER;
            j = (int)SPEC.VIZ;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager");// "Hello? ...";
            subtitle_cues_delta[i, j, k] = 0.5f;//1
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager01");// "You still there?";
            subtitle_cues_delta[i, j, k] = 0.5f;//1
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager02");// "Did you make it in one piece?";
            subtitle_cues_delta[i, j, k] = 1f;//2
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager03");// "Take a second to look around and find your bearings-";
            subtitle_cues_delta[i, j, k] = 3f;//5
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager04");// "it's probably pretty cool to be";
            subtitle_cues_delta[i, j, k] = 2f;//7
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager05");// "further out in space than any other human has ever been!";
            subtitle_cues_delta[i, j, k] = 2f;//9
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager06");// "Now you have a job to do.";
            subtitle_cues_delta[i, j, k] = 2f;//11
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager07");// "Follow the path of the neutrino";
            subtitle_cues_delta[i, j, k] = 2f;//13
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager08");// "that was detected by Ice Cube";
            subtitle_cues_delta[i, j, k] = 1f;//14
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager09");// "to discover the source.";
            subtitle_cues_delta[i, j, k] = 2f;//16
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager10");// "While we're waiting for the Impossible Drive to recharge,";
            subtitle_cues_delta[i, j, k] = 3f;//19
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager11");// "let's go over some other features of your suit.";
            subtitle_cues_delta[i, j, k] = 2f;//21
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager12");// "If you look at your feet,";
            subtitle_cues_delta[i, j, k] = 2f;//23
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager13");// "you can use the gaze points to";
            subtitle_cues_delta[i, j, k] = 1f;//24
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager14");// "switch out your helmet's view.";
            subtitle_cues_delta[i, j, k] = 2f;//26
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager15");// "Go ahead-";
            subtitle_cues_delta[i, j, k] = 1f;//27
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager16");// "look at your feet and switch to X-ray view.";
            subtitle_cues_delta[i, j, k] = 3f;//30
            k++;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            j = (int)SPEC.GAM;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager17");// "Pretty great, right?";
            subtitle_cues_delta[i, j, k] = 2f;//2
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager18");// "Look around- check out the galaxy!";
            subtitle_cues_delta[i, j, k] = 2f;//4
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager19");// "This is what the universe looks like when we see with X-rays.";
            subtitle_cues_delta[i, j, k] = 4f;//8
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager20");// "Your helmet is detecting X-rays";
            subtitle_cues_delta[i, j, k] = 2f;//10
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager21");// "in the same way your eye would normally detect light.";
            subtitle_cues_delta[i, j, k] = 2f;//12
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager22");// "Can you look at Pluto for a second?";
            subtitle_cues_delta[i, j, k] = 2f;//14
            subtitle_pause_i_voyager_0 = k;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager23");// "See how it's just a big black ball?";
            subtitle_cues_delta[i, j, k] = 2f;//16
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager24");// "That's because X-rays don't pass through it.";
            subtitle_cues_delta[i, j, k] = 3f;//19
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("voyager25");// "Now, let's switch to neutrino vision.";
            subtitle_cues_delta[i, j, k] = 2f;//21
            k++;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            j = (int)SPEC.NEU;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("alright");// "Alright- look back to Pluto:";
            subtitle_cues_delta[i, j, k] = 2f;//2
            subtitle_pause_i_voyager_1 = k;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino01");// "where'd it go?!";
            subtitle_cues_delta[i, j, k] = 1f;//3
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino02");// "Pluto seems to have disappeared!";
            subtitle_cues_delta[i, j, k] = 1f;//4
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino03");// "Your helmet is now only sensing neutrino particles.";
            subtitle_cues_delta[i, j, k] = 3f;//7
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino04");// "Neutrinos pass through just about everything,";
            subtitle_cues_delta[i, j, k] = 3f;//10
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino05");// "Even whole planets!";
            subtitle_cues_delta[i, j, k] = 1f;//11
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino06");// "It's like Pluto doesn't even exist to them!";
            subtitle_cues_delta[i, j, k] = 3f;//13
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino07");// "When you're ready,";
            subtitle_cues_delta[i, j, k] = 0.5f;//15
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("neutrino08");// "Look to the gaze point at the end of the neutrino path.";
            subtitle_cues_delta[i, j, k] = 2.5f;//18
            subtitle_pause_i_voyager_2 = k;
            k++;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            j = (int)SPEC.COUNT;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            //NOTHING
            i = (int)SCENE.NOTHING;
            j = (int)SPEC.VIZ;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey");// "We're getting some... pretty intense readings.";
            subtitle_cues_delta[i, j, k] = 2f;//2
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey01");// "You're... really far out in space.";
            subtitle_cues_delta[i, j, k] = 3f;//5
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey02");// "Ok- time to brief you with the details of your mission.";
            subtitle_cues_delta[i, j, k] = 3f;//8
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey03");// "As you've seen, we've given your suit the ability";
            subtitle_cues_delta[i, j, k] = 3f;//11
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey04");// "to see in three different ways:";
            subtitle_cues_delta[i, j, k] = 1f;//12
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey05");// "Visible light, X-ray vision, and neutrino detection.";
            subtitle_cues_delta[i, j, k] = 4f;//16
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey06");// "The first two have been used for decades to look out into space.";
            subtitle_cues_delta[i, j, k] = 4f;//20
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey07");// "But if we want to see really far,";
            subtitle_cues_delta[i, j, k] = 2f;//22
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey08");// "Neutrinos are the only thing that will work.";
            subtitle_cues_delta[i, j, k] = 2f;//24
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey09");// "That's why we need the Ice Cube observatory.";
            subtitle_cues_delta[i, j, k] = 3f;//27
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey10");// "The arrays of sensors in antartica";
            subtitle_cues_delta[i, j, k] = 3f;//30
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey11");// "allow us to detect neutrinos from deep space.";
            subtitle_cues_delta[i, j, k] = 2f;//32
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey12");// "That helps us map out parts of the universe";
            subtitle_cues_delta[i, j, k] = 3f;//35
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey13");// "invisible to other telescopes.";
            subtitle_cues_delta[i, j, k] = 1f;//36
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey14");// "The question you have to answer is:";
            subtitle_cues_delta[i, j, k] = 3f;//39
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey15");// "What sent the neutrino that";
            subtitle_cues_delta[i, j, k] = 2f;//41
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey16");// "Ice Cube detected back at Earth?";
            subtitle_cues_delta[i, j, k] = 2f;//43
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey17");// "When you find the source at the end of your journey,";
            subtitle_cues_delta[i, j, k] = 3f;//46
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey18");// "you'll need to collect data from it";
            subtitle_cues_delta[i, j, k] = 1f;//47
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey19");// "using each of the three methods we've given you.";
            subtitle_cues_delta[i, j, k] = 2f;//49
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey20");// "You'll use your visible light,";
            subtitle_cues_delta[i, j, k] = 2f;//51
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey21");// "X-ray, and neutrino view to collect these readings.";
            subtitle_cues_delta[i, j, k] = 4f;//55
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey22");// "Ok. Things might get dicey going forward.";
            subtitle_cues_delta[i, j, k] = 4f;//59
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("journey23");// "Good luck.";
            subtitle_cues_delta[i, j, k] = 3f;//61
            k++;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            j = (int)SPEC.COUNT;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            //EXTREME
            i = (int)SCENE.EXTREME;
            j = (int)SPEC.VIZ;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("hi");// "Hello?";
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("ask");// "Do you read me?";
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("discover");// "You've discovered a black hole!";
            subtitle_cues_delta[i, j, k] = 3f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("scan");// "You need to scan it with each";
            subtitle_cues_delta[i, j, k] = 2f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("scan01");// "of your vision modules quickly!";
            subtitle_cues_delta[i, j, k] = 2f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("getout");// "Then get OUT of there!";
            subtitle_cues_delta[i, j, k] = 3f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("light");// "Make sure you've selected visibile light vision-";
            subtitle_cues_delta[i, j, k] = 2f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("collect");// "look at the black hole and collect visible light readings!";
            subtitle_cues_delta[i, j, k] = 4f;
            k++;
            subtitle_cues_delta[i, j, k] = 2f;
            k++;

            j = (int)SPEC.GAM;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("collect01");// "Look up at the black hole, and collect the X-ray readings!";
            subtitle_cues_delta[i, j, k] = 4f;
            k++;
            subtitle_cues_delta[i, j, k] = 2f;
            k++;

            j = (int)SPEC.NEU;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("collect02");// "Look back at the black hole, and collect the neutrino readings!";
            subtitle_cues_delta[i, j, k] = 4f;
            k++;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            j = (int)SPEC.COUNT;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("nice");// "You did it! We have the data!";
            subtitle_cues_delta[i, j, k] = 3f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("path");// "Now follow the neutrino path back to Earth!";
            subtitle_cues_delta[i, j, k] = 3f;
            k++;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            //EARTH
            i = (int)SCENE.EARTH;
            j = (int)SPEC.VIZ;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("wow");// "Wow! You did it!";
            subtitle_cues_delta[i, j, k] = 2f;//2
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("wow01");// "I... can't believe you're alive!";
            subtitle_cues_delta[i, j, k] = 0.5f;//2
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("congrats");//  "....I mean, congratulations, agent!";
            subtitle_cues_delta[i, j, k] = 1.5f;//4
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("source");// "It looks like the source";
            subtitle_cues_delta[i, j, k] = 2f;//6
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("source01");// "of the neutrino particle we detected with Ice Cube";
            subtitle_cues_delta[i, j, k] = 3f;//9
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("source02");// "was a black hole!";
            subtitle_cues_delta[i, j, k] = 1f;//10
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("bh");// "Black holes are one of the strangest,";
            subtitle_cues_delta[i, j, k] = 2f;//12
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole01");// "most extreme objects in the whole universe!";
            subtitle_cues_delta[i, j, k] = 2f;//14
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole02");// "Did you know that black holes can have the mass";
            subtitle_cues_delta[i, j, k] = 3f;//17
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("sn");// "of several million suns?";
            subtitle_cues_delta[i, j, k] = 2f;//19
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole03");// "One spoonful of black hole could weigh as much";
            subtitle_cues_delta[i, j, k] = 3f;//22
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole04");// "as a whole planet!";
            subtitle_cues_delta[i, j, k] = 1f;//23
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole05");// "They also emit high energy neutrinos";
            subtitle_cues_delta[i, j, k] = 3f;//26
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole06");// "that travel millions of lightyears back to Earth.";
            subtitle_cues_delta[i, j, k] = 2f;//28
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole07");// "It would have gone totally unnoticed";
            subtitle_cues_delta[i, j, k] = 3f;//31
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole08");// "if it weren't for the scientists at Ice Cube.";
            subtitle_cues_delta[i, j, k] = 2f;//33
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole09");// "Black holes are very hard to detect-";
            subtitle_cues_delta[i, j, k] = 3f;//36
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole10");// "well- because they're black!";
            subtitle_cues_delta[i, j, k] = 1f;//37
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("blackhole11");// "It's impossible to see something black on a black background of space!";
            subtitle_cues_delta[i, j, k] = 4f;//41
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("thankyou");// "Fortunately, IceCube has found a way to observe them using neutrinos!";
            subtitle_cues_delta[i, j, k] = 4f;//45
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("end");// "Well, that's mission complete on our end.";
            subtitle_cues_delta[i, j, k] = 4f;//49
            k++;
            subtitle_strings[i, j, k] = LocalizationManager.instance.GetLocalizedValue("bye");// "Until next time!";
            subtitle_cues_delta[i, j, k] = 1f;//50
            k++;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            j = (int)SPEC.GAM;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            j = (int)SPEC.NEU;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            j = (int)SPEC.COUNT;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            //CREDITS
            i = (int)SCENE.CREDITS;
            j = (int)SPEC.VIZ;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            j = (int)SPEC.COUNT;
            k = 0;
            subtitle_strings[i, j, k] = string.Empty;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;
            subtitle_cues_delta[i, j, k] = 1f;
            k++;

            //gen absolutes from deltas
            for (i = 0; i < (int)SCENE.COUNT; i++)
                for (j = 0; j < (int)SPEC.COUNT + 1; j++)
                    for (k = 1; k < MAX_SUBTITLES_PER_CLIP; k++)
                    {
                        subtitle_cues_absolute[i, j, k] = subtitle_cues_absolute[i, j, k - 1] + subtitle_cues_delta[i, j, k - 1];
                    }
        }

  //      music_files = new string[(int)SCENE.COUNT, (int)SPEC.COUNT];
  //      for (int i = 0; i < (int)SCENE.COUNT; i++)
  //      {
  //          for (int j = 0; j < (int)SPEC.COUNT; j++)
  //          {
  //              music_files[i, j] = "Music/" + spec_names[j] + "/" + scene_names[i];
  //          }
  //      }
  //      musics = new AudioClip[(int)SCENE.COUNT, (int)SPEC.COUNT];
		//musics_loaded = new bool[musics.GetLength(0), musics.GetLength(1)];
        music_vols = new float[(int)SCENE.COUNT, (int)SPEC.COUNT];

        for (int i = 0; i < (int)SCENE.COUNT; i++)
        {
            for (int j = 0; j < (int)SPEC.COUNT; j++)
            {   //load as needed
				//musics[i, j] = Resources.Load<AudioClip>(music_files[i, j]);
				//musics_loaded[i, j] = false;
                music_vols[i, j] = 1.0f;
            }
        }

        n_sfx_audiosources = 5;
        sfx_audiosource_i = 0;
        sfx_audiosource = new AudioSource[n_sfx_audiosources];
        for (int i = 0; i < n_sfx_audiosources; i++)
        {
            sfx_audiosource[i] = GameObject.Find("Script").AddComponent<AudioSource>();
            sfx_audiosource[i].priority = 3;
        }
        sfxs = new AudioClip[(int)SFX.COUNT];
		sfxs_loaded = new bool[sfxs.Length];
        sfx_vols = new float[(int)SFX.COUNT];
        for (int i = 0; i < (int)SFX.COUNT; i++)
        {   //load later
			//sfxs[i] = Resources.Load<AudioClip>(sfx_files[i]);
			sfxs_loaded[i] = false;
            sfx_vols[i] = 1.0f;
        }

        string[,] skybox_files = new string[(int)SCENE.COUNT, (int)SPEC.COUNT];
        for (int i = 0; i < (int)SCENE.COUNT; i++)
            for (int j = 0; j < (int)SPEC.COUNT; j++)
                skybox_files[i, j] = "Skybox/" + spec_names[j] + "/" + scene_names[i] + "/" + scene_names[i];

        helmet_colors = new Color[(int)SCENE.COUNT];
        helmet_colors[0] = scene0_helmet_color;
        helmet_colors[1] = scene1_helmet_color;
        helmet_colors[2] = scene2_helmet_color;
        helmet_colors[3] = scene3_helmet_color;
        helmet_colors[4] = scene4_helmet_color;

        ta = new float[(int)SCENE.COUNT, (int)SPEC.COUNT];

        blackhole_spec_triggered = new int[(int)SPEC.COUNT];
        logged_spec_gam = false;
        logged_spec_neu = false;

        advance_trigger = new gaze_trigger();
        spec_trigger = new gaze_trigger();
        warp_trigger = new gaze_trigger();
        warp_trigger.t_max_numb = 4f; 
        blackhole_trigger = new gaze_trigger();
        blackhole_trigger.range = 0.8f;
        language_trigger = new gaze_trigger();
        language1_trigger = new gaze_trigger();
        languagepor_trigger = new gaze_trigger();
        start_trigger = new gaze_trigger();
        menulanguage = new gaze_trigger();
        default_layer = LayerMask.NameToLayer("Default");

        camera_house = GameObject.Find("CameraHouse");
		ovr_manager = camera_house.GetComponentInChildren<OVRManager>();
        //main_camera = GameObject.Find("Main Camera");
        main_camera = GameObject.Find("CenterEyeAnchor");
        main_camera_skybox = main_camera.GetComponent<Skybox>();
        portal_projection = GameObject.Find("Portal_Projection");
        portal = GameObject.Find("Portal");
        portal_camera_next = GameObject.Find("Portal_Camera_Next");
        portal_camera_next_skybox = portal_camera_next.GetComponent<Skybox>();
        helmet = GameObject.Find("Helmet");
        helmet_light = GameObject.Find("Helmet_Light");
        helmet_light_light = helmet_light.GetComponent<Light>();
        cam_reticle = GameObject.Find("Cam_Reticle");
        cam_spinner = GameObject.Find("Cam_Spinner");
        reticle_d = cam_reticle.transform.position.z;
        gaze_projection = GameObject.Find("Gaze_Projection");
        gaze_reticle = GameObject.Find("Gaze_Reticle");
        arrow = GameObject.Find("arrow");
        spec_projection = GameObject.Find("Spec_Projection");
        spec_viz_reticle = GameObject.Find("Spec_Viz_Reticle");
        spec_gam_reticle = GameObject.Find("Spec_Gam_Reticle");
        spec_neu_reticle = GameObject.Find("Spec_Neu_Reticle");
        spec_sel_reticle = GameObject.Find("Spec_Sel_Reticle");
        spec_viz_reticle.transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("visible");
        spec_gam_reticle.transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("xray");
        spec_neu_reticle.transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("neutrino");
        lang_esp_reticle = GameObject.Find("Lang_Esp_Reticle");
        lang_eng_reticle = GameObject.Find("Lang_Eng_Reticle");
        lang_sel_reticle = GameObject.Find("Lang_Sel_Reticle");

        lang_esp_reticle1 = GameObject.Find("Lang_Esp_Reticle1");
        lang_eng_reticle1 = GameObject.Find("Lang_Eng_Reticle1");
        lang_por_reticle1 = GameObject.Find("Lang_Por_Reticle1");
        lang_sel_reticle1 = GameObject.Find("Lang_Sel_Reticle1");

        languagesel = GameObject.Find("LanguageSel");
        startsel = GameObject.Find("startsel");
        menuscreen = GameObject.Find("Menu"); 
        languageop = GameObject.Find("lang");
        startbutton = GameObject.Find("st");
        language = GameObject.Find("Language");

        gazeray = GameObject.Find("Ray");
        gazeball = GameObject.Find("Ball");
		//grid = GameObject.Find("MyGrid");
		grid = GameObject.Find("DomArray");
        event_player_logged = false;
		grid_eventPlayer = grid.GetComponent<EventPlayer>();
		grid_oyoff = grid.transform.position.y;
        grid.transform.position = new Vector3(grid.transform.position.x, grid_oyoff + grid_yoff, grid.transform.position.z);
        ar_group = GameObject.Find("AR");
        ar_camera_project = GameObject.Find("AR_Camera_Project");
        ar_camera_static = GameObject.Find("AR_Camera_Static");
        ar_maps = new GameObject[3];
        ar_maps[0] = GameObject.Find("map0");
        ar_maps[1] = GameObject.Find("map1");
        ar_maps[2] = GameObject.Find("map2");
        ar_maps[0].transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("map");
        ar_maps[1].transform.GetChild(0).GetComponent<TextMesh>().text = LocalizationManager.instance.GetLocalizedValue("map01");
        ar_alert = GameObject.Find("Alert");
        ar_timer = GameObject.Find("Timer");
        ar_timer_text = ar_timer.GetComponent<TextMesh>();
        credits_0 = GameObject.Find("Credits_0");
        credits_text_0 = credits_0.GetComponent<TextMesh>();
        credits_1 = GameObject.Find("Credits_1");
        credits_text_1 = credits_1.GetComponent<TextMesh>();
        //subtitles = GameObject.Find("Subtitles");
        //subtitles_text = subtitles.GetComponent<TextMesh>();
		subtitles = GameObject.Find("SubtitlesPro");
		subtitles_text = subtitles.GetComponent<TMPro.TextMeshPro>();
        start_text = GameObject.Find("start_tmp");
        start_tmp = start_text.GetComponent<TMPro.TextMeshPro>();
        lang_text = GameObject.Find("language_tmp");
        language_tmp = lang_text.GetComponent<TMPro.TextMeshPro>();

        dropdown = GameObject.Find("Dropdown");
        //stars = GameObject.Find("Stars");
        //starsscale = GameObject.Find("StarsScale");

        ar_label_lefts = new GameObject[MAX_LABELS];
        ar_label_left_kids = new GameObject[MAX_LABELS];
        ar_label_left_quads = new GameObject[MAX_LABELS];
        ar_label_left_texts = new TextMesh[MAX_LABELS];
        ar_label_rights = new GameObject[MAX_LABELS];
        ar_label_right_kids = new GameObject[MAX_LABELS];
        ar_label_right_quads = new GameObject[MAX_LABELS];
        ar_label_right_texts = new TextMesh[MAX_LABELS];
        ar_label_bhs = new GameObject[MAX_LABELS];
        ar_label_bh_kids = new GameObject[MAX_LABELS];
        ar_label_bh_quads = new GameObject[MAX_LABELS];
        ar_label_bh_texts = new TextMesh[MAX_LABELS];

        //technically isn't connected to max labels, but as there's a label per line, it's at least upper bound
        ar_label_checks = new GameObject[MAX_LABELS];
        ar_spec_checks = new GameObject[MAX_LABELS];
        ar_progresses = new GameObject[MAX_LABELS];
        ar_progress_offsets = new GameObject[MAX_LABELS];
        ar_progress_lines = new LineRenderer[MAX_LABELS];

        float lw;
        AnimationCurve curve;
        lw = 0.0001f;
        curve = new AnimationCurve();
        curve.AddKey(0, lw);
        curve.AddKey(1, lw);
        for (int i = 0; i < MAX_LABELS; i++)
        {
            ar_label_lefts[i] = (GameObject)Instantiate(ar_label_left_prefab);
            ar_label_lefts[i].transform.parent = ar_group.transform;
            int k;
            k = 0;
            foreach (Transform child_transform in ar_label_lefts[i].transform)
            {
                GameObject child = child_transform.gameObject;
                switch (k)
                {
                    case 0: ar_label_left_kids[i] = child; break;
                }
                k++;
            }
            k = 0;
            foreach (Transform child_transform in ar_label_left_kids[i].transform)
            {
                GameObject child = child_transform.gameObject;
                switch (k)
                {
                    case 0: ar_label_left_quads[i] = child; break;
                    case 1: ar_label_left_texts[i] = child.GetComponent<TextMesh>(); break;
                }
                k++;
            }

            ar_label_rights[i] = (GameObject)Instantiate(ar_label_right_prefab);
            ar_label_rights[i].transform.parent = ar_group.transform;
            k = 0;
            foreach (Transform child_transform in ar_label_rights[i].transform)
            {
                GameObject child = child_transform.gameObject;
                switch (k)
                {
                    case 0: ar_label_right_kids[i] = child; break;
                }
                k++;
            }
            k = 0;
            foreach (Transform child_transform in ar_label_right_kids[i].transform)
            {
                GameObject child = child_transform.gameObject;
                switch (k)
                {
                    case 0: ar_label_right_quads[i] = child; break;
                    case 1: ar_label_right_texts[i] = child.GetComponent<TextMesh>(); break;
                }
                k++;
            }

            ar_label_bhs[i] = (GameObject)Instantiate(ar_label_bh_prefab);
            ar_label_bhs[i].transform.parent = ar_group.transform;
            k = 0;
            foreach (Transform child_transform in ar_label_bhs[i].transform)
            {
                GameObject child = child_transform.gameObject;
                switch (k)
                {
                    case 0: ar_label_bh_kids[i] = child; break;
                }
                k++;
            }
            k = 0;
            foreach (Transform child_transform in ar_label_bh_kids[i].transform)
            {
                GameObject child = child_transform.gameObject;
                switch (k)
                {
                    case 0: ar_label_bh_quads[i] = child; break;
                    case 1: ar_label_bh_texts[i] = child.GetComponent<TextMesh>(); break;
                }
                k++;
            }

            ar_label_checks[i] = (GameObject)Instantiate(ar_check_prefab);
            ar_label_checks[i].transform.parent = ar_label_bh_kids[i].transform;
            ar_spec_checks[i] = (GameObject)Instantiate(ar_check_prefab); //just to ensure full instantiate- actually set details below (unrelated to labels)

            ar_progress_offsets[i] = (GameObject)Instantiate(ar_progress_prefab);
            ar_progress_offsets[i].transform.parent = ar_group.transform;
            ar_progresses[i] = ar_progress_offsets[i].transform.GetChild(0).gameObject;
            ar_progress_lines[i] = ar_progresses[i].GetComponent<LineRenderer>();

            ar_progress_lines[i].widthCurve = curve;
            for (int j = 0; j < 2; j++)
                ar_progress_lines[i].SetPosition(j, new Vector3(0, 0, 0));
        }

        ar_spec_checks[0].transform.parent = spec_viz_reticle.transform;
        ar_spec_checks[1].transform.parent = spec_gam_reticle.transform;
        ar_spec_checks[2].transform.parent = spec_neu_reticle.transform;
        for (int i = 0; i < 3; i++)
        {
            ar_spec_checks[i].transform.localPosition = new Vector3(0.0f, -1.0f, 0.0f);
            ar_spec_checks[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        ar_alert.SetActive(false);
        ar_timer.SetActive(false);

        icecube = new GameObject[(int)SPEC.COUNT];
        pluto = new GameObject[(int)SPEC.COUNT];
        vearth = new GameObject[(int)SPEC.COUNT];
        milky = new GameObject[(int)SPEC.COUNT];
        nearth = new GameObject[(int)SPEC.COUNT];
        blackhole = new GameObject[(int)SPEC.COUNT];
        esun = new GameObject[(int)SPEC.COUNT];
        earth = new GameObject[(int)SPEC.COUNT];
        for (int i = 0; i < (int)SPEC.COUNT; i++)
        {
            icecube[i] = GameObject.Find("Icecube_" + spec_names[i]);
            pluto[i] = GameObject.Find("Pluto_" + spec_names[i]);
            vearth[i] = GameObject.Find("VEarth_" + spec_names[i]);
            milky[i] = GameObject.Find("Milky_" + spec_names[i]);
            nearth[i] = GameObject.Find("NEarth_" + spec_names[i]);
            blackhole[i] = GameObject.Find("BlackHole_" + spec_names[i]);
            esun[i] = GameObject.Find("ESun_" + spec_names[i]);
            earth[i] = GameObject.Find("Earth_" + spec_names[i]);
        }

        alpha_id = Shader.PropertyToID("alpha");
        time_mod_twelve_pi_id = Shader.PropertyToID("time_mod_twelve_pi");
        jitter_id = Shader.PropertyToID("jitter");

        voiceover_audiosource = GameObject.Find("Script").AddComponent<AudioSource>();
        espAudio = GameObject.Find("Script").AddComponent<AudioSource>();
        porAudio = GameObject.Find("Script").AddComponent<AudioSource>();

        voiceover_audiosource.priority = 1;

        voiceover_was_playing = false;
        music_audiosource = GameObject.Find("Script").AddComponent<AudioSource>();
		music_audiosource.loop = true;
        voiceover_audiosource.priority = 2;
        music_was_playing = false;

        default_portal_scale = portal.transform.localScale;
        default_portal_position = portal.transform.position;

        default_look_ahead = new Vector3(0, 0, 1);
        look_ahead = default_look_ahead;
        lazy_look_ahead = default_look_ahead;
        very_lazy_look_ahead = default_look_ahead;
        //super_lazy_look_ahead = default_look_ahead;
        player_head = new Vector3(0, 2, 0);

		if (MouseRotatesCamera)
		{
			camera_house.transform.rotation = Quaternion.Euler((mouse_y - Screen.height / 2) * -2, (mouse_x - Screen.width / 2) * 2, 0);
		}

		//recenter oculus
		/*if (ovr_manager.usePositionTracking)
		{
			OVRManager.display.RecenterPose();
		}*/

		gaze_pt = new Vector3(1f, .8f, -1f).normalized;

        gaze_pt *= 1000;
        cam_euler = getCamEuler(cam_reticle.transform.position);
        gaze_cam_euler = getCamEuler(gaze_pt);
        anti_gaze_pt = new Vector3(-330f, -350f, 575f);
        anti_gaze_cam_euler = getCamEuler(anti_gaze_pt);

        gazeray.GetComponent<LineRenderer>().SetPosition(0, anti_gaze_pt);
        gazeray.GetComponent<LineRenderer>().SetPosition(1, gaze_pt);
        gazeball.transform.position = gaze_pt;

        for (int i = 0; i < (int)SPEC.COUNT; i++)
        {
            vearth[i].transform.position = anti_gaze_pt;
            nearth[i].transform.position = anti_gaze_pt;
        }
        earth[0].transform.position = anti_gaze_pt.normalized * 600 + new Vector3(0.0f, 0.0f, 0.0f);

        scene_centers[(int)SCENE.ICE] = icecube[0].transform.position;
        scene_centers[(int)SCENE.VOYAGER] = pluto[0].transform.position;
        scene_centers[(int)SCENE.NOTHING] = milky[0].transform.position;
        scene_centers[(int)SCENE.EXTREME] = blackhole[0].transform.position;
        scene_centers[(int)SCENE.EARTH] = earth[0].transform.position;
        scene_centers[(int)SCENE.CREDITS] = new Vector3(0, 0, 0);

        spec_euler = cam_euler;
        spec_euler.x = -3.141592f / 3f;
        spec_projection.transform.rotation = rotationFromEuler(spec_euler);

        //skyboxes = new Material[(int)SCENE.COUNT, (int)SPEC.COUNT];
        //for (int i = 0; i < (int)SCENE.COUNT; i++)
        //    for (int j = 0; j < (int)SPEC.COUNT; j++)
        //        skyboxes[i, j] = Resources.Load<Material>(skybox_files[i, j]);
		//skyboxes are just referenced in the inspector now - no need to load from Resources
        main_camera_skybox.material = GetSkybox(cur_scene_i, cur_spec_i);
        portal_camera_next_skybox.material = GetSkybox(next_scene_i, (int)SPEC.VIZ);

        grid.SetActive(false);
		grid_eventPlayer.keepPlaying = false;

		reStart();
        MapVols();
        SetupScene();

        lang_sel_reticle1.SetActive(false);
        arrow.SetActive(false);
        languagesel.SetActive(false);
        startsel.SetActive(false);

        hmd_mounted = true;
    }

    //called just before portal to next scene appears
    void PreSetupNextScene()
    {
        scene_rots[next_scene_i] = 0;
		//Analytics: level start
		//AnalyticsEvent.LevelStart(next_scene_i); //looks like this happens in SetupScene instead
		switch (next_scene_i)
        {

            case (int)SCENE.ICE:
                break;

            case (int)SCENE.VOYAGER:
                break;

            case (int)SCENE.NOTHING:
                break;

            case (int)SCENE.EXTREME:
                for (int i = 0; i < (int)SPEC.COUNT; i++)
                {
                    foreach (Transform child_transform in blackhole[i].transform)
                    {
                        GameObject child = child_transform.gameObject;
                        ParticleSystem ps = child.GetComponent<ParticleSystem>();
                        if (ps) ps.Play();
                    }
                }
                break;

            case (int)SCENE.EARTH:
                break;

            case (int)SCENE.CREDITS:
                break;

        }
    }

    void SetupScene()
    {
        IceCubeAnalytics.Instance.LogSceneChanged(((SCENE)cur_scene_i).ToString());
        //Debug.Log("Scene changed: " + ((SCENE)cur_scene_i).ToString());

        SetSpec((int)SPEC.VIZ, false);

        for (int i = 0; i < 3; i++)
            ar_maps[i].SetActive(false);
        spec_projection.SetActive(false);
        gaze_reticle.SetActive(false);

        main_camera_skybox.material = GetSkybox(cur_scene_i, cur_spec_i);
        if (cur_scene_i != (int)SCENE.ICE)
        {
            gazeray.SetActive(true);
            gazeball.SetActive(true);
        }

        AnimationCurve curve;
        float lw;

        lw = 0.0001f;
        curve = new AnimationCurve();
        curve.AddKey(0, lw);
        curve.AddKey(1, lw);
        for (int i = 0; i < MAX_LABELS; i++)
        {
            ar_label_left_texts[i].text = string.Empty;
            ar_label_right_texts[i].text = string.Empty;
            ar_label_bh_texts[i].text = string.Empty;

            ar_label_lefts[i].transform.localScale = new Vector3(0f, 0f, 0f);
            ar_label_rights[i].transform.localScale = new Vector3(0f, 0f, 0f);
            ar_label_bhs[i].transform.localScale = new Vector3(0f, 0f, 0f);
            ar_label_lefts[i].transform.position = new Vector3(0f, 0f, 0f);
            ar_label_rights[i].transform.position = new Vector3(0f, 0f, 0f);
            ar_label_bhs[i].transform.position = new Vector3(0f, 0f, 0f);
            ar_label_left_kids[i].transform.localPosition = new Vector3(0f, 0f, 0f);
            ar_label_right_kids[i].transform.localPosition = new Vector3(0f, 0f, 0f);
            ar_label_bh_kids[i].transform.localPosition = new Vector3(0f, 0f, 0f);
            ar_label_checks[i].SetActive(false);
            ar_spec_checks[i].SetActive(false);

            ar_progress_lines[i].widthCurve = curve;
            for (int j = 0; j < 2; j++)
                ar_progress_lines[i].SetPosition(j, new Vector3(0, 0, 0));
        }

        int label_left_i = 0;
        int label_right_i = 0;
        int label_bh_i = 0;

        switch (cur_scene_i)
        {
            case (int)SCENE.ICE:
                advance_passed_ice_0 = false;
                advance_passed_ice_1 = false;
                event_player_logged = false;
                ar_label_rights[label_right_i].transform.localScale = new Vector3(3f, 3f, 3f);
                ar_label_rights[label_right_i].transform.position = icecube[0].transform.position;
                ar_label_right_kids[label_right_i].transform.localPosition = new Vector3(20, 0, -20);
                ar_label_rights[label_right_i].transform.rotation = rotationFromEuler(getCamEuler(ar_label_rights[label_right_i].transform.position));

                //while (language_selected)
                //{
                    ar_label_right_texts[label_right_i].text = "ICECUBE";
                //}
                label_right_i++;

                gaze_projection.transform.rotation = rotationFromEuler(gaze_cam_euler);
                portal_projection.transform.rotation = rotationFromEuler(gaze_cam_euler);
                gazeray.SetActive(false);
                gazeball.SetActive(false);

                //resetting credits for credit scene
                credits_t = 0;
                if (credits_i * 2 + 1 < credit_strings.Length)
                {
                    credits_text_0.text = LocalizationManager.instance.GetLocalizedValue(credit_strings[credits_i * 2 + 0]);
                    credits_text_1.text = LocalizationManager.instance.GetLocalizedValue(credit_strings[credits_i * 2 + 1]);
                }
                credits_i = 0;

				//set a closer far clip plane to avoid z-fighting
				Camera.main.farClipPlane = IceFarClip;

				break;

            case (int)SCENE.VOYAGER:

                spec_projection.SetActive(true);
                
                IceCubeAnalytics.Instance.LogObjectDisplayed(false, "vision_modules", spec_projection.transform.position, spec_projection.transform.rotation, ((SCENE)cur_scene_i).ToString());
                //Debug.Log("Object displayed: vision_modules");

                advance_passed_voyager_0 = false;
                advance_passed_voyager_1 = false;

                ar_label_rights[label_right_i].transform.localScale = new Vector3(10f, 10f, 10f);
                ar_label_rights[label_right_i].transform.position = pluto[0].transform.position;
                ar_label_right_kids[label_right_i].transform.localPosition = new Vector3(11, 0, 0);
                ar_label_rights[label_right_i].transform.rotation = rotationFromEuler(getCamEuler(ar_label_rights[label_right_i].transform.position));
                ar_label_right_texts[label_right_i].text = LocalizationManager.instance.GetLocalizedValue("pluto");//"PLUTO";
                label_right_i++;

                ar_label_lefts[label_left_i].transform.localScale = new Vector3(10f, 10f, 10f);
                ar_label_lefts[label_left_i].transform.position = vearth[0].transform.position;
                ar_label_left_kids[label_left_i].transform.localPosition = new Vector3(-5, 0, 0);
                ar_label_lefts[label_left_i].transform.rotation = rotationFromEuler(getCamEuler(ar_label_lefts[label_left_i].transform.position));
                ar_label_left_texts[label_left_i].text = LocalizationManager.instance.GetLocalizedValue("earth");//"EARTH";
                label_left_i++;

                gaze_projection.transform.rotation = rotationFromEuler(gaze_cam_euler);
                portal_projection.transform.rotation = rotationFromEuler(gaze_cam_euler);

                ar_maps[0].SetActive(true);
                grid.SetActive(false);
				grid_eventPlayer.keepPlaying = false;

				//return to a farther far clip pane so we can see space things
				Camera.main.farClipPlane = SpaceFarClip;

				break;

            case (int)SCENE.NOTHING:

                ar_label_rights[label_right_i].transform.localScale = new Vector3(2f, 2f, 2f);
                ar_label_rights[label_right_i].transform.position = milky[0].transform.position;
                ar_label_right_kids[label_right_i].transform.localPosition = new Vector3(10, -5, 0);
                ar_label_rights[label_right_i].transform.rotation = rotationFromEuler(getCamEuler(ar_label_rights[label_right_i].transform.position));
                ar_label_right_texts[label_right_i].text = LocalizationManager.instance.GetLocalizedValue("milkyway");
                label_right_i++;

                ar_label_lefts[label_left_i].transform.localScale = new Vector3(10f, 10f, 10f);
                ar_label_lefts[label_left_i].transform.position = nearth[0].transform.position;
                ar_label_left_kids[label_left_i].transform.localPosition = new Vector3(-5, 0, 0);
                ar_label_lefts[label_left_i].transform.rotation = rotationFromEuler(getCamEuler(ar_label_lefts[label_left_i].transform.position));
                ar_label_left_texts[label_left_i].text = LocalizationManager.instance.GetLocalizedValue("earth");
                label_left_i++;

                gaze_projection.transform.rotation = rotationFromEuler(gaze_cam_euler);
                portal_projection.transform.rotation = rotationFromEuler(gaze_cam_euler);

                ar_maps[1].SetActive(true);

                spec_projection.SetActive(true);
                IceCubeAnalytics.Instance.LogObjectDisplayed(false, "vision_modules", spec_projection.transform.position, spec_projection.transform.rotation, ((SCENE)cur_scene_i).ToString());
                //Debug.Log("Object displayed: vision_modules");

                break;

            case (int)SCENE.EXTREME:

                float bar_y = -2;
                float bar_x = -11;
                logged_spec_gam = false;
                logged_spec_viz = false;
                logged_spec_neu = false;
                ar_label_bhs[label_bh_i].transform.localScale = new Vector3(20f, 20f, 20f);
                ar_label_bhs[label_bh_i].transform.position = blackhole[0].transform.position;
                ar_label_bh_kids[label_bh_i].transform.localPosition = new Vector3(25, 5, 0);
                ar_label_bhs[label_bh_i].transform.rotation = rotationFromEuler(getCamEuler(ar_label_bhs[label_bh_i].transform.position));
                ar_label_bh_texts[label_bh_i].text = LocalizationManager.instance.GetLocalizedValue("visible");

                ar_progress_offsets[label_bh_i].transform.localScale = ar_label_bhs[label_bh_i].transform.localScale;
                ar_progress_offsets[label_bh_i].transform.position = ar_label_bh_kids[label_bh_i].transform.position;
                ar_progress_offsets[label_bh_i].transform.rotation = ar_label_bhs[label_bh_i].transform.rotation;
                lw = 10f;
                curve = new AnimationCurve();
                curve.AddKey(0, lw);
                curve.AddKey(1, lw);
                ar_progress_lines[label_bh_i].widthCurve = curve;
                ar_progress_lines[label_bh_i].SetPosition(0, new Vector3(bar_x, bar_y, 0));
                ar_progress_lines[label_bh_i].SetPosition(1, new Vector3(bar_x, bar_y, 0));
                label_bh_i++;


                ar_label_bhs[label_bh_i].transform.localScale = new Vector3(20f, 20f, 20f);
                ar_label_bhs[label_bh_i].transform.position = blackhole[0].transform.position;
                ar_label_bh_kids[label_bh_i].transform.localPosition = new Vector3(27, 0, 0);
                ar_label_bhs[label_bh_i].transform.rotation = rotationFromEuler(getCamEuler(ar_label_bhs[label_bh_i].transform.position));
                ar_label_bh_texts[label_bh_i].text = LocalizationManager.instance.GetLocalizedValue("xray");

                ar_progress_offsets[label_bh_i].transform.localScale = ar_label_bhs[label_bh_i].transform.localScale;
                ar_progress_offsets[label_bh_i].transform.position = ar_label_bh_kids[label_bh_i].transform.position;
                ar_progress_offsets[label_bh_i].transform.rotation = ar_label_bhs[label_bh_i].transform.rotation;
                lw = 10f;
                curve = new AnimationCurve();
                curve.AddKey(0, lw);
                curve.AddKey(1, lw);
                ar_progress_lines[label_bh_i].widthCurve = curve;
                ar_progress_lines[label_bh_i].SetPosition(0, new Vector3(bar_x, bar_y, 0));
                ar_progress_lines[label_bh_i].SetPosition(1, new Vector3(bar_x, bar_y, 0));
                label_bh_i++;


                ar_label_bhs[label_bh_i].transform.localScale = new Vector3(20f, 20f, 20f);
                ar_label_bhs[label_bh_i].transform.position = blackhole[0].transform.position;
                ar_label_bh_kids[label_bh_i].transform.localPosition = new Vector3(25, -5, 0);
                ar_label_bhs[label_bh_i].transform.rotation = rotationFromEuler(getCamEuler(ar_label_bhs[label_bh_i].transform.position));
                ar_label_bh_texts[label_bh_i].text = LocalizationManager.instance.GetLocalizedValue("neutrino");

                ar_progress_offsets[label_bh_i].transform.localScale = ar_label_bhs[label_bh_i].transform.localScale;
                ar_progress_offsets[label_bh_i].transform.position = ar_label_bh_kids[label_bh_i].transform.position;
                ar_progress_offsets[label_bh_i].transform.rotation = ar_label_bhs[label_bh_i].transform.rotation;
                lw = 10f;
                curve = new AnimationCurve();
                curve.AddKey(0, lw);
                curve.AddKey(1, lw);
                ar_progress_lines[label_bh_i].widthCurve = curve;
                ar_progress_lines[label_bh_i].SetPosition(0, new Vector3(bar_x, bar_y, 0));
                ar_progress_lines[label_bh_i].SetPosition(1, new Vector3(bar_x, bar_y, 0));
                label_bh_i++;


                ar_label_lefts[label_left_i].transform.localScale = new Vector3(20f, 20f, 20f);
                ar_label_lefts[label_left_i].transform.position = blackhole[0].transform.position;
                ar_label_left_kids[label_left_i].transform.localPosition = new Vector3(-25, 0, 0);
                ar_label_lefts[label_left_i].transform.rotation = rotationFromEuler(getCamEuler(ar_label_lefts[label_left_i].transform.position));
                ar_label_left_texts[label_left_i].text = LocalizationManager.instance.GetLocalizedValue("blackhole");
                label_left_i++;

                for (int j = 0; j < (int)SPEC.COUNT; j++)
                    ta[(int)SCENE.EXTREME, j] = 0;

                //should have also been done in pre-setup, but can't hurt (in case of debug "start here")
                for (int i = 0; i < (int)SPEC.COUNT; i++)
                {
                    foreach (Transform child_transform in blackhole[i].transform)
                    {
                        GameObject child = child_transform.gameObject;
                        ParticleSystem ps = child.GetComponent<ParticleSystem>();
                        if (ps) ps.Play();
                    }
                }

                ar_maps[2].SetActive(true);
                spec_projection.SetActive(true);
                IceCubeAnalytics.Instance.LogObjectDisplayed(false, "vision_modules", spec_projection.transform.position, spec_projection.transform.rotation, ((SCENE)cur_scene_i).ToString());
                //Debug.Log("Object displayed: vision_modules");

                gaze_projection.transform.rotation = rotationFromEuler(anti_gaze_cam_euler);
                portal_projection.transform.rotation = rotationFromEuler(anti_gaze_cam_euler);

                ar_alert.SetActive(true);
                ar_timer.SetActive(true);
                timer_t = 0;
                alert_t = 0;

                break;

            case (int)SCENE.EARTH:

                ar_label_lefts[label_left_i].transform.localScale = new Vector3(8f, 8f, 8f);
                ar_label_lefts[label_left_i].transform.position = earth[0].transform.position;
                ar_label_left_kids[label_left_i].transform.localPosition = new Vector3(-5, 0, 0);
                ar_label_lefts[label_left_i].transform.rotation = rotationFromEuler(getCamEuler(ar_label_lefts[label_left_i].transform.position));
                ar_label_left_texts[label_left_i].text = "ICECUBE";
                label_left_i++;

                ar_label_rights[label_right_i].transform.localScale = new Vector3(8f, 8f, 8f);
                ar_label_rights[label_right_i].transform.position = esun[0].transform.position;
                ar_label_right_kids[label_right_i].transform.localPosition = new Vector3(15, 0, 0);
                ar_label_rights[label_right_i].transform.rotation = rotationFromEuler(getCamEuler(ar_label_rights[label_right_i].transform.position));
                ar_label_right_texts[label_right_i].text = LocalizationManager.instance.GetLocalizedValue("sun");
                label_right_i++;

                ar_alert.SetActive(false);
                ar_timer.SetActive(false);

                for (int i = 0; i < (int)SPEC.COUNT; i++)
                {
                    foreach (Transform child_transform in blackhole[i].transform)
                    {
                        GameObject child = child_transform.gameObject;
                        ParticleSystem ps = child.GetComponent<ParticleSystem>();
                        if (ps) ps.Stop();
                    }
                }

                gaze_projection.transform.rotation = rotationFromEuler(anti_gaze_cam_euler);
                portal_projection.transform.rotation = rotationFromEuler(anti_gaze_cam_euler);

                break;

            case (int)SCENE.CREDITS:

                gaze_reticle.SetActive(false);
				cam_reticle.SetActive(false);
                gazeray.SetActive(false);
                gazeball.SetActive(false);

                gaze_projection.transform.rotation = rotationFromEuler(gaze_cam_euler);
                portal_projection.transform.rotation = rotationFromEuler(gaze_cam_euler);
                spec_projection.SetActive(false);
                language1_trigger.reset();
                languagepor_trigger.reset();

                break;
        }

        helmet_light_light.color = helmet_colors[cur_scene_i];

        // 
        if (!voiceovers_played[cur_scene_i, (int)SPEC.VIZ] && dumb_delay_t > dumb_delay_t_max)
        {
            if (voiceover_audiosource.isPlaying && espAudio.isPlaying && porAudio.isPlaying)
            {
                voiceover_audiosource.Stop();
                espAudio.Stop();
                porAudio.Stop();
                if(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i].Length > 0)
                {
                    IceCubeAnalytics.Instance.LogAudioComplete(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i], ((SCENE)cur_scene_i).ToString());
                    //Debug.Log(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i] + " complete");
                }
            }
			
            Language(cur_scene_i, (int)SPEC.VIZ);

            voiceover_audiosource.Play();
            espAudio.Play();
            porAudio.Play();
			
            voiceover_was_playing = true;
            voiceovers_played[cur_scene_i, (int)SPEC.VIZ] = true;
            subtitle_i = 0;
            subtitle_t = 0;
            subtitles_text.text = string.Empty;
            subtitle_spec = (int)SPEC.VIZ;
			if(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i+1].Length > 0)
			{
				IceCubeAnalytics.Instance.LogAudioStarted(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i+1], ((SCENE)cur_scene_i).ToString());
				//Debug.Log(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i+1] + " started");
                audio_started_spec = subtitle_spec;
                audio_started_scene = cur_scene_i;
                audio_started_subtitle = subtitle_i+1;
			}
        }
        if (music_audiosource.isPlaying) music_audiosource.Stop();
        music_audiosource.clip = GetMusicClip(cur_scene_i, (int)SPEC.VIZ);
        music_audiosource.volume = music_vols[cur_scene_i, (int)SPEC.VIZ];
        music_audiosource.Play();
        music_was_playing = true;


    }

    // Loads appropriate voiceovers accordingly 
    void Language(int scene, int spec)
    {	//load voiceover clips as they are needed
        if (LocalizationManager.instance.spanish)
        {
            espAudio.volume = voiceover_vols[scene, spec];
			if(!espVoiceoversLoaded[scene, spec])
			{
				espVoiceovers[scene, spec] = Resources.Load<AudioClip>(espVoiceover_files[scene, spec]);
				espVoiceoversLoaded[scene, spec] = true;
			}
            espAudio.clip = espVoiceovers[scene, spec];

            voiceover_audiosource.volume = 0.0f;
            //voiceover_audiosource.clip = voiceovers[scene, specc];

            porAudio.volume = 0.0f;
            //porAudio.clip = porVoiceovers[scene, specc];
        }
        else if(LocalizationManager.instance.portuguese)
        {
            porAudio.volume = voiceover_vols[scene, spec];
			if (!porVoiceoversLoaded[scene, spec])
			{
				porVoiceovers[scene, spec] = Resources.Load<AudioClip>(porVoiceover_files[scene, spec]);
				porVoiceoversLoaded[scene, spec] = true;
			}
			porAudio.clip = porVoiceovers[scene, spec];
            
            espAudio.volume = 0.0f;
			//espAudio.clip = espVoiceovers[scene, specc];

            voiceover_audiosource.volume = 0.0f;
            //voiceover_audiosource.clip = voiceovers[scene, specc];
        }
        else //English
        {
            espAudio.volume = 0.0f;
            //espAudio.clip = espVoiceovers[scene, spec];

            porAudio.volume = 0.0f;
            //porAudio.clip = porVoiceovers[scene, specc];

            voiceover_audiosource.volume = voiceover_vols[scene, spec];
			if (!voiceoversLoaded[scene, spec])
			{
				voiceovers[scene, spec] = Resources.Load<AudioClip>(voiceover_files[scene, spec]);
				voiceoversLoaded[scene, spec] = true;
			}
			voiceover_audiosource.clip = voiceovers[scene, spec];
        }
    }

    void UpdateScene()
    {
        if (LocalizationManager.instance.spanish)
        {
            espAudio.volume = voiceover_vols[cur_scene_i, cur_spec_i];
            voiceover_audiosource.volume = 0.0f;
            porAudio.volume = 0.0f;
        }
        else if (LocalizationManager.instance.portuguese)
        {
            porAudio.volume = voiceover_vols[cur_scene_i, cur_spec_i];
            voiceover_audiosource.volume = 0.0f;
            espAudio.volume = 0.0f;
        }
        else
        {
            espAudio.volume = 0.0f;
            porAudio.volume = 0.0f;
            voiceover_audiosource.volume = voiceover_vols[cur_scene_i, cur_spec_i];
        }

        float old_ta = ta[cur_scene_i, cur_spec_i];
        if (!advance_paused && hmd_mounted) ta[cur_scene_i, cur_spec_i] += Time.deltaTime;
        float cur_ta = ta[cur_scene_i, cur_spec_i];

        switch (cur_scene_i)
        {
            case (int)SCENE.ICE:
                float grid_t;
                float pulse_t;
                float beam_t;

                //prompt grid/pulse/beam according to language
                if (LocalizationManager.instance.spanish)
                {
                    //Debug.Log("in the spanish pulse ");
                    grid_t = 40f + dumb_delay_t_max;
                    pulse_t = 47f + dumb_delay_t_max;
                    beam_t = 59f + dumb_delay_t_max;
                }
                else if(LocalizationManager.instance.portuguese)
                {
                    grid_t = 48.6f + dumb_delay_t_max;
                    pulse_t = 57.9f + dumb_delay_t_max;
                    beam_t = 70f + dumb_delay_t_max;
                }
                else
                {
                    grid_t = 32f + dumb_delay_t_max;
                    pulse_t = 42f + dumb_delay_t_max;
                    beam_t = 49f + dumb_delay_t_max;
                }

				//pulse
				if (cur_ta < pulse_t)
				{
					nwave_t_10 = 0;
				}
				else
				{
					grid_eventPlayer.keepPlaying = true;
                    
                    if(!event_player_logged)
                    {
                        event_player_logged = true;
                        IceCubeAnalytics.Instance.LogObjectDisplayed(false, "neutrino_event", grid_eventPlayer.gameObject.transform.position, grid_eventPlayer.gameObject.transform.rotation, ((SCENE)cur_scene_i).ToString());
                        //Debug.Log("Object displayed: neutrino event");
                    }
                }

                //grid
                if (cur_ta >= grid_t)
                {
                    if (old_ta < grid_t) //newly here
                    {
                        grid.SetActive(true);
                        grid_yacc = 0.5f;
                    }
					
                    if (grid_yacc != 0)
                    {
                        grid_yvel += grid_yacc;
                        grid_yoff += grid_yvel;
                        if (grid_yoff > 0) { grid_yoff *= -1f; grid_yvel *= -0.5f; }
                        grid_yvel *= 0.96f;
                        if (Mathf.Abs(grid_yoff) < 1 && Mathf.Abs(grid_yvel) < 1) { grid_yoff = 0; grid_yvel = 0; grid_yacc = 0; }
                        grid.transform.position = new Vector3(grid.transform.position.x, grid_oyoff + grid_yoff, grid.transform.position.z);
                    }
					
					if (old_ta < grid_t) //newly here
                    {
						IceCubeAnalytics.Instance.LogObjectDisplayed(false, "grid", grid.transform.position, grid.transform.rotation, ((SCENE)cur_scene_i).ToString());
                        //Debug.Log("Object displayed grid");
					}
                }

                //ray
                if (cur_ta >= beam_t)
					
                    if (old_ta < beam_t) //newly here
                    {
						if(!gazeray.activeSelf)
						{
							gazeray.SetActive(true);
							gazeball.SetActive(true);
							IceCubeAnalytics.Instance.LogObjectDisplayed(false, "gazeray", gazeray.transform.position, gazeray.transform.rotation, ((SCENE)cur_scene_i).ToString());
                            //Debug.Log("Object displayed gazeray");
							IceCubeAnalytics.Instance.LogObjectDisplayed(false, "gazeball", gazeball.transform.position, gazeball.transform.rotation, ((SCENE)cur_scene_i).ToString());
                            //Debug.Log("Object displayed gazeball");
						}
                    }
                //command, gaze up 
                if (subtitle_i == subtitle_pause_i_ice_0 && !advance_passed_ice_0)
                {
					//todo - incorporate "object assigned here"
                    gaze_projection.transform.rotation = rotationFromEuler(getEuler(new Vector3(0f, 10f, 10f).normalized));
                    
                    if(!gaze_reticle.activeSelf)
                    {
                        IceCubeAnalytics.Instance.LogObjectAssigned("gazeup", ((SCENE)cur_scene_i).ToString());
                        IceCubeAnalytics.Instance.LogObjectDisplayed(true, "gazeup", gaze_reticle.transform.position, gaze_projection.transform.rotation, ((SCENE)cur_scene_i).ToString());
                        //Debug.Log("Object displayed gazeup");
                        gaze_reticle.SetActive(true);
                    }

                    advance_trigger.position = gaze_reticle.transform.position;
                    arrow.SetActive(true);

                    if (advance_trigger.tick(cam_reticle.transform.position, Time.deltaTime))
                    {
                        if (advance_trigger.just_triggered)
                        {
                            advance_passed_ice_0 = true;
                            gaze_reticle.SetActive(false);
                            PlaySFX(SFX.SELECT);
                            gaze_projection.transform.rotation = rotationFromEuler(gaze_cam_euler);
							IceCubeAnalytics.Instance.LogObjectSelected("gazeup", ((SCENE)cur_scene_i).ToString());
                            //Debug.Log("Gaze up selected");
							//IceCubeAnalytics.Instance.LogObjectAssigned("gazefeet");
                        }
                    }
                }
                //command, feet gaze
                if (subtitle_i == subtitle_pause_i_ice_1 && !advance_passed_ice_1)
                {
                    gaze_projection.transform.rotation = rotationFromEuler(getEuler(new Vector3(0f, -10f, 10f).normalized));
                    if(!gaze_reticle.activeSelf)
                    {
                        IceCubeAnalytics.Instance.LogObjectAssigned("gazefeet", ((SCENE)cur_scene_i).ToString());
                        IceCubeAnalytics.Instance.LogObjectDisplayed(true, "gazefeet", gaze_reticle.transform.position, gaze_projection.transform.rotation, ((SCENE)cur_scene_i).ToString());
                        //Debug.Log("Gaze feet displayed");
                        gaze_reticle.SetActive(true);
                    }

                    advance_trigger.position = gaze_reticle.transform.position;
                    if (advance_trigger.tick(cam_reticle.transform.position, Time.deltaTime))
                    {
                        if (advance_trigger.just_triggered)
                        {
                            advance_passed_ice_1 = true;
                            gaze_reticle.SetActive(false);
                            arrow.SetActive(false);
                            PlaySFX(SFX.SELECT);
                            gaze_projection.transform.rotation = rotationFromEuler(gaze_cam_euler);
							IceCubeAnalytics.Instance.LogObjectSelected("gazefeet", ((SCENE)cur_scene_i).ToString());
                            //Debug.Log("Gaze feet selected");
                        }
                    }
                }

                break;

            case (int)SCENE.VOYAGER:

                if (voiceovers_played[cur_scene_i, (int)cur_spec_i] && !voiceover_was_playing)
                {
                    if(cur_spec_i == (int)SPEC.VIZ && !logged_spec_gam)
                    {
                        logged_spec_gam = true;
                        IceCubeAnalytics.Instance.LogObjectAssigned("xray_vision", ((SCENE)cur_scene_i).ToString());
                        //Debug.Log("Object assigned xray_vision");
                    }
                    
                    if(cur_spec_i == (int)SPEC.GAM && !logged_spec_neu)
                    {
                        logged_spec_neu = true;
                        IceCubeAnalytics.Instance.LogObjectAssigned("neutrino_vision", ((SCENE)cur_scene_i).ToString());
                        //Debug.Log("Object assigned neutrino_vision");
                    }
                }

                //command
                if (cur_spec_i == (int)SPEC.GAM && subtitle_i == subtitle_pause_i_voyager_0 && !advance_passed_voyager_0)
                {
                    gaze_projection.transform.rotation = rotationFromEuler(getCamEuler(pluto[0].transform.position));
                    if(!gaze_reticle.activeSelf)
					{
                        IceCubeAnalytics.Instance.LogObjectAssigned("pluto1", ((SCENE)cur_scene_i).ToString());
						IceCubeAnalytics.Instance.LogObjectDisplayed(true, "pluto1", gaze_reticle.transform.position, gaze_projection.transform.rotation, ((SCENE)cur_scene_i).ToString());
                        //Debug.Log("Pluto 1 displayed");
					}
					gaze_reticle.SetActive(true);
					
                    advance_trigger.position = gaze_reticle.transform.position;
                    if (advance_trigger.tick(cam_reticle.transform.position, Time.deltaTime))
                    {
                        if (advance_trigger.just_triggered)
                        {
                            advance_passed_voyager_0 = true;
                            gaze_reticle.SetActive(false);
                            gaze_projection.transform.rotation = rotationFromEuler(gaze_cam_euler);
							IceCubeAnalytics.Instance.LogObjectSelected("pluto1", ((SCENE)cur_scene_i).ToString());
                             Debug.Log("Pluto 1 selected");
                        }
                    }
                }
                if (cur_spec_i == (int)SPEC.NEU && subtitle_i == subtitle_pause_i_voyager_1 && !advance_passed_voyager_1)
                {
                    gaze_projection.transform.rotation = rotationFromEuler(getCamEuler(pluto[0].transform.position));
					if(!gaze_reticle.activeSelf)
					{
                        IceCubeAnalytics.Instance.LogObjectAssigned("pluto2", ((SCENE)cur_scene_i).ToString());
						IceCubeAnalytics.Instance.LogObjectDisplayed(true, "pluto2", gaze_reticle.transform.position, gaze_projection.transform.rotation, ((SCENE)cur_scene_i).ToString());
                        //Debug.Log("Pluto 2 displayed");
					}
					gaze_reticle.SetActive(true);
                    advance_trigger.position = gaze_reticle.transform.position;
                    if (advance_trigger.tick(cam_reticle.transform.position, Time.deltaTime))
                    {
                        if (advance_trigger.just_triggered)
                        {
                            advance_passed_voyager_1 = true;
                            gaze_reticle.SetActive(false);
                            gaze_projection.transform.rotation = rotationFromEuler(gaze_cam_euler);
							IceCubeAnalytics.Instance.LogObjectSelected("pluto2", ((SCENE)cur_scene_i).ToString());
                            //Debug.Log("Pluto 2 selected");
                        }
                    }
                }

                break;

            case (int)SCENE.NOTHING:


                break;

            case (int)SCENE.EXTREME:

                alert_t += Time.deltaTime;
                timer_t += Time.deltaTime;
                if (Mathf.Floor(alert_t) % 2 == 1)
                    ar_alert.SetActive(false);
                else
                    ar_alert.SetActive(true);

                float seconds_left = 70 - timer_t;//60 - timer_t;

                if (seconds_left > 0)
                {
                    if (voiceovers_played[cur_scene_i, (int)cur_spec_i] && !voiceover_was_playing)
                    {
                        if(cur_spec_i == (int)SPEC.VIZ && !logged_spec_viz)
                        {
                            logged_spec_viz = true;
                            IceCubeAnalytics.Instance.LogObjectAssigned("visible_light", ((SCENE)cur_scene_i).ToString());
                            //Debug.Log("Object assigned visible_light");
                            IceCubeAnalytics.Instance.LogObjectAssigned("xray_vision", ((SCENE)cur_scene_i).ToString());
                            //Debug.Log("Object assigned xray_vision");
                            IceCubeAnalytics.Instance.LogObjectAssigned("neutrino_vision", ((SCENE)cur_scene_i).ToString());
                            //Debug.Log("Object assigned neutrino_vision");
                        }
                    }

                    if (blackhole_spec_triggered[cur_spec_i] == 0)
                    {
                        if (!HeadsetPaused && !voiceover_audiosource.isPlaying && !espAudio.isPlaying && !porAudio.isPlaying)
                        {
							gaze_projection.transform.rotation = rotationFromEuler(getCamEuler(blackhole[0].transform.position));
							
                            if (!gaze_reticle.activeSelf) 
							{
								gaze_reticle.SetActive(true);
                                IceCubeAnalytics.Instance.LogObjectAssigned("blackhole", ((SCENE)cur_scene_i).ToString());
								IceCubeAnalytics.Instance.LogObjectDisplayed(true, "blackhole", gaze_reticle.transform.position, gaze_projection.transform.rotation, ((SCENE)cur_scene_i).ToString());
                                //Debug.Log("Blackhole displayed");
							}
							
                            blackhole_trigger.position = gaze_reticle.transform.position;
							
                            if (blackhole_trigger.tick(cam_reticle.transform.position, Time.deltaTime))
                            {
                                if (blackhole_trigger.just_triggered)
                                {
									IceCubeAnalytics.Instance.LogObjectSelected("blackhole", ((SCENE)cur_scene_i).ToString());
                                    //Debug.Log("Black hole selected");
                                    gaze_reticle.SetActive(false);
                                    blackhole_spec_triggered[cur_spec_i] = 1;
                                    blackhole_trigger.reset();
                                    bool all_done = true;
                                    for (int i = 0; i < (int)SPEC.COUNT; i++) 
									{
										all_done = (all_done && blackhole_spec_triggered[i] == 1);
									}
									
                                    if (all_done)
                                    {
                                        gaze_projection.transform.rotation = rotationFromEuler(anti_gaze_cam_euler);
                                        gaze_reticle.SetActive(true);
                                        IceCubeAnalytics.Instance.LogObjectAssigned("earth", ((SCENE)cur_scene_i).ToString());
										IceCubeAnalytics.Instance.LogObjectDisplayed(true, "earth", gaze_projection.transform.position, gaze_projection.transform.rotation, ((SCENE)cur_scene_i).ToString());
                                        //Debug.Log("Earth displayed");
                                    }
                                }
                            }
                        }
                    }

                    ar_timer_text.text = "00:" + Mathf.Floor(seconds_left) + ":" + Mathf.Floor((seconds_left - Mathf.Floor(seconds_left)) * 100);
                    float bar_y = -2;
                    float bar_x = -11;
                    float bar_w = 23;
                    bool play_end = true;

                    for (int i = 0; i < (int)SPEC.COUNT; i++)
                    {
                        float t = blackhole_spec_triggered[i];
                        if (t == 0 && i == cur_spec_i) t = Mathf.Min(1f, (blackhole_trigger.t_in / blackhole_trigger.t_max));
                        ar_progress_lines[i].SetPosition(1, new Vector3(bar_x + bar_w * t, bar_y, 0));
                        if (t == 1 && !ar_label_checks[i].activeSelf)
                        {
                            ar_label_checks[i].SetActive(true);
                            ar_spec_checks[i].SetActive(true);
                            PlaySFX(SFX.COMPLETE);
                        }
                        if (t < 1) play_end = false;
                    }
					
                    if (voiceovers_played[cur_scene_i, (int)SPEC.COUNT]) 
                    {
                        play_end = false;

                    }

                    if (play_end)
                    {
                        if (voiceover_audiosource.isPlaying && espAudio.isPlaying && porAudio.isPlaying)
                        {
                            voiceover_audiosource.Stop();
                            espAudio.Stop();
                            porAudio.Stop();
                            if(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i].Length > 0)
                            {
                                IceCubeAnalytics.Instance.LogAudioComplete(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i], ((SCENE)cur_scene_i).ToString());
                                //Debug.Log(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i] + " complete");
                            }
                        }

                        Language(cur_scene_i, (int)SPEC.COUNT);
                        //Debug.Log("vPlay01");
                        voiceover_audiosource.Play();
                        espAudio.Play();
                        porAudio.Play();
                        voiceover_was_playing = true;
                        voiceovers_played[cur_scene_i, (int)SPEC.COUNT] = true;
                        subtitle_i = 0;
                        subtitle_t = 0;
                        subtitles_text.text = string.Empty;
                        subtitle_spec = (int)SPEC.COUNT;
                        if(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i+1].Length > 0)
                        {
                            IceCubeAnalytics.Instance.LogAudioStarted(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i+1], ((SCENE)cur_scene_i).ToString());
                            //Debug.Log(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i+1] + " started");
                            audio_started_spec = subtitle_spec;
                            audio_started_scene = cur_scene_i;
                            audio_started_subtitle = subtitle_i+1;
                        }
                    }
                }
                else if (in_fail_motion == 0)
                {
                    in_fail_motion = 0.0001f;
                    ar_timer_text.text = "XX:XX:XX";
					IceCubeAnalytics.Instance.LogFailedEnd();
                    //Debug.Log("Log failed");
                    logged_spec_viz = false;
                }

                float bhr_speed = 2.0f;
                for (int i = 0; i < (int)SPEC.COUNT; i++)
                {
                    foreach (Transform child_transform in blackhole[i].transform)
                    {
                        child_transform.localRotation = Quaternion.Euler(10.0f, nwave_t_10 * 36 * bhr_speed, 10.0f);
                    }
                }

                break;

            case (int)SCENE.EARTH:

                earth[0].transform.position = anti_gaze_pt.normalized * 600 + new Vector3(0.0f, 500.0f, 0.0f);
				if(!warp_trigger.just_triggered)
				{
					IceCubeAnalytics.Instance.LogObjectSelected("earth", ((SCENE)cur_scene_i).ToString());
                    //Debug.Log("Earth selected");
				}
                warp_trigger.just_triggered = true;
				
                break;

            case (int)SCENE.CREDITS:

                credits_t += Time.deltaTime;
                if (credits_t > max_credits_t)
                {
                    //Debug.Log(credit_strings.Length);

                    credits_t = 0;
                    credits_i++;
                    if (credits_i * 2 + 1 < credit_strings.Length)
                    {
                        credits_text_0.text = LocalizationManager.instance.GetLocalizedValue(credit_strings[credits_i * 2 + 0]);
                        credits_text_1.text = LocalizationManager.instance.GetLocalizedValue(credit_strings[credits_i * 2 + 1]);
                    }
                }

                break;

        }
		
        if (!gaze_reticle.activeSelf && voiceovers_played[cur_scene_i, (int)SPEC.COUNT] && cur_scene_i != (int)SCENE.CREDITS)
        {
            if(!gaze_reticle.activeSelf)
            {
                gaze_reticle.SetActive(true);
                if(cur_scene_i == (int)SCENE.ICE)
                {
                    IceCubeAnalytics.Instance.LogObjectAssigned("gaze_ice", ((SCENE)cur_scene_i).ToString());
                    IceCubeAnalytics.Instance.LogObjectDisplayed(true, "gaze_ice", gaze_reticle.transform.position, gaze_reticle.transform.rotation, ((SCENE)cur_scene_i).ToString());
                    //Debug.Log("Gaze ice displayed");
                }
                else if(cur_scene_i == (int)SCENE.VOYAGER)
                {
                    IceCubeAnalytics.Instance.LogObjectAssigned("gaze_voyager", ((SCENE)cur_scene_i).ToString());
                    IceCubeAnalytics.Instance.LogObjectDisplayed(true, "gaze_voyager", gaze_reticle.transform.position, gaze_reticle.transform.rotation, ((SCENE)cur_scene_i).ToString());
                    //Debug.Log("Gaze voyager displayed");
                }
                else if(cur_scene_i == (int)SCENE.NOTHING)
                {
                    IceCubeAnalytics.Instance.LogObjectAssigned("gaze_nothing", ((SCENE)cur_scene_i).ToString());
                    IceCubeAnalytics.Instance.LogObjectDisplayed(true, "gaze_nothing", gaze_reticle.transform.position, gaze_reticle.transform.rotation, ((SCENE)cur_scene_i).ToString());
                    //Debug.Log("Gaze nothing displayed");
                }
            }
        }

    }

    void SetSpec(int spec, bool log=true)
    {
        //leaving actualy gameanalytics in (but commented out) so it's easy to find/replace them
        cur_spec_i = spec;
		/*AnalyticsEvent.Custom("set_spec", new Dictionary<string, object>
		{
			{"spec_id", spec }
		});*/

       
		switch (spec)
        {
            case (int)SPEC.GAM:
                spec_sel_reticle.transform.position = spec_gam_reticle.transform.position;
                //GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "Universe", "Scene_" + cur_scene_i, "X-ray", 0);
				if(log)
				{
					IceCubeAnalytics.Instance.LogObjectSelected("xray_vision", ((SCENE)cur_scene_i).ToString());
                    //Debug.Log("Object selected: xray_vision");
				}
                break;
            case (int)SPEC.VIZ:
                spec_sel_reticle.transform.position = spec_viz_reticle.transform.position;
                //GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "Universe", "Scene_" + cur_scene_i, "viz", 0);
				if(log)
				{
					IceCubeAnalytics.Instance.LogObjectSelected("visible_light", ((SCENE)cur_scene_i).ToString());
                    //Debug.Log("Object selected: visible_light");
				}
                break;
            case (int)SPEC.NEU:
                spec_sel_reticle.transform.position = spec_neu_reticle.transform.position;
                //GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "Universe", "Scene_" + cur_scene_i, "neu", 0);
				
				IceCubeAnalytics.Instance.LogObjectSelected("neutrino_vision", ((SCENE)cur_scene_i).ToString());
                //Debug.Log("Object selected: neutrino_vision");
                break;
        }

        main_camera.GetComponent<Camera>().cullingMask = (1 << layers[cur_scene_i, cur_spec_i]) | (1 << default_layer);
        portal_camera_next.GetComponent<Camera>().cullingMask = (1 << layers[next_scene_i, (int)SPEC.VIZ]);
        main_camera_skybox.material = GetSkybox(cur_scene_i, cur_spec_i);
        portal_camera_next_skybox.material = GetSkybox(next_scene_i, (int)SPEC.VIZ);

        if (cur_scene_i == (int)SCENE.EXTREME) blackhole_trigger.reset();
    }

    float nwave_t_1 = 0;
    float nwave_t_10 = 0;

    private bool isCoroutineExecuting = false;
    // Adds delay to start game
    IEnumerator ExecuteAfterTime(float time)
    {
        if (isCoroutineExecuting)
            yield break;

        isCoroutineExecuting = true;

        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        menuscreen.SetActive(false); // hides main menu
        language_selected = true; // condition to run the game
		if (!languageAnalyticsSent)
		{
            IceCubeAnalytics.Instance.LogLanguageSelected(LocalizationManager.instance.SelectedLanguage);
			/*AnalyticsEvent.Custom("language_at_start", new Dictionary<string, object>
			{
				{ "language", LocalizationManager.instance.SelectedLanguage}
			});*/
			//Debug.Log("Sent language at start analytics event");
			languageAnalyticsSent = true;
		}
		voiceover_was_playing = true;


        isCoroutineExecuting = false;
    }

    //adds delay for language options to disperse
    // private bool de = false;
    // IEnumerator langgone(float time)
    // {
    //     if (de)
    //         yield break;

    //     de = true;

    //     yield return new WaitForSeconds(time);

    //     // Code to execute after the delay
    //     // asfter user selects language
    //     //hide lanaguage options
    //     languageop.SetActive(false);
        
    //     de = false;
    // }
    

    // loads language text and changes audio to desired language
    void loadLanguage()
    {
        languageop.SetActive(true);

        if (!language_selected) // true 
        {
			if(LocalizationManager.instance.english)
			{
				lang_sel_reticle1.SetActive(true);
                lang_sel_reticle1.transform.position = lang_eng_reticle1.transform.position;
			}
			else if(LocalizationManager.instance.spanish)
			{
				lang_sel_reticle1.SetActive(true);
                lang_sel_reticle1.transform.position = lang_esp_reticle1.transform.position;
			}
			else if(LocalizationManager.instance.portuguese)
			{
				lang_sel_reticle1.SetActive(true);
				lang_sel_reticle1.transform.position = lang_por_reticle1.transform.position;
			}
			
            language1_trigger.position = lang_eng_reticle1.transform.position;

            //English
            if (language1_trigger.tick(cam_reticle.transform.position, Time.deltaTime))
            {
                lang_sel_reticle1.SetActive(true);
                lang_sel_reticle1.transform.position = lang_eng_reticle1.transform.position;
				
				if(!LocalizationManager.instance.english)
				{
					LocalizationManager.instance.LoadLocalizedText("localizedText_en.json");
					UpdateCaptions();
				}

                espAudio.volume = 0.0f;
                porAudio.volume = 0.0f;
                voiceover_audiosource.volume = voiceover_vols[cur_scene_i, cur_spec_i];
            }

            language1_trigger.position = lang_esp_reticle1.transform.position;

            // Spanish
            if (language1_trigger.tick(cam_reticle.transform.position, Time.deltaTime))
            {
                lang_sel_reticle1.SetActive(true);
                lang_sel_reticle1.transform.position = lang_esp_reticle1.transform.position;
				if(!LocalizationManager.instance.spanish)
				{
					LocalizationManager.instance.LoadLocalizedText("localizedText_es.json");
					UpdateSpanishCues();
				}

                espAudio.volume = voiceover_vols[cur_scene_i, cur_spec_i];
                voiceover_audiosource.volume = 0.0f;
                porAudio.volume = 0.0f;
            }

            languagepor_trigger.position = lang_por_reticle1.transform.position;

            // Portugues
            if (languagepor_trigger.tick(cam_reticle.transform.position, Time.deltaTime))
            {
                lang_sel_reticle1.SetActive(true);
                lang_sel_reticle1.transform.position = lang_por_reticle1.transform.position;
				if(!LocalizationManager.instance.portuguese)
				{
					LocalizationManager.instance.LoadLocalizedText("localizedText_pt.json");
					UpdatePortugueseCues();
				}

                porAudio.volume = voiceover_vols[cur_scene_i, cur_spec_i];
                voiceover_audiosource.volume = 0.0f;
                espAudio.volume = 0.0f;
            }
        }
    }

	void Update()
	{
		//cylinder.transform.LookAt(gaze_reticle.transform.position, Vector3.up);//.position, Vector3.up);
		/*Vector3 toGazeReticle = Vector3.Normalize(cylinder.transform.position - gaze_reticle.transform.position);
        Vector3 vLook = main_camera.transform.forward;
        Vector3 vUp = Vector3.Cross(vLook, toGazeReticle);

        Quaternion q = cylinder.transform.rotation;
        q.SetLookRotation(-toGazeReticle);
        cylinder.transform.rotation = q;*/

		languageop.SetActive(false);

		nwave_t_1 += Time.deltaTime;
		nwave_t_10 += Time.deltaTime;
		while (nwave_t_1 > 1) nwave_t_1 -= 1f;
		while (nwave_t_10 > 10) nwave_t_10 -= 10f;

		//float aspect = main_camera.GetComponent<Camera>().aspect;
		//float fov = main_camera.GetComponent<Camera>().fieldOfView;
		//ar_camera_project.GetComponent<Camera>().aspect = aspect;
		//ar_camera_static.GetComponent<Camera>().aspect = aspect;
		//portal_camera_next.GetComponent<Camera>().aspect = aspect;
		//ar_camera_project.GetComponent<Camera>().fieldOfView = fov;
		//ar_camera_static.GetComponent<Camera>().fieldOfView = fov;
		//portal_camera_next.GetComponent<Camera>().fieldOfView = fov;

		if (Input.GetMouseButtonDown(0))
		{
			mouse_captured = !mouse_captured;
			if (mouse_captured)
			{
				mouse_just_captured = true;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}

		// LOADS Language selection
		//--------------------------------------------------------------------------

		menulanguage.position = language.transform.position;
		if (menulanguage.in_range(cam_reticle.transform.position))
		{
			languagesel.transform.position = language.transform.position;
			languagesel.SetActive(true);
			lang_menu = true;
		}
		// if user select language prompt language options
		if (lang_menu)
		{
			loadLanguage();
		}

		//select start
		start_trigger.position = startbutton.transform.position;
		if (start_trigger.tick(cam_reticle.transform.position, Time.deltaTime))
		{	//language analytics was here, but it triggered too often
			startsel.transform.position = startbutton.transform.position;
			startsel.SetActive(true);
			StartCoroutine(ExecuteAfterTime(2)); // removing screen and delay start
												 //menuscreen.SetActive(false); // hides main menu
												 //language_selected = true; // condition to run the game
												 //voiceover_was_playing = true;
		}
		//----------------------------------------------------------------

		if (Input.GetKeyDown("space"))
		{
			HandleHMDUnmounted();
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			HandleHMDMounted();
		}
		if (Input.GetKeyUp("space"))
		{
			HandleHMDMounted();
		}

		if (in_portal_motion > 0) in_portal_motion += Time.deltaTime * 0.8f;
		if (in_portal_motion > max_portal_motion)
		{
			out_portal_motion = in_portal_motion - max_portal_motion;
			if (out_portal_motion <= 0) out_portal_motion = 0.00001f;
			in_portal_motion = 0;
			if (cur_scene_i == (int)SCENE.CREDITS)
			{
				reStart();
				next_scene_i = (int)SCENE.ICE;
			}
			cur_scene_i = next_scene_i;
			next_scene_i = (next_scene_i + 1) % ((int)SCENE.COUNT);
			SetupScene();
		}
		if (out_portal_motion > 0) out_portal_motion += Time.deltaTime;
		if (out_portal_motion > max_portal_motion) out_portal_motion = 0;

		if (in_portal_motion > 0)
		{
			float t = in_portal_motion / (float)max_portal_motion;
			portal.transform.localPosition = new Vector3(default_portal_position.x, default_portal_position.y, Mathf.Lerp(default_portal_position.z, 0, t * t * t * t * t));
			float engulf = t - 1;
			engulf *= -engulf;
			engulf += 1;
			engulf /= 2;
			portal.transform.localScale = new Vector3(default_portal_scale.x * engulf, default_portal_scale.y * engulf, default_portal_scale.z * engulf);
		}
		else
		{
			portal.transform.localPosition = default_portal_position;
			portal.transform.localScale = new Vector3(0, 0, 0);
		}

		if (in_fail_motion > 0) in_fail_motion += Time.deltaTime * 0.2f;
		if (in_fail_motion > max_fail_motion)
		{
			//FAIL
			for (int i = 0; i < (int)SPEC.COUNT; i++)
				blackhole_spec_triggered[i] = 0;
			out_fail_motion = in_fail_motion - max_fail_motion;
			if (out_fail_motion <= 0) out_fail_motion = 0.00001f;
			in_fail_motion = 0;
			warp_trigger.t_in = 0;
			next_scene_i = cur_scene_i + 1;
			scene_rots[next_scene_i] = 0;
			voiceovers_played[(int)SCENE.EXTREME, (int)SPEC.VIZ] = false;
			voiceovers_played[(int)SCENE.EXTREME, (int)SPEC.COUNT] = false;
			SetupScene();
		}
		if (out_fail_motion > 0) out_fail_motion += Time.deltaTime;
		if (out_fail_motion > max_fail_motion) out_fail_motion = 0;

		// Start - Runs after language is selected
		if (language_selected)
		{
			UpdateScene();

			if (dumb_delay_t > dumb_delay_t_max)
			{
                bool wasPausedThisFrame = false;
				float old_sub_t = subtitle_t;
				subtitle_t += Time.deltaTime;
				if (
				  old_sub_t < subtitle_cues_absolute[cur_scene_i, subtitle_spec, subtitle_i + 1] &&
				  subtitle_t >= subtitle_cues_absolute[cur_scene_i, subtitle_spec, subtitle_i + 1]
				)
				{
					subtitle_i++;
					if (
					  (cur_scene_i == (int)SCENE.ICE && !advance_passed_ice_0 && subtitle_i == subtitle_pause_i_ice_0 + 1) ||
					  (cur_scene_i == (int)SCENE.ICE && !advance_passed_ice_1 && subtitle_i == subtitle_pause_i_ice_1 + 1) ||
					  (cur_scene_i == (int)SCENE.VOYAGER && cur_spec_i == (int)SPEC.GAM && !advance_passed_voyager_0 && subtitle_i == subtitle_pause_i_voyager_0 + 1) ||
					  (cur_scene_i == (int)SCENE.VOYAGER && cur_spec_i == (int)SPEC.NEU && !advance_passed_voyager_1 && subtitle_i == subtitle_pause_i_voyager_1 + 1)
					)
					{
						
						//freeze time
						if (!advance_paused)
						{
							if (voiceover_audiosource.isPlaying || espAudio.isPlaying || porAudio.isPlaying)
							{
								if (LocalizationManager.instance.spanish)
								{
									espAudio.volume = voiceover_vols[cur_scene_i, cur_spec_i];
									voiceover_audiosource.volume = 0.0f;
									porAudio.volume = 0.0f;
								}
								else if (LocalizationManager.instance.portuguese)
								{
									porAudio.volume = voiceover_vols[cur_scene_i, cur_spec_i];
									voiceover_audiosource.volume = 0.0f;
									espAudio.volume = 0.0f;
									//Debug.Log("is spanish playing?");
								}
								else
								{
									espAudio.volume = 0.0f;
									porAudio.volume = 0.0f;
									voiceover_audiosource.volume = voiceover_vols[cur_scene_i, cur_spec_i];
								}
								voiceover_audiosource.Pause();
								//Debug.Log("PAUSE");
								porAudio.Pause();
								espAudio.Pause();
								wasPausedThisFrame = true;

                                
							}
						}
						
						subtitle_i--;
						//if(!advance_paused)
						//{
						//	IceCubeAnalytics.Instance.LogCaption(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i]);
						//}
						

						advance_paused = true;

						subtitle_t = subtitle_cues_absolute[cur_scene_i, subtitle_spec, subtitle_i + 1] - 0.0001f;
					}
					else
					{
						if (advance_paused)
						{
							if (LocalizationManager.instance.spanish)
							{
								espAudio.volume = voiceover_vols[cur_scene_i, cur_spec_i];
								voiceover_audiosource.volume = 0.0f;
								porAudio.volume = 0.0f;
							}
							else if (LocalizationManager.instance.portuguese)
							{
								porAudio.volume = voiceover_vols[cur_scene_i, cur_spec_i];
								voiceover_audiosource.volume = 0.0f;
								espAudio.volume = 0.0f;
								//Debug.Log(LocalizationManager.instance.portuguese);
							}
							else
							{
								espAudio.volume = 0.0f;
								porAudio.volume = 0.0f;
								voiceover_audiosource.volume = voiceover_vols[cur_scene_i, cur_spec_i];
							}
							//Debug.Log("vPla02");
							voiceover_audiosource.Play();
							espAudio.Play();
							porAudio.Play();
							
							if(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i].Length > 0)
							{
								IceCubeAnalytics.Instance.LogAudioStarted(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i], ((SCENE)cur_scene_i).ToString());
								//Debug.Log(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i] + " started");
                                audio_started_spec = subtitle_spec;
                                audio_started_scene = cur_scene_i;
                                audio_started_subtitle = subtitle_i+1;
							}
							
						}
						advance_paused = false;
					}

					subtitles_text.text = subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i];
					if(!advance_paused)
					{
						if(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i].Length > 0)
						{
							IceCubeAnalytics.Instance.LogCaption(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i], ((SCENE)cur_scene_i).ToString());
							//Debug.Log("Caption: " + subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i]);
						}
					}
				}
			}
		}

		if (mouse_captured)
		{
			float in_x = Input.GetAxis("Mouse X") * 10;
			float in_y = Input.GetAxis("Mouse Y") * 10;
			if (!mouse_just_captured)
			{	//force mouse to center of screen if camera rotation is off
				mouse_x += MouseRotatesCamera ? in_x : Screen.height / 2;
				mouse_y += MouseRotatesCamera? in_y : Screen.width / 2;
			}
			else
			{
				if (in_x != 0 || in_y != 0)
					mouse_just_captured = false;
			}
		}
		//don't adjust position if oculus position tracking is being used
		Vector3 offset;
		if (!ovr_manager.usePositionTracking)
		{	//head tracking off
			offset = new Vector3(
							-main_camera.transform.localPosition.x,
							-main_camera.transform.localPosition.y,
							-main_camera.transform.localPosition.z);
		}
		else
		{	//head tracking on
			offset = Vector3.zero;
			//set AR objects to same position as camera
			ARParent.position = ARAnchor.localPosition;
		}
		camera_house.transform.localPosition = offset + player_head;
		if (cur_scene_i == (int)SCENE.EXTREME)
		{
			if (MouseRotatesCamera) camera_house.transform.rotation = Quaternion.Euler((mouse_y - Screen.height / 2) * -2 + Random.Range(-extreme_camera_shake, extreme_camera_shake), (mouse_x - Screen.width / 2) * 2 + Random.Range(-extreme_camera_shake, extreme_camera_shake), 0 + Random.Range(-extreme_camera_shake, extreme_camera_shake));
			else camera_house.transform.rotation = Quaternion.Euler(Random.Range(-extreme_camera_shake, extreme_camera_shake), Random.Range(-extreme_camera_shake, extreme_camera_shake), Random.Range(-extreme_camera_shake, extreme_camera_shake));
		}
		else
		{
			if (MouseRotatesCamera) camera_house.transform.rotation = Quaternion.Euler((mouse_y - Screen.height / 2) * -2, (mouse_x - Screen.width / 2) * 2, 0);
			else camera_house.transform.rotation = Quaternion.Euler(0, 0, 0);
		}

		look_ahead = main_camera.transform.rotation * default_look_ahead;
		lazy_look_ahead = Vector3.Lerp(lazy_look_ahead, look_ahead, 0.1f);
		very_lazy_look_ahead = Vector3.Lerp(very_lazy_look_ahead, look_ahead, 0.002f);
		//super_lazy_look_ahead = Vector3.Lerp(super_lazy_look_ahead, look_ahead, 0.01f);

		if (cur_scene_i == (int)SCENE.EXTREME)
			helmet.transform.position = main_camera.transform.position + new Vector3(Random.Range(-extreme_helmet_shake, extreme_helmet_shake), Random.Range(-extreme_helmet_shake, extreme_helmet_shake), Random.Range(-extreme_helmet_shake, extreme_helmet_shake));
		else
			helmet.transform.position = main_camera.transform.position;
		helmet.transform.rotation = rotationFromEuler(getEuler(lazy_look_ahead));

		cam_euler = getCamEuler(cam_reticle.transform.position);
		spec_euler = getEuler(very_lazy_look_ahead);
		spec_euler.x = -3.141592f / 3f;
		spec_projection.transform.rotation = rotationFromEuler(spec_euler);

		if (in_portal_motion > 0)
		{
			float t = (float)in_portal_motion / max_portal_motion;
			flash_alpha = t * t * t * t * t;
		}
		else if (out_portal_motion > 0)
		{
			flash_alpha = 1.0f - ((float)out_portal_motion / max_portal_motion);
		}
		else if (in_fail_motion > 0)
		{
			float t = (float)in_fail_motion / max_fail_motion;
			flash_alpha = t * t * t * t * t;
		}
		else if (out_fail_motion > 0)
		{
			flash_alpha = 1.0f - ((float)out_fail_motion / max_fail_motion);
		}
		else
			flash_alpha = 0;
		flash_alpha *= 1.1f;
		if (flash_alpha > 1)
			flash_alpha = 1;
		flash_alpha = flash_alpha * flash_alpha;
		flash_alpha = flash_alpha * flash_alpha;
		alpha_material.SetFloat(alpha_id, flash_alpha);

		cam_spinner.transform.localScale = new Vector3(warp_trigger.shrink, warp_trigger.shrink, warp_trigger.shrink);
		cam_spinner.transform.localRotation = Quaternion.Euler(0, 0, warp_trigger.rot);
		warp_trigger.position = gaze_reticle.transform.position;

		if (
			(
			cur_scene_i != (int)SCENE.EARTH &&
			voiceovers_played[cur_scene_i, (int)SPEC.COUNT] &&
			in_fail_motion == 0 &&
			warp_trigger.tick(cam_reticle.transform.position, Time.deltaTime)
		  )
			|| //just use the above for normal use...
		  (
			cur_scene_i == (int)SCENE.EARTH &&
			voiceovers_played[cur_scene_i, (int)SPEC.COUNT] &&
			!voiceover_was_playing
		  ) //weird hack for end scene
		)
		{
			if (warp_trigger.just_in) warp_audiosource_ptr = PlaySFX(SFX.WARP);

			//advance
			if (warp_trigger.just_triggered)
			{
				if (warp_audiosource_ptr != null) warp_audiosource_ptr = null;
				if (in_portal_motion == 0 && out_portal_motion == 0)
				{
					in_portal_motion = Time.deltaTime;
					if(cur_scene_i == (int)SCENE.VOYAGER)
					{
						IceCubeAnalytics.Instance.LogObjectSelected("gaze_voyager", ((SCENE)cur_scene_i).ToString());
                        //Debug.Log("Gaze voyager selected");
					}
					else if(cur_scene_i == (int)SCENE.ICE)
					{
						IceCubeAnalytics.Instance.LogObjectSelected("gaze_ice", ((SCENE)cur_scene_i).ToString());
                        //Debug.Log("Gaze ice selected");
					}
					else if(cur_scene_i == (int)SCENE.NOTHING)
					{
						IceCubeAnalytics.Instance.LogObjectSelected("gaze_nothing", ((SCENE)cur_scene_i).ToString());
                        //Debug.Log("Gaze nothing selected");
					}
					PreSetupNextScene();
				}
				in_fail_motion = 0;
			}
		}
		else
		{
			if (warp_audiosource_ptr != null && warp_audiosource_ptr.isPlaying)
			{
				warp_audiosource_ptr.Stop();
				warp_audiosource_ptr = null;
			}
		}

		float distance_viz = Vector3.Distance(spec_viz_reticle.transform.position, cam_reticle.transform.position);
		float distance_gam = Vector3.Distance(spec_gam_reticle.transform.position, cam_reticle.transform.position);
		float distance_neu = Vector3.Distance(spec_neu_reticle.transform.position, cam_reticle.transform.position);

		if (distance_viz < distance_gam && distance_viz < distance_neu) 
        {
            spec_trigger.position = spec_viz_reticle.transform.position;
        }

		if (distance_gam < distance_viz && distance_gam < distance_neu) 
        {
            spec_trigger.position = spec_gam_reticle.transform.position;
        }

		if (distance_neu < distance_gam && distance_neu < distance_viz) 
        {
            spec_trigger.position = spec_neu_reticle.transform.position;
        }

		if (cur_scene_i == (int)SCENE.VOYAGER && !voiceovers_played[cur_scene_i, (int)SPEC.GAM]) 
        {
            spec_trigger.position = spec_gam_reticle.transform.position; //ensure you can only select gamma to start voyager selection
        }

		//IF !ICE scene
		if (cur_scene_i != (int)SCENE.ICE && cur_scene_i != (int)SCENE.EARTH)
		{
			if (!(cur_scene_i == (int)SCENE.VOYAGER && voiceover_was_playing) && !advance_paused && spec_projection.activeSelf && spec_trigger.tick(cam_reticle.transform.position, Time.deltaTime))
			{
				if (spec_trigger.just_triggered)
				{
					int old_spec = cur_spec_i;
					if (distance_gam <= distance_viz && distance_gam <= distance_neu) SetSpec((int)SPEC.GAM);
					if (distance_viz <= distance_gam && distance_viz <= distance_neu) SetSpec((int)SPEC.VIZ);
					if (distance_neu <= distance_gam && distance_neu <= distance_viz) SetSpec((int)SPEC.NEU);

					//spec switched
					if (old_spec != cur_spec_i)
					{
                        //IceCubeAnalytics.Instance.LogObjectSelected("spec__reticle", spec_neu_reticle.transform.position, spec_neu_reticle.transform.rotation, ((SCENE)cur_scene_i).ToString());
                        //Debug.Log("Object selected spec neu reticle");
						
                        PlaySFX(SFX.SELECT);
						if (!voiceovers_played[cur_scene_i, cur_spec_i] && dumb_delay_t > dumb_delay_t_max)
						{
							if (voiceover_audiosource.isPlaying || espAudio.isPlaying || porAudio.isPlaying)
							{
								voiceover_audiosource.Stop();
								espAudio.Stop();
								porAudio.Stop();
							}

                            if(old_spec == (int)SPEC.VIZ)
                            {
                                if(subtitle_strings[audio_started_scene, audio_started_spec, audio_started_subtitle].Length > 0)
                                {
                                    IceCubeAnalytics.Instance.LogAudioStarted(subtitle_strings[audio_started_scene, audio_started_spec, audio_started_subtitle], ((SCENE)cur_scene_i).ToString());
                                    //Debug.Log(subtitle_strings[audio_started_scene, audio_started_spec, audio_started_subtitle] + " completed");
                                }
                            }
                            else if(old_spec == (int)SPEC.GAM)
                            {
                                if((audio_started_subtitle-1) > 0)
                                {
                                    IceCubeAnalytics.Instance.LogAudioStarted(subtitle_strings[audio_started_scene, audio_started_spec, audio_started_subtitle-1], ((SCENE)cur_scene_i).ToString());
                                    //Debug.Log(subtitle_strings[audio_started_scene, audio_started_spec, audio_started_subtitle-1] + " completed");
                                }
                            }

							Language(cur_scene_i, cur_spec_i);
							//Debug.Log("vPlay03"); 
							voiceover_audiosource.Play();
							espAudio.Play();
							porAudio.Play();

							voiceover_was_playing = true;
							voiceovers_played[cur_scene_i, cur_spec_i] = true;
							subtitle_i = 0;
							subtitle_t = 0;
							subtitles_text.text = string.Empty;
							subtitle_spec = cur_spec_i;
                            if(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i+1].Length > 0)
							{
								IceCubeAnalytics.Instance.LogAudioStarted(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i+1], ((SCENE)cur_scene_i).ToString());
								//Debug.Log(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i+1] + " started");
                                audio_started_spec = subtitle_spec;
                                audio_started_scene = cur_scene_i;
                                audio_started_subtitle = subtitle_i;
							}
						}

						float old_time = music_audiosource.time;
						if (music_audiosource.isPlaying)
						{
							old_time = music_audiosource.time;
							music_audiosource.Stop();
						}
						music_audiosource.clip = GetMusicClip(cur_scene_i, cur_spec_i);
						music_audiosource.volume = music_vols[cur_scene_i, cur_spec_i];
						music_audiosource.time = old_time;
						music_audiosource.Play();
						music_was_playing = true;
					}
				}
			}
		}


		scene_rots[cur_scene_i] += scene_rot_deltas[cur_scene_i] * Time.deltaTime;
		while (scene_rots[cur_scene_i] > 3.14159265f * 2.0f) scene_rots[cur_scene_i] -= (3.14159265f * 2.0f);
		scene_rots[next_scene_i] += scene_rot_deltas[next_scene_i] * Time.deltaTime;
		while (scene_rots[next_scene_i] > 3.14159265f * 2.0f) scene_rots[next_scene_i] -= (3.14159265f * 2.0f);

		scene_groups[cur_scene_i, cur_spec_i].transform.position = new Vector3(0, 0, 0);
		scene_groups[cur_scene_i, cur_spec_i].transform.rotation = Quaternion.Euler(0f, 0f, 0f);

		scene_groups[cur_scene_i, cur_spec_i].transform.Translate(scene_centers[cur_scene_i]);
		scene_groups[cur_scene_i, cur_spec_i].transform.Rotate(0f, Mathf.Rad2Deg * scene_rots[cur_scene_i], 0f);
		scene_groups[cur_scene_i, cur_spec_i].transform.Translate(-scene_centers[cur_scene_i]);
		main_camera_skybox.material.SetFloat("_Rotation", -Mathf.Rad2Deg * scene_rots[cur_scene_i]);

		time_mod_twelve_pi = (time_mod_twelve_pi + Time.deltaTime) % (12.0f * 3.1415926535f);
		Shader.SetGlobalFloat(time_mod_twelve_pi_id, time_mod_twelve_pi);
		jitter_countdown -= Time.deltaTime;
		if (jitter_countdown <= 0.0f)
		{
			if (jitter_state == 1)
			{
				jitter_state = 0;
				jitter_countdown = Random.Range(jitter_min_downtime, jitter_max_downtime);
			}
			else
			{
				jitter_state = 1;
				jitter_countdown = Random.Range(jitter_min_uptime, jitter_max_uptime);
			}
		}
		
		if (jitter_state == 1 && cur_scene_i == (int)SCENE.EXTREME)
			jitter += Random.Range(-0.1f, 0.1f);
		else
			jitter = 0;
		
		Shader.SetGlobalFloat(jitter_id, jitter);

		//voiceover finished katherine
		if (voiceover_was_playing)
		{
			if (!LocalizationManager.instance.spanish && !LocalizationManager.instance.portuguese)
			{
				if (!HeadsetPaused && !advance_paused && !voiceover_audiosource.isPlaying)
				{
					voiceover_was_playing = false;
					
					bool play_end = !voiceovers_played[cur_scene_i, (int)SPEC.COUNT];
					
					if (cur_scene_i == (int)SCENE.EXTREME) play_end = false;
					for (int i = 0; play_end && i < (int)SPEC.COUNT; i++)
					{
						if (!voiceovers_played[cur_scene_i, i]) play_end = false;
					}
					
					if (play_end)
					{
						//if(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i].Length > 0)
						{
							IceCubeAnalytics.Instance.LogAudioComplete(subtitle_strings[audio_started_scene, audio_started_spec, audio_started_subtitle], ((SCENE)cur_scene_i).ToString());
							//Debug.Log(subtitle_strings[audio_started_scene, audio_started_spec, audio_started_subtitle] + " complete");
						}
						
						Language(cur_scene_i, (int)SPEC.COUNT);
						//Debug.Log("vPlay04");
						voiceover_audiosource.Play();
						voiceover_was_playing = true;
						voiceovers_played[cur_scene_i, (int)SPEC.COUNT] = true;
						subtitle_i = 0;
						subtitle_t = 0;
						subtitles_text.text = string.Empty;
						subtitle_spec = (int)SPEC.COUNT;
						// prompt warp
					}
				}
			}

			if (LocalizationManager.instance.spanish)
			{
				if (!HeadsetPaused && !advance_paused && !espAudio.isPlaying)
				{
					voiceover_was_playing = false;
					bool play_end = !voiceovers_played[cur_scene_i, (int)SPEC.COUNT];
					if (cur_scene_i == (int)SCENE.EXTREME) play_end = false;
					for (int i = 0; play_end && i < (int)SPEC.COUNT; i++)
					{
						if (!voiceovers_played[cur_scene_i, i]) play_end = false;
					}
					
					if (play_end)
					{
						//if(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i].Length > 0)
						{
							IceCubeAnalytics.Instance.LogAudioComplete(subtitle_strings[audio_started_scene, audio_started_spec, audio_started_subtitle], ((SCENE)cur_scene_i).ToString());
							//Debug.Log(subtitle_strings[audio_started_scene, audio_started_spec, audio_started_subtitle] + " complete");
						}
						Language(cur_scene_i, (int)SPEC.COUNT);
						//Debug.Log("vPlay05555555");
						voiceover_audiosource.Play();
						voiceover_was_playing = true;
						voiceovers_played[cur_scene_i, (int)SPEC.COUNT] = true;
						subtitle_i = 0;
						subtitle_t = 0;
						subtitles_text.text = string.Empty;
						subtitle_spec = (int)SPEC.COUNT;
						// prompt warp
					}
				}
			}

			if (LocalizationManager.instance.portuguese)
			{
				if (!HeadsetPaused && !advance_paused && !porAudio.isPlaying)
				{
					voiceover_was_playing = false;
					bool play_end = !voiceovers_played[cur_scene_i, (int)SPEC.COUNT];
					if (cur_scene_i == (int)SCENE.EXTREME) play_end = false;
					for (int i = 0; play_end && i < (int)SPEC.COUNT; i++)
					{
						if (!voiceovers_played[cur_scene_i, i]) play_end = false;
					}
					if (play_end)
					{
						//if(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i].Length > 0)
						{
							IceCubeAnalytics.Instance.LogAudioComplete(subtitle_strings[audio_started_scene, audio_started_spec, audio_started_subtitle], ((SCENE)cur_scene_i).ToString());
							//Debug.Log(subtitle_strings[audio_started_scene, audio_started_spec, audio_started_subtitle] + " complete");
						}
						Language(cur_scene_i, (int)SPEC.COUNT);
						//Debug.Log("vPlay06");
						voiceover_audiosource.Play();
						voiceover_was_playing = true;
						voiceovers_played[cur_scene_i, (int)SPEC.COUNT] = true;
						subtitle_i = 0;
						subtitle_t = 0;
						subtitles_text.text = string.Empty;
						subtitle_spec = (int)SPEC.COUNT;
						// allows prompts warp
					}
				}
			}

			float ball_t = (nwave_t_10 % 2f) / 2f;
			gazeball.transform.position = Vector3.Lerp(gaze_pt, anti_gaze_pt, ball_t);
			Vector3 ball_pos = gazeball.transform.position;
	
			MapVols();
		}
		else
		{
			if (dumb_delay_t < dumb_delay_t_max) //delays audio
			{

				if (hmd_mounted && language_selected) dumb_delay_t += Time.deltaTime;

				if (dumb_delay_t >= dumb_delay_t_max && language_selected) // newly done with delay
				{
					Language(cur_scene_i, cur_spec_i);
					voiceover_audiosource.Play();
					espAudio.Play();
					porAudio.Play();
					voiceover_was_playing = true;
					voiceovers_played[cur_scene_i, cur_spec_i] = true;
					subtitle_i = 0;
					subtitle_t = 0;
					subtitles_text.text = string.Empty;
					subtitle_spec = cur_spec_i;
					if(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i+1].Length > 0)
					{
						IceCubeAnalytics.Instance.LogAudioStarted(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i+1], ((SCENE)cur_scene_i).ToString());
						//Debug.Log(subtitle_strings[cur_scene_i, subtitle_spec, subtitle_i+1] + " started");
                        audio_started_spec = subtitle_spec;
                        audio_started_scene = cur_scene_i;
                        audio_started_subtitle = subtitle_i+1;
					}
				}
			}

			if (music_was_playing)
			{
				if (!HeadsetPaused && !music_audiosource.isPlaying)
				{
					music_audiosource.clip = GetMusicClip(cur_scene_i, cur_spec_i);
					music_audiosource.volume = music_vols[cur_scene_i, cur_spec_i];
					music_audiosource.Play();
					music_was_playing = true;
				}
			}

			float ball_t = (nwave_t_10 % 2f) / 2f;
			gazeball.transform.position = Vector3.Lerp(gaze_pt, anti_gaze_pt, ball_t);
			Vector3 ball_pos = gazeball.transform.position;

			MapVols();
		}
	}
}


