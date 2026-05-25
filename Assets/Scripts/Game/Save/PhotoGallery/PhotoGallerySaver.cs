using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scripts.Game.Save;

namespace Game.Save.PhotoGallery
{
    public class PhotoGallerySaver : BaseSaver
    {
        public override string Key => "PhotoGallery";
        public override JObject Save()
        {
            var photoGallery = DependencyContainer.PhotoGallery;
            var jobject = new JObject();
            var photoGalleryJson = JsonConvert.SerializeObject(photoGallery);

            jobject.Add("photoGallery", photoGalleryJson);

            return jobject;
        }

        public override bool Load(JObject data)
        {
            var photoGallery = DependencyContainer.PhotoGallery;
            
            if (data is null)
            {
                photoGallery.SetPhotos(new());
                
                return true;
            }    

            if (data.TryGetValue("photoGallery", out var photoGalleryJson))
            {
                var photoGalleryRaw = JsonConvert.DeserializeObject<Scripts.Game.PhotoGallery>(photoGalleryJson.ToString());

                photoGallery.SetPhotos(photoGalleryRaw.PhotosBase64.ToList());
            }
            
            return true;
        }
    }
}