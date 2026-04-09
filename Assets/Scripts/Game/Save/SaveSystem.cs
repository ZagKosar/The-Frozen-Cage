using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

            foreach (var saver in savers)
            {
                jobject.Add(saver.Key, saver.Save());
            }

            File.WriteAllText(saveFile, jobject.ToString());
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
