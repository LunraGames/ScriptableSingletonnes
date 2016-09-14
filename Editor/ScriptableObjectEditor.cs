using System.IO;
using UnityEditor;
using UnityEngine;

namespace LunraGames.Singletonnes
{
	[CustomEditor(typeof(ScriptableSingleton), true)]
	public class ScriptableSingletonEditor : Editor 
	{
		static float ButtonHeight = 40f;
		static string WrongNameMessage = "Your asset's name does not match its type.";
		static string WrongPathMessage = "Your asset is not in a valid directory.";
		static string WrongNameAndPathMessage = "Your asset's name does not match its type, and is not in a valid directory.";

		static string AutoFixText = "Auto Fix";
		static string DefineDirectoryFixText = "Define Specific Resources Folder";
			
		public override void OnInspectorGUI() 
		{
			var typedTarget = (ScriptableSingleton)target;

			var path = AssetDatabase.GetAssetPath(typedTarget);
			var assetName = Path.GetFileNameWithoutExtension(path);
			var requiredName = typedTarget.CurrentType.Name;
			var requiredPathEnding = Path.Combine("Resources", Path.Combine(ScriptableSingleton.ContainingDirectory, requiredName));
			var invalidName = assetName != requiredName;
			var invalidPath = !path.EndsWith(Path.Combine(Path.Combine("Resources", ScriptableSingleton.ContainingDirectory), Path.GetFileName(path)));

			if (invalidName || invalidPath) 
			{
				EditorGUILayout.BeginHorizontal();
				{
					var helpMessage = invalidName && invalidPath ? WrongNameAndPathMessage : (invalidName ? WrongNameMessage : WrongPathMessage);
					EditorGUILayout.HelpBox(helpMessage, MessageType.Error);
					if (invalidPath) 
					{
						if (GUILayout.Button(DefineDirectoryFixText, EditorStyles.miniButton, GUILayout.Height(ButtonHeight))) 
						{
							var selectedPath = UnityEditor.EditorUtility.SaveFolderPanel("Select a Resources Directory", "Assets", string.Empty);
							if (!string.IsNullOrEmpty(selectedPath)) 
							{
								selectedPath = selectedPath.Substring(Path.GetDirectoryName(Application.dataPath).Length + 1);

								if (selectedPath.EndsWith("Resources")) 
								{
									MoveAsset(path, Path.Combine(selectedPath, Path.Combine(ScriptableSingleton.ContainingDirectory, requiredName + ".asset")));
								}
								else if (selectedPath.EndsWith(Path.Combine("Resources", ScriptableSingleton.ContainingDirectory))) 
								{
									MoveAsset(path, Path.Combine(selectedPath, requiredName + ".asset"));
								} 
								else UnityEditor.EditorUtility.DisplayDialog("Invalid", "You must select a \"Resources\" directory.", "Okay");
							}
						}
						if (GUILayout.Button(AutoFixText, EditorStyles.miniButton, GUILayout.Height(ButtonHeight)))
						{
							MoveAsset(path, Path.Combine("Assets", requiredPathEnding + ".asset"));
						}
					}
					else if (invalidName)
					{
						if (GUILayout.Button(AutoFixText, EditorStyles.miniButton, GUILayout.Height(ButtonHeight))) 
						{
							MoveAsset(path, Path.Combine(Path.GetDirectoryName(path), requiredName+".asset"));
						}
					}
				}
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(4f);
			}

			DrawDefaultInspector();

		}

		void MoveAsset(string originPath, string targetPath) 
		{
			var moveResult = string.Empty;
			var parentDir = Path.GetDirectoryName(targetPath);

			if (!AssetDatabase.IsValidFolder(parentDir)) 
			{
				var resourceDir = Path.GetDirectoryName(parentDir);
				if (!AssetDatabase.IsValidFolder(resourceDir)) AssetDatabase.CreateFolder(Path.GetDirectoryName(resourceDir), "Resources");
				AssetDatabase.CreateFolder(resourceDir, ScriptableSingleton.ContainingDirectory);
			}

			moveResult = AssetDatabase.MoveAsset(originPath, targetPath);

			if (!string.IsNullOrEmpty(moveResult)) UnityEditor.EditorUtility.DisplayDialog("Error", moveResult, "Okay");
		}
	}
}