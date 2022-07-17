using System.Threading.Tasks;

namespace Api.Services.Streams
{
    public interface IConsumer<in T>
    {
        Task Subscribe();
        Task Consume(T data);
        Task Complete();
    }
}