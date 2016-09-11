using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LunraGames.ScriptableSingletons
{
	public abstract class ScriptableSingleton : ScriptableObject
	{
		public static string ContainingDirectory = "ScriptableSingletons";

		Type _CurrentType;
		public Type CurrentType { get { return _CurrentType; } }

		protected ScriptableSingleton() 
		{
			_CurrentType = GetType();	
		}
	}

	public abstract class ScriptableSingleton<T> : ScriptableSingleton where T : UnityEngine.Object 
	{
		static T _Instance;
		public static T Instance 
		{
			get 
			{
				return _Instance ?? (_Instance = Resources.Load<T>(Path.Combine(ContainingDirectory,typeof(T).Name)));
			}
		}
	}
}