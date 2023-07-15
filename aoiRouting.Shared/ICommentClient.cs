using System.Threading.Tasks;
using aoiRouting.Shared.Models;
namespace aoiRouting.Shared
{
    public interface ICommentClient
    {
        Task ReceiveComment(Comment comment);
    }
}