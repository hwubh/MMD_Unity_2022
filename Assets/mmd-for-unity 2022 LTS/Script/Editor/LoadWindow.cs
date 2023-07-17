using UnityEngine;
using UnityEditor;
using MMD_URP;
using System.IO;

//public class LoadedWindow : EditorWindow
//{
//	/// <summary>
//	/// メッセージ用テキスト
//	/// </summary>
//	public string Text { get; set; }

//	const int width = 400;

//	const int height = 300;

//	/// <summary>
//	/// 初期化
//	/// </summary>
//	/// <returns>ウィンドウ</returns>
//	public static LoadedWindow Init()
//	{
//		var window = EditorWindow.GetWindow<LoadedWindow>("PMD file loaded!") as LoadedWindow;
//		var pos = window.position;
//		pos.height = LoadedWindow.height;
//		pos.width = LoadedWindow.width;
//		window.position = pos;
//		return window;
//	}

//	void OnGUI()
//	{
//		EditorGUI.TextArea(new Rect(0, 0, LoadedWindow.width, LoadedWindow.height - 30), this.Text);

//		if (GUI.Button(new Rect(0, height - 30, LoadedWindow.width, 30), "OK"))
//			Close();
//	}
//}

public class LoaderWindow : EditorWindow
{
	Object pmxFile;
	Object vmdFile;

	[MenuItem("MMD for Unity/PMX/VMD Loader")]
	static void Init()
	{
		var window = GetWindow<LoaderWindow>(true, "Import MMD Files");
		window.Show();
	}

	void OnEnable()
	{

	}

	void OnGUI()
	{
		// GUIの有効化
		//GUI.enabled = !EditorApplication.isPlaying;

		// GUI描画
		pmxFile = EditorGUILayout.ObjectField("PMX File", pmxFile, typeof(Object), false);
		vmdFile = EditorGUILayout.ObjectField("VMD File", vmdFile, typeof(Object), false);

		{
			bool gui_enabled_old = GUI.enabled;
			GUI.enabled =  (pmxFile != null) && (vmdFile != null);
			if (GUILayout.Button("Convert"))
			{
				GameSystem.pmxPath = Path.Combine(Application.dataPath.Replace("/Assets", ""), AssetDatabase.GetAssetPath(pmxFile));
				GameSystem.vmdPath = Path.Combine(Application.dataPath.Replace("/Assets", ""), AssetDatabase.GetAssetPath(vmdFile));
				//if(GameSystem.convertor == null)
				//	GameSystem.convertor = new GameObject("Convertor").AddComponent<Convertor>();
				GameSystem.convertor = GameSystem.convertor ?? new GameObject("Convertor").AddComponent<Convertor>();
				//GameSystem.convertor.Import(GameSystem.pmxPath = AssetDatabase.GetAssetPath(pmxFile), GameSystem.vmdPath = AssetDatabase.GetAssetPath(vmdFile));
				GameSystem.convertor.Import(GameSystem.pmxPath, GameSystem.vmdPath);
				vmdFile = pmxFile = null;     // 読み終わったので空にする 
			}
			GUI.enabled = gui_enabled_old;
		}
	}
}