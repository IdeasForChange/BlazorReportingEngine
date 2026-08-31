using System.Threading.Channels;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class ReportJobChannel
{
    private readonly Channel<long> _channel = Channel.CreateUnbounded<long>(new UnboundedChannelOptions
    {
        SingleReader = false,
        SingleWriter = false
    });

    public ChannelWriter<long> Writer => _channel.Writer;
    public ChannelReader<long> Reader => _channel.Reader;
}