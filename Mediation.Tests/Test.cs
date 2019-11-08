using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Mediation.Tests {
    public class Test {
        struct Ping {
            public string Message { get; }

            public Ping(string message) {
                Message = message ?? throw new ArgumentNullException(nameof(message));
            }
        }

        struct Pong {
            public string Message { get; }

            public Pong(string message) {
                Message = message ?? throw new ArgumentNullException(nameof(message));
            }
        }

        class RequestHandlerTest : IRequestHandler<Ping, Pong> {
            public Pong Handle(Ping request) {
                return new Pong("Pong");
            }

            public Task<Pong> HandleAsync(Ping request, CancellationToken cancellationToken) {
                return Task.FromResult(new Pong("Pong"));
            }
        }

        class InnerPipelineStep<TRequest, TResponse> : IPipelineStep<TRequest, TResponse> {
            RequestEventLog Messages { get; }

            public InnerPipelineStep(RequestEventLog messages) {
                Messages = messages ?? throw new ArgumentNullException(nameof(messages));
            }

            public TResponse Execute(TRequest request, Func<TResponse> next) {
                Messages.Log("Inner Pipe Executing");
                var response = next();
                Messages.Log("Inner Pipe Executed");
                return response;
            }

            public async Task<TResponse> ExecuteAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken) {
                Messages.Log("Inner Pipe Executing Async");
                var response = await next().ConfigureAwait(false);
                Messages.Log("Inner Pipe Executed Async");
                return response;
            }
        }

        class OuterPipelineStep<TRequest, TResponse> : IPipelineStep<TRequest, TResponse> {
            RequestEventLog Messages { get; }

            public OuterPipelineStep(RequestEventLog messages) {
                Messages = messages ?? throw new ArgumentNullException(nameof(messages));
            }

            public TResponse Execute(TRequest request, Func<TResponse> next) {
                Messages.Log("Outer Pipe Executing");
                var response = next();
                Messages.Log("Outer Pipe Executed");
                return response;
            }

            public async Task<TResponse> ExecuteAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken) {
                Messages.Log("Outer Pipe Executing Async");
                var response = await next().ConfigureAwait(false);
                Messages.Log("Outer Pipe Executed Async");
                return response;
            }
        }

        class RequestEventLog {
            List<string> _Events { get; } = new List<string>();
            public IReadOnlyList<string> Events { get => _Events; }

            public void Log(string @event) {
                _Events.Add(@event);
            }
        }

        IServiceProvider GetRequestServiceProvider() {
            return new ServiceCollection()
                .AddSingleton<RequestEventLog>()
                .AddTransient(typeof(IPipelineStep<,>), typeof(OuterPipelineStep<,>))
                .AddTransient(typeof(IPipelineStep<,>), typeof(InnerPipelineStep<,>))
                .AddTransient<IRequestHandler<Ping, Pong>, RequestHandlerTest>()
                .AddTransient<MediationServiceProvider>(provider => provider.GetService)
                .AddTransient<IMediator, DefaultMediator>()
                .BuildServiceProvider();
        }

        [Fact]
        public async Task Testing() {
            var provider = GetRequestServiceProvider();
            var mediator = provider.GetRequiredService<IMediator>();
            var result = await mediator.RequestAsync<Ping, Pong>(new Ping("Ping"))
                .ConfigureAwait(false);
            Assert.IsType<Pong>(result);
            Assert.Equal("Pong", result.Message);
            var log = provider.GetRequiredService<RequestEventLog>();
            Assert.Equal(log.Events, new string[] {
                "Outer Pipe Executing Async",
                "Inner Pipe Executing Async",
                "Inner Pipe Executed Async",
                "Outer Pipe Executed Async"
            });
        }
    }
}
