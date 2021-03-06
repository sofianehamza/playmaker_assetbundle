// (c) Copyright HutongGames, LLC 2010-2017. All rights reserved.

using System;
using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Asset Bundle")]
	[Tooltip("Load an AssetBundle from the cache. If AssetBundle is not currently cached, it will be downloaded and stored locally. For Unity 5.3 or later, using UnityWebRequest instead is recommended.")]

	public class  AssetBundleLoadFromCacheOrDownload : FsmStateAction
	{
			
		[RequiredField]
		[Tooltip("Path name of the asset bundle to load. This field is case sensitive.")]
		[Title("Bundle Path")]
		public FsmString myassetBundle;
		
		[RequiredField]
		[Tooltip("Version of the asset bundle to load. If you are unsure, set to 1")]
		[Title("Version")]
		public FsmInt version;
			
		[ActionSection("Events")]
			
		[Tooltip("Event to fire on load success.")]
		public FsmEvent loadSuccess;
			
		[Tooltip("Event to fire on load failure.")]
		public FsmEvent loadFailed;
			
		[ActionSection("Output")]
			
		[RequiredField]
		[Tooltip("Saved asset bundle. Important: Create an playmaker object variable with the object type of UnityEngine/AssetBundle")]
		[Title("Asset Bundle")]
		[UIHint(UIHint.Variable)]
		public FsmObject myLoadedAssetBundle;
		
		[Tooltip("Optionally save the progress of loading.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat progress;

		[ActionSection("Options")]

		[Tooltip("Set to true for optional debug messages in the console. Turn off for builds.")]
		public FsmBool enableDebug;
			
		// private variables
		private AssetBundle _bundle;
		private WWW www;
			
		public override void Reset()
			{
				
				enableDebug = false;
				myLoadedAssetBundle = null;
				myassetBundle = new FsmString { UseVariable = false };
				loadFailed = null;
				loadSuccess = null;
				version =  new FsmInt { UseVariable = false };
				progress = null;
				
			}
			
		public override void OnEnter()
			{

				StartCoroutine(LoadBundleWWW());
				
			}
			
			
		public IEnumerator LoadBundleWWW()
			{
				
				
				// check for bundle
				while (!Caching.ready)
				yield return null;
				
				www = WWW.LoadFromCacheOrDownload(myassetBundle.Value, version.Value);
				yield return www;
				
				
				// If bundle returned is null or empty, fail load.
				if (!string.IsNullOrEmpty(www.error))
				{
					
					if(enableDebug.Value)
					{
						Debug.Log(www.error);
						Debug.Log("Failed to load AssetBundle.");
					}
					
					Fsm.Event(loadFailed);
					yield return null;
				}
				
				_bundle = www.assetBundle;
				
			// Asset bundle loaded successfully
				
				if (www.isDone && _bundle != null)
				{
					if(enableDebug.Value)
					{
						Debug.Log("AssetBundle load success.");
					}
					
					myLoadedAssetBundle.Value = _bundle;
					Fsm.Event(loadSuccess);
					
				}		
				
				// Asset bundle loaded fail
				
				if (www.isDone && _bundle == null)
				{
					if(enableDebug.Value)
					{
						Debug.Log("AssetBundle load failure. Bundle is null");
					}
					
					Fsm.Event(loadFailed);
					
				}	
				
			}
			
			public override void OnUpdate()
			{

				// load progress
				progress.Value = www.progress * 100f;
					
					if(enableDebug.Value)
					{
						Debug.Log("Bundle Load Progress: " + www.progress * 100f); 
					}
					
			}
				
	}
}