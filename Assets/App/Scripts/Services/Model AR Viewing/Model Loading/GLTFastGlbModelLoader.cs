
using System.Threading.Tasks;
using UnityEngine;
using GLTFast;
using System.Threading;

namespace App.Services.ModelARViewing.ModelLoading
{
    public class GLTFastGlbModelLoader : IGlbModelLoader
    {
        public Model LoadModel(byte[] modelData)
        {
            return new Model(new GameObject());
        }

        public async Task<Model> LoadModelAsync(byte[] modelData, CancellationToken cancellationToken)
        {
            Model model = null;
            GltfImport gltfImport = new GltfImport();

            bool success = await gltfImport.LoadGltfBinary(modelData);

            GameObject gameObjectRoot = new GameObject("GLTFRoot");

            if (success)
            {
                success = await gltfImport.InstantiateSceneAsync(gameObjectRoot.transform, cancellationToken: cancellationToken);
            }

            if (success)
            {
                model = new Model(gameObjectRoot);
            }

            return model;
        }
    }
}