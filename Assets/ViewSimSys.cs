#define SRP

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using System;
using System.IO;


using UnityEngine;
using UnityEditor;

using Firebase.Storage;
using ViewSim;

public class ViewSimSys : MonoBehaviour
{
    public ComputeShader shader;
    int handleCapture, handleCompare, handleTexture, handleShow, handleUColor;

    public int maxCompareFrames = 24;
    public float samplingInterval = .1f;

    float previousSampleTime=0;

    public ComputeBuffer simBuffer;
    public uint[] simMatrix=null;

    public ComputeBuffer pairBuffer;
    public uint[] pairValue=null;

    public bool captureViews = true;

    public RenderTexture symTexture;

    public GameObject viewpointObject;

    AsyncGPUReadbackRequest request;
    public Vector2Int captureSize= new Vector2Int(128,128); 
    float invScreenSize;

    public string CameraLog;

    public Viewpoint testView;
    
    //public ViewSim.ListBuffer<Viewpoint> viewpoints;

    public ViewSim.ListBuffer<Viewpoint> viewpointsL1;
    public ViewSim.ListBuffer<Viewpoint> viewpointsL2;
    public ViewSim.ListBuffer<Viewpoint> viewpointsL3;


    public float viewThreshold = .75f;

    public bool showComparison;

    [Serializable]
    public class Viewpoint
    {
        public RenderTexture colorTexture, depthTexture;
        public Matrix4x4 mvp, inv;
        public Vector3 position;
        public Quaternion rotation;
        public float time;
        public string name;
        public int width;
        public int height;

       public Viewpoint()
       {


       }


       public Viewpoint(Viewpoint v, string name = "Viewpoint")
       {
            mvp = v.mvp;
            inv = v.inv;
            position = v.position;
            rotation = v.rotation;
            time = v.time;

            init(v.colorTexture.width, v.colorTexture.height, name);

            Graphics.Blit(v.colorTexture, colorTexture);
            Graphics.Blit(v.depthTexture, depthTexture);
       }

        public string SaveToString()
        {
            return JsonUtility.ToJson(this);
        }


        public void init()
        {
            if (colorTexture == null)
            {
                colorTexture = new RenderTexture(width, height, 0);
                colorTexture.name = name + "_Color";

                colorTexture.enableRandomWrite = true;
                colorTexture.filterMode = FilterMode.Point;
                colorTexture.format = RenderTextureFormat.ARGB32;
                colorTexture.useMipMap = false;
                colorTexture.Create();
            }

            if (depthTexture == null)
            {
                depthTexture = new RenderTexture(width, height, 0);
                depthTexture.name = name + "_Depth";

                depthTexture.enableRandomWrite = true;
                depthTexture.filterMode = FilterMode.Point;
                depthTexture.format = RenderTextureFormat.RFloat;
                depthTexture.useMipMap = false;
                depthTexture.Create();
            }

        }


        public void init(int width, int height, string name = "Viewpoint")
        {
            this.name = name;
            this.width = width;
            this.height = height;

            init();

           
        }

        public void set(Camera cam)
        { 
            mvp = cam.projectionMatrix * cam.worldToCameraMatrix;
            inv = cam.worldToCameraMatrix.inverse * cam.projectionMatrix.inverse;
            time = Time.time;
            rotation = cam.transform.rotation;
            position = cam.transform.position;
        }
    };



    void storeSession(ViewSim.ListBuffer<Viewpoint> viewpoints)
    {
        foreach (Viewpoint viewpoint in viewpoints)
        {
			
#if UNITY_ANDROID
            SaveRenderTexture(viewpoint.colorTexture, UnityEngine.Application.persistentDataPath+"/Images/");
            SaveRenderTextureBinary(viewpoint.depthTexture, UnityEngine.Application.persistentDataPath+"/Images/");
            System.IO.File.WriteAllText(UnityEngine.Application.persistentDataPath+"/Images/" + viewpoint.name + ".json", viewpoint.SaveToString());
#else
            SaveRenderTexture(viewpoint.colorTexture, "Images/");
            SaveRenderTextureBinary(viewpoint.depthTexture, "Images/");
            System.IO.File.WriteAllText("Images/" + viewpoint.name + ".json", viewpoint.SaveToString());
#endif
            session.Add(viewpoint);
        }

    }

    public List<Viewpoint> session = new List<Viewpoint>();
    public Texture2D colorTexture = null;
    public Texture2D depthTexture = null;
   
    static Firebase.Storage.FirebaseStorage _storage;
   
    void loadSession()
    {
        int index = 0;
		
#if UNITY_ANDROID
		string path = UnityEngine.Application.persistentDataPath+"/Images/v" + index + ".json";
#else
        string path = "Images/v" + index + ".json";
#endif

        StreamReader reader = new StreamReader(path);



        while (File.Exists(path))
        {
            string jsonString = File.ReadAllText(path);
            Viewpoint v = JsonUtility.FromJson<Viewpoint>(jsonString);

            v.init();

            Debug.Log("time " + v.time);

            if (colorTexture == null)
            {
                colorTexture = new Texture2D(v.width, v.height);
            }

            //color image path
#if UNITY_ANDROID
			path = UnityEngine.Application.persistentDataPath+"/Images/v" + index + "_Color.png";
#else
            path = "Images/v" + index + "_Color.png";
#endif
            colorTexture.LoadImage(File.ReadAllBytes(path));
            //copy it over?
            Graphics.Blit(colorTexture, v.colorTexture);


            if (depthTexture == null)
            {
                depthTexture = new Texture2D(v.width, v.height, TextureFormat.RFloat, false);
            }

            //depth image path
#if UNITY_ANDROID
			path = UnityEngine.Application.persistentDataPath+"/Images/v" + index + "_Depth.bin";
#else
            path = "Images/v" + index + "_Depth.bin";
#endif
            depthTexture.LoadRawTextureData(File.ReadAllBytes(path));
            depthTexture.Apply();

            //depthTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            //copy it over
            Graphics.Blit(depthTexture, v.depthTexture);

            session.Add(v);


            CreateGameObjectFromViewpoint(v);

            //go to the next index
            index++;
            path = "Images/v" + index + ".json";
        }

        //for testing, convert it to a listbuffer
        ViewSim.ListBuffer<Viewpoint> sessionViewpoints = new ViewSim.ListBuffer<Viewpoint>(session.Count);
        //TODO make this better
        foreach (Viewpoint v in session)
            sessionViewpoints.Add(v);

        CreateViewPointMatrix(sessionViewpoints);

    }


    public void CreateGameObjectFromViewpoint(Viewpoint v)
    {
        GameObject g = Instantiate(viewpointObject, v.position, v.rotation);
        Material m = g.GetComponent<Renderer>().material;
        m.mainTexture = v.colorTexture;
    }




    public Vector3Int getComputeParams(RenderTexture texture)
    {
        return new Vector3Int((texture.width + 7) / 8, (texture.height + 7) / 8, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        handleCapture = shader.FindKernel("CSCapture");
        handleCompare = shader.FindKernel("CSCompare");
        handleTexture = shader.FindKernel("CSTexture");
        handleShow = shader.FindKernel("CSShow");
        handleUColor = shader.FindKernel("CSUColor");

#if UNITY_ANDROID
		DirectoryInfo di = new DirectoryInfo(UnityEngine.Application.persistentDataPath+"/Images/");
		if(!di.Exists)
		{
			di.Create();
		}
#else
		DirectoryInfo di = new DirectoryInfo("/Images/");
		if(!di.Exists)
		{
			di.Create();
		}
#endif

        int count = maxCompareFrames * maxCompareFrames;

        simBuffer = new ComputeBuffer(count, 4);
        simMatrix = new uint[count];

        ClearSimMatrix();

        //just compare 2 values
        pairBuffer = new ComputeBuffer(2, 4);
        pairValue = new uint[2];

		

        invScreenSize = 1f / (float)(captureSize.x * captureSize.y);
        viewpointsL1 = new ViewSim.ListBuffer<Viewpoint>(maxCompareFrames);
        viewpointsL2 = new ViewSim.ListBuffer<Viewpoint>(maxCompareFrames);
        viewpointsL3 = new ViewSim.ListBuffer<Viewpoint>(maxCompareFrames);


        //init the frames
        for (int i = 0; i < maxCompareFrames; i++)
        {
            Viewpoint viewpoint = new Viewpoint();
            viewpoint.init(captureSize.x, captureSize.y, "view-" + viewpointsL1.Count);
            viewpointsL1.Add(viewpoint);
        }

        //clear it out
        viewpointsL1.Clear();
        viewpointsL1.IncrementBackPtr();


        testView = viewpointsL1[0];

        //start coroutine
        StartCoroutine(ProcessGPURequests());

        // if (!captureViews)
        //    loadSession();




        //  StoreCaptureTT("capture.bin");
        //  LoadReplayTT("capture.bin");
        
        Camera.onPostRender += MyPostRender;
        Camera.main.depthTextureMode = DepthTextureMode.Depth;

		_storage = FirebaseStorage.DefaultInstance;
    }


    float computeSim(int indexA=0, int indexB=1)
    {
        return Mathf.Min(pairValue[indexA] * invScreenSize, pairValue[indexA] * invScreenSize);
       // return viewSimWeight * (simMatrix[indexA] * invScreenSize) + (1f- viewSimWeight)* (simMatrix[indexB] * invScreenSize);
    }

    static void SaveRenderTexture(RenderTexture rt, string dir)
    {
        string path = dir + rt.name + ".png";
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;
        var bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        Debug.Log($"Saved texture: {rt.width}x{rt.height} - " + path);
		
		/*StorageReference storageRef = ViewSimSys._storage.GetReferenceFromUrl("gs://icecubevr-a0510.appspot.com");
		StorageReference imageRef = storageRef.Child("images");
		StorageReference testPng = imageRef.Child("test.png");
		Stream stream = new FileStream(path, FileMode.Open);
		testPng.PutStreamAsync(stream).ContinueWith((System.Threading.Tasks.Task<StorageMetadata> task) => {
			if (task.IsFaulted || task.IsCanceled) {
				Debug.Log(task.Exception.ToString());
				// Uh-oh, an error occurred!
			}
			else {
				// Metadata contains file metadata such as size, content-type, and download URL.
				StorageMetadata metadata = task.Result;
				string md5Hash = metadata.Md5Hash;
				Debug.Log("Finished uploading...");
				Debug.Log("md5 hash = " + md5Hash);
			}
		});*/
    }

    static void SaveRenderTextureBinary(RenderTexture rt, string dir)
    {
        string path = dir + rt.name + ".bin";
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RFloat, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;
        var bytes = tex.GetRawTextureData();
        System.IO.File.WriteAllBytes(path, bytes);
        Debug.Log($"Saved texture: {rt.width}x{rt.height} - " + path);
		
		/*StorageReference storageRef = ViewSimSys._storage.GetReferenceFromUrl("gs://icecubevr-a0510.appspot.com");
		StorageReference imageRef = storageRef.Child("bin_images");
		StorageReference testPng = imageRef.Child("test.bin");
		Stream stream = new FileStream(path, FileMode.Open);
		testPng.PutStreamAsync(stream).ContinueWith((System.Threading.Tasks.Task<StorageMetadata> task) => {
			if (task.IsFaulted || task.IsCanceled) {
				Debug.Log(task.Exception.ToString());
				// Uh-oh, an error occurred!
			}
			else {
				// Metadata contains file metadata such as size, content-type, and download URL.
				StorageMetadata metadata = task.Result;
				string md5Hash = metadata.Md5Hash;
				Debug.Log("Finished uploading...");
				Debug.Log("md5 hash = " + md5Hash);
			}
		});*/
    }


    static void SaveRenderTextureEXR(RenderTexture rt, string dir)
    {
        string path = dir + rt.name + ".exr";
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RFloat, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;
        var bytes = tex.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
        System.IO.File.WriteAllBytes(path, bytes);
        Debug.Log($"Saved texture: {rt.width}x{rt.height} - " + path);
		
		/*StorageReference storageRef = ViewSimSys._storage.GetReferenceFromUrl("gs://icecubevr-a0510.appspot.com");
		StorageReference imageRef = storageRef.Child("exr_images");
		StorageReference testPng = imageRef.Child("test.exr");
		Stream stream = new FileStream(path, FileMode.Open);
		testPng.PutStreamAsync(stream).ContinueWith((System.Threading.Tasks.Task<StorageMetadata> task) => {
			if (task.IsFaulted || task.IsCanceled) {
				Debug.Log(task.Exception.ToString());
				// Uh-oh, an error occurred!
			}
			else {
				// Metadata contains file metadata such as size, content-type, and download URL.
				StorageMetadata metadata = task.Result;
				string md5Hash = metadata.Md5Hash;
				Debug.Log("Finished uploading...");
				Debug.Log("md5 hash = " + md5Hash);
			}
		});*/
    }


    int getBestViewpointIndex(ViewSim.ListBuffer<Viewpoint> viewpoints)
    {
        CreateViewPointMatrix(viewpoints);

        int count= viewpoints.Count;
        uint frameValue;
        uint maxValue=0;
        int maxIndex=0;
        for (int y = 0; y < viewpoints.Count; y++)
        {
            frameValue = 0;
            for (int x = 0; x < viewpoints.Count; x++)
            {
                int xindex = x + y * maxCompareFrames;
                int yindex = y + x * maxCompareFrames;
                frameValue += Math.Min(simMatrix[xindex], simMatrix[yindex]);
            }

            if (frameValue >= maxValue)
            {
                maxIndex = y;
                maxValue = frameValue;
            }
        }

        Debug.Log("Best Index " + maxIndex);

        //Debug.Log(simMatrix);

               // int index = viewpoints.Count/2;
        //TODO optimize this
        return maxIndex;
    }



    int ExtractAndAdd(ViewSim.ListBuffer<Viewpoint> SourceViewpoints, ViewSim.ListBuffer<Viewpoint> DestinationViewpoints)
    {
        int bestIndex = getBestViewpointIndex(SourceViewpoints);
        Viewpoint v = SourceViewpoints[bestIndex];

        DestinationViewpoints.Add(new Viewpoint(v, "v"+ DestinationViewpoints.Count));

        return bestIndex;
       // viewpoints.Clear();
//viewpoints.IncrementBackPtr();

        //lets remove the front up until this point
      // while (viewpoints.Count > 0)
      //  {
      //      if (v == viewpoints.Front())
      //      {
      //          break;
      //      }
      //      else
      //      {
      //          viewpoints.PopFront();
      //      }
      //
      //  }
      //
       // Debug.Log("Frames left " + SourceViewpoints.Count);

       
    }

    void CheckViews(Viewpoint va, Viewpoint vb)
    {
        ClearPairValue();

        Compare(va, vb, pairBuffer, 0);
        Compare(vb, va, pairBuffer, 1);

        if (request.done)
            request = AsyncGPUReadback.Request(pairBuffer);

    }



    void writeDebugData(ViewSim.ListBuffer<Viewpoint> viewpoints)
    {
        foreach (Viewpoint viewpoint in viewpoints)
        {
            CreateGameObjectFromViewpoint(viewpoint);
#if UNITY_ANDROID
			SaveRenderTexture(viewpoint.colorTexture, UnityEngine.Application.persistentDataPath+"/Images/");
#else
            SaveRenderTexture(viewpoint.colorTexture, "Images/");
#endif
        }
		
#if UNITY_ANDROID
		SaveRenderTexture(symTexture, UnityEngine.Application.persistentDataPath+"/Images/");
#else
        SaveRenderTexture(symTexture, "Images/");
#endif

#if UNITY_ANDROID
		StreamWriter writer = new StreamWriter(UnityEngine.Application.persistentDataPath+"/Images/mat.csv");
#else
        StreamWriter writer = new StreamWriter("Images/mat.csv");
#endif

        for (int y = 0; y < maxCompareFrames; y++) 
        {
            for (int x = 0; x < maxCompareFrames; x++)
            {
                int index = x + y * maxCompareFrames;
                writer.Write(simMatrix[index] + ",");
            }
            writer.Write("\n");
        }

        writer.Close();
		
		StorageReference storageRef = ViewSimSys._storage.GetReferenceFromUrl("gs://icecubevr-a0510.appspot.com");
		StorageReference folderRef = storageRef.Child("transforms");
		StorageReference fileToUpload = folderRef.Child("mat.csv");
		
		Stream stream = new FileStream(UnityEngine.Application.persistentDataPath+"/Images/mat.csv", FileMode.Open);
		
		fileToUpload.PutStreamAsync(stream).ContinueWith((System.Threading.Tasks.Task<StorageMetadata> task) => {
			if (task.IsFaulted || task.IsCanceled) {
				Debug.Log(task.Exception.ToString());
				// Uh-oh, an error occurred!
			}
			else {
				// Metadata contains file metadata such as size, content-type, and download URL.
				StorageMetadata metadata = task.Result;
				string md5Hash = metadata.Md5Hash;
				Debug.Log("Finished uploading...");
				Debug.Log("md5 hash = " + md5Hash);
			}
		});

    }

    IEnumerator ProcessGPURequests()
    {


        while (true)
        {

            yield return new WaitForEndOfFrame();

            if ((viewpointsL1.Count >= 2) && (request.done))
            {
                if (captureViews)
                {
                    CheckViews(viewpointsL1.Front(), viewpointsL1.Back());
                }

                yield return new WaitUntil(() => request.done);

                if (!request.hasError)
                {
                    pairValue = request.GetData<uint>().ToArray();
                    float sim = computeSim();
//                    Debug.Log("V1 " + sim);
                    if ((sim < viewThreshold) || (viewpointsL1.Count > maxCompareFrames - 1))
                    {



                        int bestIndex = ExtractAndAdd(viewpointsL1, viewpointsL2);

                        //FOR TESTING
                      //  writeDebugData(viewpointsL1);
                      //  captureViews = false;


                        //now reset this
                        viewpointsL1.ClearToIndex(bestIndex);
                        viewpointsL1.IncrementBackPtr();


                        //now run this on L2
                        CheckViews(viewpointsL2.Front(), viewpointsL2.Back());
                        yield return new WaitUntil(() => request.done);

                        if (!request.hasError)
                        {
                            pairValue = request.GetData<uint>().ToArray();
                            sim = computeSim();
                            Debug.Log("V2 " + sim);
                            if ((sim < viewThreshold) || (viewpointsL2.Count >= maxCompareFrames - 1))
                            {
                                ExtractAndAdd(viewpointsL2, viewpointsL3);

                                //now clear out L2
                                viewpointsL2.Clear();

                                if (viewpointsL3.Count >= maxCompareFrames)
                                {
                                    //show our captured viewpoints
                                    storeSession(viewpointsL3);
                                    // foreach (Viewpoint viewpoint in viewpointsL3)
                                    // {
                                    //     CreateGameObjectFromViewpoint(viewpoint);
                                    //     SaveRenderTexture(viewpoint.colorTexture, "Images/");
                                    // }
                                    captureViews = false;
                                }
                            }
                        }
                    }
                }
            
            }
        }
    }


    // Update is called once per frame
    void LateUpdate()
    {


       // if (viewpoints.Count == 2)
       //     request = AsyncGPUReadback.Request(simBuffer);

     // if ((viewpoints.Count >= 2) && (request.done))
     // {
     //
     //     if (!request.hasError)
     //     {
     //         simMatrix = request.GetData<uint>().ToArray();
     //         float sim = computeSim();
     //         Debug.Log(sim);
     //         if (sim < .5)
     //         {
     //             ExtractAndReset();
     //         }
     //     }
     //     if (captureViews)
     //     {
     //         CheckViews(viewpoints.Front(), viewpoints.Back());
     //     }
     // }
    }

    public void CaptureView(Camera cam, Viewpoint v)
    {

        //initialize the viewpoint
        v.set(cam);

        //set the values
        shader.SetInt("SourceWidth", cam.pixelWidth);
        shader.SetInt("SourceHeight", cam.pixelHeight);



            shader.SetTexture(handleCapture, "SourceColor", RenderTexture.active);
       // shader.SetTextureFromGlobal(handleCapture, "SourceColor", "_CameraColorTexture");
        shader.SetTextureFromGlobal(handleCapture, "SourceDepth", "_CameraDepthTexture");

        shader.SetInt("DestinationWidth", v.colorTexture.width);
        shader.SetInt("DestinationHeight", v.colorTexture.height);

        shader.SetTexture(handleCapture, "DestinationColor", v.colorTexture);
        shader.SetTexture(handleCapture, "DestinationDepth", v.depthTexture);

        //run the shader
        Vector3Int execParam = getComputeParams(v.colorTexture);
        shader.Dispatch(handleCapture, execParam.x, execParam.y, execParam.z);
    }

    public void Compare(Viewpoint va, Viewpoint vb, ComputeBuffer simBuffer, int index=-1)
    {


        shader.SetInt("SourceWidth", va.colorTexture.width);
        shader.SetInt("SourceHeight", va.colorTexture.height);

        shader.SetTexture(handleCompare, "SourceColor", va.colorTexture);
        shader.SetTexture(handleCompare, "SourceDepth", va.depthTexture);

        shader.SetInt("DestinationWidth", vb.colorTexture.width);
        shader.SetInt("DestinationHeight", vb.colorTexture.height);

        shader.SetTexture(handleCompare, "DestinationColor", vb.colorTexture);
        shader.SetTexture(handleCompare, "DestinationDepth", vb.depthTexture);

        // Matrix4x4 mat = cam.projectionMatrix * cam.worldToCameraMatrix;

        shader.SetMatrix("inv", vb.inv);
        shader.SetMatrix("mat", va.mvp);


        shader.SetInt("simBufferIndex", index);
        shader.SetBuffer(handleCompare, "simBuffer", simBuffer);

        Vector3Int execParam = getComputeParams(vb.colorTexture);
        shader.Dispatch(handleCompare, execParam.x, execParam.y, execParam.z);

    }

    public void ShowDifference(Viewpoint va, Viewpoint vb, int index = -1)
    {


        shader.SetInt("SourceWidth", va.colorTexture.width);
        shader.SetInt("SourceHeight", va.colorTexture.height);

        shader.SetTexture(handleShow, "SourceColor", va.colorTexture);
        shader.SetTexture(handleShow, "SourceDepth", va.depthTexture);

        shader.SetInt("DestinationWidth", vb.colorTexture.width);
        shader.SetInt("DestinationHeight", vb.colorTexture.height);

        shader.SetTexture(handleShow, "DestinationColor", vb.colorTexture);
        shader.SetTexture(handleShow, "DestinationDepth", vb.depthTexture);

        // Matrix4x4 mat = cam.projectionMatrix * cam.worldToCameraMatrix;

        shader.SetMatrix("inv", vb.inv);
        shader.SetMatrix("mat", va.mvp);


        shader.SetInt("simBufferIndex", index);
        shader.SetBuffer(handleShow, "simBuffer", pairBuffer);

        Vector3Int execParam = getComputeParams(vb.colorTexture);
        shader.Dispatch(handleShow, execParam.x, execParam.y, execParam.z);

    }



    public void ConvertFloatToRGBA(RenderTexture floatTexture, RenderTexture colorTexture)
    {

        shader.SetInt("SourceWidth", floatTexture.width);
        shader.SetInt("SourceHeight", floatTexture.height);

        shader.SetTexture(handleUColor, "SourceColor", floatTexture);
 

        shader.SetInt("DestinationWidth", colorTexture.width);
        shader.SetInt("DestinationHeight", colorTexture.height);

        shader.SetTexture(handleUColor, "UColor", colorTexture);


        Vector3Int execParam = getComputeParams(colorTexture);
        shader.Dispatch(handleUColor, execParam.x, execParam.y, execParam.z);

    }

    void ClearPairValue()
    {
        //zero out the data
        //TODO do this in the shader?
        pairValue[0] = 0;
        pairValue[1] = 1;

        pairBuffer.SetData(pairValue);
    }

    void ClearSimMatrix()
    {
        if (simMatrix == null)
            return;
        //zero out the data
        //TODO do this in the shader?
        int count = maxCompareFrames * maxCompareFrames;

       // Debug.Log("sim lenght " + simMatrix.Length);
        for (int i = 0; i < count; i++)
        {
            simMatrix[i] = 0;
//            Debug.Log(i);
        }

        simBuffer.SetData(simMatrix);
    }
 

    void CreateViewPointMatrix(ViewSim.ListBuffer<Viewpoint> viewpoints)
    {


        ClearSimMatrix();

//        Debug.Log("compare views");
        for (int x = 0; x < viewpoints.Count; x++)
            for (int y = 0; y < viewpoints.Count; y++)
                Compare(viewpoints[x], viewpoints[y], simBuffer, x+ maxCompareFrames*y);

        if (symTexture == null)
        {
           symTexture = new RenderTexture(maxCompareFrames, maxCompareFrames, 0);
           symTexture.name = name + "_sym";
          
           symTexture.enableRandomWrite = true;
           symTexture.filterMode = FilterMode.Point;
           symTexture.format = RenderTextureFormat.ARGB32;
           symTexture.useMipMap = false;
           symTexture.Create();

        }

        Viewpoint v = viewpoints[0];

        shader.SetInt("SourceWidth", v.colorTexture.width);
        shader.SetInt("SourceHeight", v.colorTexture.height);

       // shader.SetTexture(handleCapture, "SourceColor", v.colorTexture);
       // shader.SetTexture(handleCapture, "SourceDepth", v.depthTexture);


        shader.SetInt("DestinationWidth", symTexture.width);
        shader.SetInt("DestinationHeight", symTexture.height);

        shader.SetTexture(handleTexture, "DestinationColor", symTexture);
        // shader.SetTexture(handleCapture, "DestinationDepth", v.depthTexture);

        shader.SetBuffer(handleTexture, "simBuffer", simBuffer);

        //run the shader
        Vector3Int execParam = getComputeParams(symTexture);
        shader.Dispatch(handleTexture, execParam.x, execParam.y, execParam.z);

        //get the data
        simBuffer.GetData(simMatrix);

#if UNITY_ANDROID
		StreamWriter writer = new StreamWriter(UnityEngine.Application.persistentDataPath+"/Images/mat.csv");
#else
        StreamWriter writer = new StreamWriter("Images/mat.csv");
#endif

        for (int y = 0; y < maxCompareFrames; y++)
        {
            for (int x = 0; x < maxCompareFrames; x++)
            {
                int index = x + y * maxCompareFrames;
                writer.Write(simMatrix[index] + ",");
            }
            writer.Write("\n");
        }

        writer.Close();
		
		StorageReference storageRef = ViewSimSys._storage.GetReferenceFromUrl("gs://icecubevr-a0510.appspot.com");
		StorageReference imageRef = storageRef.Child("transforms");
		StorageReference testPng = imageRef.Child("mat.csv");
		Stream stream = new FileStream(UnityEngine.Application.persistentDataPath+"/Images/mat.csv", FileMode.Open);
		testPng.PutStreamAsync(stream).ContinueWith((System.Threading.Tasks.Task<StorageMetadata> task) => {
			if (task.IsFaulted || task.IsCanceled) {
				Debug.Log(task.Exception.ToString());
				// Uh-oh, an error occurred!
			}
			else {
				// Metadata contains file metadata such as size, content-type, and download URL.
				StorageMetadata metadata = task.Result;
				string md5Hash = metadata.Md5Hash;
				Debug.Log("Finished uploading...");
				Debug.Log("md5 hash = " + md5Hash);
			}
		});
        // v = viewpoints[viewpoints.Count - 1];
        // CreateGameObjectFromViewpoint(v);
    }

    string CreateCSVString(Camera cam)
    {
        return Time.time + "," + cam.transform.position + "\n";
    }

#if SRP
#else
    private void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += endRender;
    }

    private void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= endRender;
    }
#endif

#if SRP
    public void MyPostRender(Camera cam)
    {
        // Camera cam = this.GetComponent<Camera>();


#else
    private void endRender(ScriptableRenderContext arg1, Camera cam)
    {
#endif


        

        if (cam != Camera.main)
            return;


//        Debug.Log(RenderTexture.active);
 //      CaptureView(cam, testView);
 //
 //  
 //
 //
 //      Graphics.Blit(RenderTexture.active, testView.colorTexture);
 //
 //      return;
 //

        // Debug.Log("Camera callback: Camera name is " + cam.name);



        //        Debug.Log("got here");

        // if (CameraLog.Length < 1024)
        //     CameraLog += CreateCSVString(cam);
        // else
        //     Debug.Log(CameraLog);

        //don't do this too fast
        if (captureViews)
        {

            

            if ((Time.time - previousSampleTime) > samplingInterval)
            {
                previousSampleTime = Time.time;

                if (!viewpointsL1.IsFull())
                    viewpointsL1.IncrementBackPtr();

                //if (viewpointsL1.Count >= maxCompareFrames - 1)
                //{
                //    ExtractAndReset(viewpointsL1, viewpointsL2);
                //}

            }



            Viewpoint viewpoint = viewpointsL1.Back();

            CaptureView(cam, viewpoint);

          

            if (showComparison)
            {
                if (viewpointsL1.Count >= 2)
                {
                    ShowDifference(viewpointsL1.Front(), viewpoint);

                    Graphics.Blit(viewpoint.colorTexture, RenderTexture.active);
                }
            }


        }


    }

}
