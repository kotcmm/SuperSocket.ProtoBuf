using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.ProtocolBuffers;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace ProtobufServer
{
    public class ProtobufAppServer<TMessage, TBuilder> :
        AppServer<ProtobufAppSession<TMessage, TBuilder>, ProtobufRequestInfo<TMessage, TBuilder>>
        where TMessage : GeneratedMessage<TMessage, TBuilder>
        where TBuilder : GeneratedBuilder<TMessage, TBuilder>, new()
    {
        public ProtobufAppServer()
            : base(new DefaultReceiveFilterFactory<ProtobufReceiveFilter<TMessage, TBuilder>, ProtobufRequestInfo<TMessage, TBuilder>>())
        {
        }
    }
}
