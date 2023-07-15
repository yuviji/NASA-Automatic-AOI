using System.Threading.Tasks;
using aoiRouting.Shared.Models;
namespace aoiRouting.Shared
{
    public interface IPinClient
    {
        Task ReceivePin(Pin pin);
    }
}