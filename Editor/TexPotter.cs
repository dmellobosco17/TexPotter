using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Editor.ImageProcessing
{
    public class TexPotter
    {
        [MenuItem("Assets/⚠ OptimizeTextures ⚠")]
        public static void Run()
        {
            string file = "C:/Projects/FallCars/FallCars/Assets/Editor/ImageProcessing/beer.png";
            Object[] objects = Selection.GetFiltered(typeof(Texture), SelectionMode.Assets | SelectionMode.DeepAssets);

            string[] list = new string[objects.Length];
            string listText = "";

            for (int i = 0; i < objects.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(objects[i]);
                Debug.Log(path);
                list[i] = path;
                listText += "\t" + path + "\n\n";
            }

            ShowPopup window = ScriptableObject.CreateInstance<ShowPopup>();
            window.Show(
                "⚠ You are about to optimize textures ⚠",
                "All selected textures inside this folder and subfolders will be resized!!!\n", listText,
                () => { ProcessFiles(list); });
        }

        public static void ProcessFiles(string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Texture Optimization", files[i], i / (float) files.Length);
                OptimizeTexture(files[i]);
            }

            EditorUtility.ClearProgressBar();
            
            AssetDatabase.Refresh();
        }

        public static void OptimizeTexture(string file)
        {
            Debug.Log("Optimizing... " + file);
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(file);
            int width = GetNearestPOTSize(texture.width);
            int height = GetNearestPOTSize(texture.height);

            if (file.StartsWith("Assets/"))
                file = file.Substring("Assets".Length);
            file = Application.dataPath + file;

            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            string packagePath = Path.GetFullPath("Packages/com.bosco.texpotter").Replace('\\','/');
            startInfo.FileName = packagePath + "/Editor/ImageMagick/convert.exe";
            Debug.Log	("Converter path : "+startInfo.FileName);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = $"\"{file}\" -background transparent -gravity center -extent {width}x{height} \"{file}\"";
            //Process.Start(startInfo);
            Debug.Log(startInfo.FileName);
            Debug.Log(startInfo.Arguments);

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    while (!exeProcess.HasExited)
                    {
                        Debug.Log(exeProcess.StandardOutput.ReadLine());
                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static int GetNearestPOTSize(int size)
        {
            int pot = 1;
            while (pot <= 2048)
            {
                if (size <= pot) return pot;

                pot *= 2;
            }
            return pot;
        }
    }
}
