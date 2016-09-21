using UnityEngine;
using UnityEditor;
using System;

namespace LunraGames.Singletonnes
{
	public abstract class EditorScriptableSingletonBase : ScriptableObject
	{
		public readonly Type CurrentType;

		protected EditorScriptableSingletonBase(Type type)
		{
			CurrentType = type;
		}
	}
}