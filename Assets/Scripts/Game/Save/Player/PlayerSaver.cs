using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scripts.Game.Save;
using Scripts.Game.Save.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Game.Save.Player
{
    public class PlayerSaver : BaseSaver
    {
        [SerializeField] private Game.Player _player;

        public override string Key => "Player";

        public override JObject Save()
        {
            var jobject = new JObject();
            var inventoryJson = JsonConvert.SerializeObject(_player.Inventory);
            var positionJson = JsonConvert.SerializeObject(new SerializableVector3 (_player.transform.localPosition));
            var rotationJson = JsonConvert.SerializeObject(new SerializableQuaternion (_player.transform.localRotation));
            var cameraRotationJson = JsonConvert.SerializeObject(new SerializableQuaternion (Camera.main.transform.localRotation));

            jobject.Add("inventory", inventoryJson);
            jobject.Add("position", positionJson);
            jobject.Add("rotation", rotationJson);
            jobject.Add("camera_rotation", cameraRotationJson);

            return jobject;
        }

        public override bool Load(JObject data)
        {
            if (data is null)
            {
                _player.SetInventory(new Inventory());
                
                return true;
            }    

            if (data.TryGetValue("inventory", out var inventoryJson))
            {
                var inventory = JsonConvert.DeserializeObject<Inventory>(inventoryJson.ToString());

                _player.SetInventory(inventory);
            }

            if (data.TryGetValue("position", out var positionJson))
            {
                var position = JsonConvert.DeserializeObject<SerializableVector3>(positionJson.ToString());
                
                _player.transform.localPosition = position.ToVector3();
            }

            if (data.TryGetValue("rotation", out var rotationJson))
            {
                var rotation = JsonConvert.DeserializeObject<SerializableQuaternion>(rotationJson.ToString());

                _player.transform.localRotation = rotation.ToQuaternion();
            }

            if (data.TryGetValue("camera_rotation", out var cameraRotationJson))
            {
                var cameraRotation = JsonConvert.DeserializeObject<SerializableQuaternion>(cameraRotationJson.ToString());

                Camera.main.transform.localRotation = cameraRotation.ToQuaternion();
            }

            return true;
        }
    }
}
