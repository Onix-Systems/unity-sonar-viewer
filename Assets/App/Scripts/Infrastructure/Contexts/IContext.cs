
using App.Infrastructure.CommonInterfaces;

namespace App.Infrastructure.Contexts
{
    public interface IContext: IInitializable, IDisposable
    {
        TService Get<TService>() where TService : class;
        bool TryGet<TService>(out TService service) where TService : class;
    }
}
