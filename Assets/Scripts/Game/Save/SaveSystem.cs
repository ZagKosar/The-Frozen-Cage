using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Game.Save
{
    public class SaveSystem
    {
        public void Save(int saveSlot)
        {
            var savePath = Path.Combine(Application.persistentDataPath, "Save");
            var saveFile = Path.Combine(savePath, $"Save_{saveSlot}.json");

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            var jobject = new JObject();
            var savers = GameObject.FindObjectsByType<BaseSaver>(FindObjectsSortMode.None);
            
            jobject["SceneIndex"] = SceneManager.GetActiveScene().buildIndex;
            
            foreach (var saver in savers)
            {
                jobject.Add(saver.Key, saver.Save());
            }

            File.WriteAllText(saveFile, jobject.ToString());
        }

        public int GetSaveSceneIndex(int saveSlot)
        {
            var savePath = Path.Combine(Application.persistentDataPath, "Save");
            var saveFile = Path.Combine(savePath, $"Save_{saveSlot}.json");

            if (!Directory.Exists(savePath))
                return 2;

            if (!File.Exists(saveFile))
                return 2;
            
            var json = File.ReadAllText(saveFile);
            var jobject = JObject.Parse(json);
            
            return (int)jobject.GetValue("SceneIndex");
        }

        public void Load(int saveSlot)
        {
            var savePath = Path.Combine(Application.persistentDataPath, "Save");
            var saveFile = Path.Combine(savePath, $"Save_{saveSlot}.json");

            if (!Directory.Exists(savePath))
                return;

            if (!File.Exists(saveFile))
                return;

            var json = File.ReadAllText(saveFile);
            var jobject = JObject.Parse(json);
            var savers = GameObject.FindObjectsByType<BaseSaver>(FindObjectsSortMode.None);

            foreach (var saver in savers)
            {
                saver.Load(jobject.TryGetValue(saver.Key, out var data) ? data.ToObject<JObject>() : null);
            }
        }
    }
}
