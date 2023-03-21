
using System.Threading;
using System.Threading.Tasks;

namespace App.Services.ModelARViewing.ModelLoading
{
    public interface IGlbModelLoader
    {
        Model LoadModel(byte[] modelData);
        Task<Model> LoadModelAsync(byte[] modelData, CancellationToken cancellationToken);
    }
}
