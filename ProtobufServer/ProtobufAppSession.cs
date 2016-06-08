using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.ProtocolBuffers;
using SuperSocket.SocketBase;

namespace ProtobufServer
{
    public class ProtobufAppSession<TMessage, TBuilder> : 
        AppSession<ProtobufAppSession<TMessage, TBuilder>, ProtobufRequestInfo<TMessage, TBuilder>>
        where TMessage : GeneratedMessage<TMessage, TBuilder>
        where TBuilder : GeneratedBuilder<TMessage, TBuilder>, new()
    {

    }
}
