using UnityEngine;
using UnityEditor;
using System;

namespace LunraGames.Singletonnes
{
	public abstract class EditorScriptableSingleton : ScriptableObject
	{
		Type _CurrentType;
		public Type CurrentType { get { return _CurrentType; } }

		protected EditorScriptableSingleton() 
		{
			_CurrentType = GetType();	
		}
	}

	public abstract class EditorScriptableSingleton<T> : EditorScriptableSingleton 
		where T : EditorScriptableSingleton<T>
	{
		static T _Instance;

		public static T Instance { get { return _Instance ?? ( _Instance = FindInstance()); } }

		static T FindInstance()
		{
			var instances = AssetDatabase.FindAssets("t:ScriptableObject "+typeof(T).Name);
			if (instances.Length != 1) 
			{
				Debug.LogError(instances.Length == 0 ? "No instance of Noise Maker settings exist" : "More than one instance of Noise Maker settings exists");
				return null;
			}
			return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(instances[0]));
		}
	}
}