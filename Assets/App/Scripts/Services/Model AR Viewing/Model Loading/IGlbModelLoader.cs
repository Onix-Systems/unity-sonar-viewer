
using System.Threading.Tasks;

namespace App.Services.ModelARViewing.ModelLoading
{
    public interface IGlbModelLoader
    {
        ModelObject LoadModel(byte[] modelData);
        Task<ModelObject> LoadModelAsync(byte[] modelData);
    }
}
