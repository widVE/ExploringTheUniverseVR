using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Graphics
{
	/// <summary>
	/// Simple script that forces the render queue of all materials attached to the same GameObject.
	/// </summary>
	[ExecuteInEditMode]
	public class MaterialRenderQueue : MonoBehaviour
	{
		Material[] M;
		bool HasMaterials { get { return M != null && M.Length > 0; } }
		[Range(0, 5000)]
		public int RenderQueue = -2;
		int _prevRenderQueue;

		[ContextMenu("Refresh")]
		void Awake()
		{
			GetMaterials();
			RenderQueue = 2000;
			if (HasMaterials)
			{
				Renderer r = GetComponent<Renderer>();
				if (r) RenderQueue = r.sharedMaterial.renderQueue;
			}
			_prevRenderQueue = RenderQueue;
		}

		void OnValidate()
		{
			SetRenderQueue();
		}

		void Start()
		{
			SetRenderQueue();
		}

		void Update()
		{
			if (_prevRenderQueue != RenderQueue || (HasMaterials && M[0].renderQueue != RenderQueue)) SetRenderQueue();
		}

		void SetRenderQueue()
		{
			if (RenderQueue < -1) RenderQueue = 0;
			else if (RenderQueue > 5000) RenderQueue = 5000;
			if (!HasMaterials) GetMaterials();
			if (HasMaterials)
			{
				for (int i = 0; i < M.Length; i++)
				{
					M[i].renderQueue = RenderQueue;
				}
			}
			_prevRenderQueue = RenderQueue;
		}

		void GetMaterials()
		{   //find the materials attached to this object
			Renderer r = GetComponent<Renderer>();
			if (r) M = r.sharedMaterials;
		}
	}
}