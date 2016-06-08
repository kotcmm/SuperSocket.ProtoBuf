using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.ProtocolBuffers;
using SuperSocket.SocketBase.Protocol;

namespace ProtobufServer
{
    public class ProtobufRequestInfo<TMessage, TBuilder> : IRequestInfo<TMessage>
        where TMessage : GeneratedMessage<TMessage, TBuilder>
        where TBuilder : GeneratedBuilder<TMessage, TBuilder>, new()
    {
        public ProtobufRequestInfo(TMessage body)
        {
            Body = body;
        }

        public TMessage Body { get; private set; }

        public string Key { get { return string.Empty; } }
    }
}
