// (c) Copyright HutongGames, LLC 2010-2017. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Asset Bundle")]
	[Tooltip("Load Scene from Asset Bundle.")]

	public class  LoadAssetScene : FsmStateAction
	{

		[RequiredField]
		[Tooltip("Asset bundle to load scene from.")]
		[UIHint(UIHint.Variable)]
		public FsmObject AssetBundle;
		
		[RequiredField]
		[Tooltip("The scene int number to load.")]
		public FsmInt sceneNumber;
		
		[ActionSection("Output")]
		
		[ArrayEditor(VariableType.String)]
		[Title("Scene Paths")]
		[Tooltip("Save an array of all the scene paths to use later.")]
		public FsmArray scenes;
		
		[ActionSection("Event")]
		
		[Tooltip("Event to fire on load failure.")]
		public FsmEvent loadFailed;
		
		[Tooltip("Event to fire on load success")]
		public FsmEvent loadSuccess;

		[ActionSection("Options")]
		
		[Tooltip("Optionally load the scene immediately.")]
		public FsmBool enableSceneLoad;

		[Tooltip("Set to true for optional debug messages in the console. Turn off for builds.")]
		public FsmBool enableDebug;
		
		private AssetBundle _bundle;

		public override void Reset()
		{

			enableDebug = false;
			enableSceneLoad = true; 
			AssetBundle = null;
			sceneNumber = 0;
			loadFailed = null;
			loadSuccess = null;
			scenes = null;
			
		}

		public override void OnEnter()
		{
			loadScene();
		}
		
		
		void loadScene()
		{
			_bundle = (AssetBundle)AssetBundle.Value;
		
			// get all scene paths from bundle.
			string[] scenePath = _bundle.GetAllScenePaths();
			
			// check scenes exist.
			if(scenePath.Length == 0)
			{
				
				if(enableDebug.Value)
				{
					Debug.Log("No scenes were found in this AssetBundle");
					
				}
				
				Fsm.Event(loadFailed);
				
			}
			
			// Save all scene paths to a playmaker array.
			scenes.Values = scenePath;
			
			// check scene number is within range
			if(sceneNumber.Value >= scenePath.Length)
			{
				
				if(enableDebug.Value)
				{
					Debug.Log("This scene does not exist in this AssetBundle");
					
				}
				
				// out of range, therefore fail event.
				Fsm.Event(loadFailed);
				
			}
			
			// number is in range
			else
			{
			
				// debug path
				if(enableDebug.Value)
				{
					Debug.Log(scenePath[sceneNumber.Value]);
					
				}
				
				
				// load path
				if(enableSceneLoad.Value)
				{
					Application.LoadLevel(scenePath[sceneNumber.Value]);
				}
				
				// if dont load, then finish action.
				else
				{
					Fsm.Event(loadSuccess);
				}
			}
			
		}
	}
}