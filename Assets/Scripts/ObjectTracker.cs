using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class ObjectTracker : MonoBehaviour
{

    public bool capture;
    public int replayPos = 0;

    [Serializable]
    public class TT
    {
        public float time;
        public Vector3 position;
        public Quaternion rotation;

        public TT()
        {

        }

        public TT(Transform transform)
        {
            time = Time.time;
            position = transform.position;
            rotation = transform.rotation;
        }

        public void set(Transform transform)
        {
            transform.rotation = rotation;
            transform.position = position;
        }



        public void WriteData(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(time);
            binaryWriter.Write(position.x);
            binaryWriter.Write(position.y);
            binaryWriter.Write(position.z);
            binaryWriter.Write(rotation.x);
            binaryWriter.Write(rotation.y);
            binaryWriter.Write(rotation.z);
            binaryWriter.Write(rotation.w);
        }

        public void ReadData(BinaryReader binaryReader)
        {
            time = binaryReader.ReadSingle();
            position.x = binaryReader.ReadSingle();
            position.y = binaryReader.ReadSingle();
            position.z = binaryReader.ReadSingle();
            rotation.x = binaryReader.ReadSingle();
            rotation.y = binaryReader.ReadSingle();
            rotation.z = binaryReader.ReadSingle();
            rotation.w = binaryReader.ReadSingle();
        }

    };


    public List<TT> captureTT = new List<TT>();
    public List<TT> replayTT = new List<TT>();


    void CaptureCurrentTT(Transform transform)
    {
        TT tt = new TT(transform);
        captureTT.Add(tt);
        //        Debug.Log("here");
    }

    public void StoreCaptureTT(string filename)
    {

        using (var stream = File.Open(filename, FileMode.Create))
        {
            using (var bw = new BinaryWriter(stream))
            {
                foreach (TT tt in captureTT)
                {
                    tt.WriteData(bw);
                    //     Debug.Log(tt.time);
                }

                bw.Close();
            }
            stream.Close();
        }
    }


    public void LoadReplayTT(string filename)
    {
        using (BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open)))
        {
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                TT tt = new TT();
                tt.ReadData(br);
                replayTT.Add(tt);
            }
        }
    }

    void OnDestroy()
    {
        // Debug.Log("HERE " + captureTT.Count);
        if (capture)
            StoreCaptureTT("capture.bin");
    }

    void LateUpdate()
    {
        if (!capture)
        {
            replayTT[replayPos].set(this.transform);

            if (Time.time > replayTT[replayPos].time)
                if (replayPos < replayTT.Count - 2)
                    replayPos++;
        }
        else
        {
            CaptureCurrentTT(this.transform);
        }

    }



    // Start is called before the first frame update
    void Start()
    {
        if (!capture)
            LoadReplayTT("capture.bin");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
