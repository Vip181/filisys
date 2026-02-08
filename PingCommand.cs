using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using System.Text;

namespace filesys.Commands
{
    public static class PingCommand
    {
        public static string Execute(string target)
        {
            StringBuilder sb = new StringBuilder();

            int packetSent = 0;
            int packetReceived = 0;
            int packetLost = 0;

            Address destination = Address.Parse(target);
            Address source;

            if (destination == null)
            {
                // DNS
                var dns = new DnsClient();
                dns.Connect(DNSConfig.DNSNameservers[0]);
                dns.SendAsk(target);
                destination = dns.Receive();
                dns.Close();

                if (destination == null)
                {
                    return "Ping error: unknown host";
                }
            }

            source = IPConfig.FindNetwork(destination);

            try
            {
                sb.AppendLine("Pinging " + destination.ToString());

                var client = new ICMPClient();
                client.Connect(destination);

                for (int i = 0; i < 4; i++)
                {
                    packetSent++;
                    client.SendEcho();

                    var endpoint = new EndPoint(Address.Zero, 0);
                    int time = client.Receive(ref endpoint, 4000);

                    if (time == -1)
                    {
                        sb.AppendLine("Request timed out.");
                        packetLost++;
                    }
                    else
                    {
                        sb.AppendLine(
                            "Reply from " + endpoint.Address.ToString() +
                            " time=" + (time < 1 ? "<1" : time.ToString()) + "s"
                        );
                        packetReceived++;
                    }
                }

                client.Close();
            }
            catch
            {
                return "Ping error: network failure";
            }

            int percentLoss = packetLost * 25;

            sb.AppendLine("");
            sb.AppendLine("Ping statistics:");
            sb.AppendLine(
                " Sent = " + packetSent +
                ", Received = " + packetReceived +
                ", Lost = " + packetLost +
                " (" + percentLoss + "% loss)"
            );

            return sb.ToString();
        }
    }
}
