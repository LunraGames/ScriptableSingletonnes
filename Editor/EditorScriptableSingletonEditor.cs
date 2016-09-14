using System.IO;
using UnityEditor;
using UnityEngine;

namespace LunraGames.Singletonnes
{
	[CustomEditor(typeof(EditorScriptableSingleton), true)]
	public class EditorScriptableSingletonEditor : Editor 
	{
		static float ButtonHeight = 40f;
		static string WrongNameMessage = "Your asset's name does not match its type.";
		static string WrongPathMessage = "Your asset is not in a valid directory.";
		static string WrongNameAndPathMessage = "Your asset's name does not match its type, and is not in a valid directory.";

		static string AutoFixText = "Auto Fix";
		static string DefineDirectoryFixText = "Define Specific Editor Folder";
			
		public override void OnInspectorGUI() 
		{
			var typedTarget = (EditorScriptableSingleton)target;

			var path = AssetDatabase.GetAssetPath(typedTarget);
			var assetName = Path.GetFileNameWithoutExtension(path);
			var requiredName = typedTarget.CurrentType.Name;
			var invalidName = assetName != requiredName;
			var invalidPath = !path.Contains("/Editor/");

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
							var selectedPath = UnityEditor.EditorUtility.SaveFolderPanel("Select a Editor Directory", "Assets", string.Empty);
							if (!string.IsNullOrEmpty(selectedPath)) 
							{
								selectedPath = selectedPath.Substring(Path.GetDirectoryName(Application.dataPath).Length + 1);

								if (selectedPath.EndsWith("/Editor") || selectedPath.Contains("/Editor/")) 
								{
									MoveAsset(path, Path.Combine(selectedPath, requiredName + ".asset")); 
								} 
								else UnityEditor.EditorUtility.DisplayDialog("Invalid", "You must select an \"Editor\" directory.", "Okay");
							}
						}
						if (GUILayout.Button(AutoFixText, EditorStyles.miniButton, GUILayout.Height(ButtonHeight)))
						{
							MoveAsset(path, Path.Combine(Path.Combine(Path.GetDirectoryName(path), "Editor"), requiredName + ".asset"));
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
				AssetDatabase.CreateFolder(Path.GetDirectoryName(parentDir), "Editor");
			}

			moveResult = AssetDatabase.MoveAsset(originPath, targetPath);

			if (!string.IsNullOrEmpty(moveResult)) UnityEditor.EditorUtility.DisplayDialog("Error", moveResult, "Okay");
		}
	}
}