using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Google.Protobuf;
using Statsd.Protobuf.Metrics;
using StatsdClient;

namespace app
{
  class Program
  {
    static readonly int count = 10000;

    static void SendUDP()
    {
      var tags = new Dictionary<string, string>();
      tags.Add("metric", "test");
      var start = DateTime.UtcNow;
      for (var i = 0; i < count; i++)
      {
        DogStatsd.Histogram("interception", 1f, tags: tags.Where(t => t.Value != null).Select(t => $"{t.Key}:{t.Value?.ToString().EscapeTagValue()}").ToArray());
      }
      var end = DateTime.UtcNow;

      Console.WriteLine($"udp {(end - start).TotalMilliseconds}");
    }

    static void SendUDS()
    {
      var start = DateTime.UtcNow;

      using (var socket = new Socket(AddressFamily.Unix, SocketType.Dgram, ProtocolType.Unspecified))
      {
        socket.Connect(new UnixDomainSocketEndPoint("/var/my.sock"));

        var tags = new Dictionary<string, string>();
        tags.Add("metric", $"test");

        var str = string.Join(", ", tags.Where(t => t.Value != null).Select(t => $"{t.Key}:{t.Value?.ToString().EscapeTagValue()}"));
        var dataToSend = System.Text.Encoding.UTF8.GetBytes(str);

        for (var i = 0; i < count; i++)
        {
          var m = new TraceMetric();
          m.Name = "interception";
          m.Value = 1;
          m.Tags.Add(new TraceMetric.Types.Tag { Name = "metric", Value = $"test {i}" });

          // var ns = new NetworkStream(socket);
          // using (var cod = new CodedOutputStream(ns))
          // {
          //   m.WriteTo(cod);
          // }

          socket.Send(dataToSend);
          // socket.Send(System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(m)));
          // Console.WriteLine($"done {i}");
        }
      }

      var end = DateTime.UtcNow;
      Console.WriteLine($"uds {(end - start).TotalMilliseconds}");
    }

    static void Main(string[] args)
    {
      DogStatsd.Configure(new StatsdConfig
      {
        StatsdServerName = "statsd",
        StatsdPort = 9125
      });

      for (int i = 0; i < 10; i++)
      {
        SendUDP();
        SendUDS();
      }
    }
  }
}
