
using System.Threading.Tasks;
using UnityEngine;
using GLTFast;

namespace App.Services.ModelARViewing.ModelLoading
{
    public class GLTFastGlbModelLoader : IGlbModelLoader
    {
        public ModelObject LoadModel(byte[] modelData)
        {
            return new ModelObject(new GameObject());
        }

        public async Task<ModelObject> LoadModelAsync(byte[] modelData)
        {
            ModelObject model = null;
            GltfImport gltfImport = new GltfImport();

            bool success = await gltfImport.LoadGltfBinary(modelData);

            GameObject gameObjectRoot = new GameObject("GLTFRoot");

            if (success)
            {
                success = await gltfImport.InstantiateSceneAsync(gameObjectRoot.transform);
            }

            if (success)
            {
                model = new ModelObject(gameObjectRoot);
            }

            return model;
        }
    }
}