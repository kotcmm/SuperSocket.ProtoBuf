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
    /// <summary>
    ///     A decoder that splits the received {@link ByteBuf}s dynamically by the
    ///     value of the Google Protocol Buffers
    ///     <a href="http://code.google.com/apis/protocolbuffers/docs/encoding.html#varints">
    ///         Base
    ///         128 Varints
    ///     </a>
    ///     integer length field in the message.For example:
    ///    
    ///         BEFORE DECODE (302 bytes)       AFTER DECODE (300 bytes)
    ///         +--------+---------------+      +---------------+
    ///         | Length | Protobuf Data |----->| Protobuf Data |
    ///         | 0xAC02 |  (300 bytes)  |      |  (300 bytes)  |
    ///         +--------+---------------+      +---------------+
    /// </summary>
    public class ProtobufReceiveFilter<TMessage,TBuilder> : IReceiveFilter<ProtobufRequestInfo<TMessage, TBuilder>>, IOffsetAdapter, IReceiveFilterInitializer
        where TMessage : GeneratedMessage<TMessage, TBuilder>
        where TBuilder : GeneratedBuilder<TMessage, TBuilder>, new()
    {
        private int m_ParsedLength;

        private int m_OrigOffset;

        void IReceiveFilterInitializer.Initialize(IAppServer appServer, IAppSession session)
        {
            m_OrigOffset = session.SocketSession.OrigReceiveOffset;
        }

        public ProtobufRequestInfo<TMessage, TBuilder> Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            rest = 0;
            var readOffset = offset - m_OffsetDelta;

            CodedInputStream stream = CodedInputStream.CreateInstance(readBuffer, readOffset, length);
            var varint32 = (int)stream.ReadRawVarint32();
            if (varint32 <= 0) return null;

            var headLen = (int)stream.Position - readOffset;
            rest = length - varint32 - headLen + m_ParsedLength;

            if (rest >= 0)
            {
                byte[] body = stream.ReadRawBytes(varint32);

                TMessage message = CreateBuilder().MergeFrom(body).BuildParsed();
                var requestInfo = new ProtobufRequestInfo<TMessage, TBuilder>(message);
                InternalReset();
                return requestInfo;
            }
            else
            {
                m_ParsedLength += length;
                m_OffsetDelta = m_ParsedLength;
                rest = 0;

                var expectedOffset = offset + length;
                var newOffset = m_OrigOffset + m_OffsetDelta;

                if (newOffset < expectedOffset)
                {
                    Buffer.BlockCopy(readBuffer, offset - m_ParsedLength + length, readBuffer, m_OrigOffset, m_ParsedLength);
                }

                return null;
            }
        }

        public TBuilder CreateBuilder() { return new TBuilder(); }

        private int m_OffsetDelta;

        /// <summary>
        /// Gets the offset delta.
        /// </summary>
        int IOffsetAdapter.OffsetDelta
        {
            get { return m_OffsetDelta; }
        }

        private void InternalReset()
        {
            m_ParsedLength = 0;
            m_OffsetDelta = 0;
        }

        public void Reset()
        {
            InternalReset();
        }

        ProtobufRequestInfo<TMessage, TBuilder> IReceiveFilter<ProtobufRequestInfo<TMessage, TBuilder>>.Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            throw new NotImplementedException();
        }

        public int LeftBufferSize
        {
            get { return m_ParsedLength; }
        }

        public IReceiveFilter<ProtobufRequestInfo<TMessage, TBuilder>> NextReceiveFilter
        {
            get { return null; }
        }

        public FilterState State
        {
            get; protected set;
        }

        IReceiveFilter<ProtobufRequestInfo<TMessage, TBuilder>> IReceiveFilter<ProtobufRequestInfo<TMessage, TBuilder>>.NextReceiveFilter
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
