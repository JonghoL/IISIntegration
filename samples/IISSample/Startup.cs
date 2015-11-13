using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Server.Kestrel;
using Microsoft.AspNet.Server.Kestrel.Filter;
using Microsoft.Extensions.Logging;

namespace IISSample
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerfactory)
        {
            loggerfactory.AddConsole(LogLevel.Verbose);
            
            var serverInfo = app.ServerFeatures.Get<IKestrelServerInformation>();
            serverInfo.ConnectionFilter = new LoggingConnectionFilter(loggerfactory);

            var logger = loggerfactory.CreateLogger("Requests");

            app.UseIISPlatformHandler();
            
            app.Run(async (context) =>
            {
                logger.LogVerbose("Received request: " + context.Request.Method + " " + context.Request.Path);

                context.Response.ContentType = "text/plain; charset=utf-8";
                // await context.Response.WriteAsync("Hello World - " + DateTimeOffset.Now + Environment.NewLine);
                await context.Response.WriteAsync("Path - " + context.Request.Path.Value + Environment.NewLine);
                await context.Response.WriteAsync("Query - " + context.Request.QueryString.Value + Environment.NewLine);
                throw new Exception("Help!");
                /*
                foreach (var header in context.Request.Headers)
                {
                    await context.Response.WriteAsync(header.Key + ": " + header.Value + Environment.NewLine);
                }*/
            });
        }
    }

    public class LoggingConnectionFilter : IConnectionFilter
    {
        private readonly ILogger _logger;

        public LoggingConnectionFilter(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger<LoggingConnectionFilter>();
        }

        public Task OnConnection(ConnectionFilterContext context)
        {
            context.Connection = new LoggingStream(context.Connection, _logger);
            return Task.FromResult(0);
        }
    }

    public class LoggingStream : Stream
    {
        private readonly Stream _inner;
        private readonly ILogger _logger;

        public LoggingStream(Stream inner, ILogger logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public override bool CanRead
        {
            get
            {
                return _inner.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return _inner.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _inner.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return _inner.Length;
            }
        }

        public override long Position
        {
            get
            {
                return _inner.Position;
            }

            set
            {
                _inner.Position = value;
            }
        }

        public override void Flush()
        {
            _inner.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = _inner.Read(buffer, offset, count);
            _logger.LogVerbose($"Read[{read}]" + PrintBuffer(buffer, offset, read));
            return read;
        }

        public async override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int read = await _inner.ReadAsync(buffer, offset, count, cancellationToken);
            _logger.LogVerbose($"ReadAsync[{read}]" + PrintBuffer(buffer, offset, read));
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _logger.LogVerbose($"Write[{count}]" + PrintBuffer(buffer, offset, count));
            _inner.Write(buffer, offset, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            _logger.LogVerbose($"WriteAsync[{count}]" + PrintBuffer(buffer, offset, count));
            return _inner.WriteAsync(buffer, offset, count, cancellationToken);
        }

        private string PrintBuffer(byte[] buffer, int offset, int read)
        {
            var builder = new StringBuilder();
            for (int i = offset; i < read + offset; i++)
            {
                builder.Append(buffer[i].ToString("x"));
                builder.Append(" ");
            }
            builder.AppendLine();
            for (int i = offset; i < read + offset; i++)
            {
                builder.Append((char)buffer[i]);
            }
            return builder.ToString();
        }
    }
}
