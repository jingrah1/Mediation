using System.Threading.Tasks;

namespace Mediation.Tests {
    class Test {
        async Task Testing() {
            IMediator x = null;
            x.Send<string>("Test");
            x.Send<object>("test");
            x.Request<string>("test");
            x.Request<string, string>("test");
            await x.RequestAsync<string>("test");
            await x.RequestAsync<string, string>("Test");
            await x.SendAsync("test");
            await x.SendAsync<double>(2);
        }
    }
}
