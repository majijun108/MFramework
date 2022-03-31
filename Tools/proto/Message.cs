// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: message.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace ServerMessage {

  /// <summary>Holder for reflection information generated from message.proto</summary>
  public static partial class MessageReflection {

    #region Descriptor
    /// <summary>File descriptor for message.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static MessageReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg1tZXNzYWdlLnByb3RvEg5zZXJ2ZXJfbWVzc2FnZSJtCgxDMlNfUm9vbUlu",
            "Zm8SEAoIc2VydmVySVAYASABKAkSEgoKc2VydmVyUG9ydBgCIAEoBRIQCghy",
            "b29tTmFtZRgDIAEoCRITCgtwbGF5ZXJDb3VudBgEIAEoBRIQCghtYXhDb3Vu",
            "dBgFIAEoBSJFCglDMlNfTG9jYWwSEAoIY2xpZW50SVAYASABKAkSEgoKY2xp",
            "ZW50UG9ydBgCIAEoBRISCgpwbGF5ZXJOYW1lGAMgASgJYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::ServerMessage.C2S_RoomInfo), global::ServerMessage.C2S_RoomInfo.Parser, new[]{ "ServerIP", "ServerPort", "RoomName", "PlayerCount", "MaxCount" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::ServerMessage.C2S_Local), global::ServerMessage.C2S_Local.Parser, new[]{ "ClientIP", "ClientPort", "PlayerName" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class C2S_RoomInfo : pb::IMessage<C2S_RoomInfo>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<C2S_RoomInfo> _parser = new pb::MessageParser<C2S_RoomInfo>(() => new C2S_RoomInfo());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<C2S_RoomInfo> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::ServerMessage.MessageReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public C2S_RoomInfo() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public C2S_RoomInfo(C2S_RoomInfo other) : this() {
      serverIP_ = other.serverIP_;
      serverPort_ = other.serverPort_;
      roomName_ = other.roomName_;
      playerCount_ = other.playerCount_;
      maxCount_ = other.maxCount_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public C2S_RoomInfo Clone() {
      return new C2S_RoomInfo(this);
    }

    /// <summary>Field number for the "serverIP" field.</summary>
    public const int ServerIPFieldNumber = 1;
    private string serverIP_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ServerIP {
      get { return serverIP_; }
      set {
        serverIP_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "serverPort" field.</summary>
    public const int ServerPortFieldNumber = 2;
    private int serverPort_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int ServerPort {
      get { return serverPort_; }
      set {
        serverPort_ = value;
      }
    }

    /// <summary>Field number for the "roomName" field.</summary>
    public const int RoomNameFieldNumber = 3;
    private string roomName_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string RoomName {
      get { return roomName_; }
      set {
        roomName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "playerCount" field.</summary>
    public const int PlayerCountFieldNumber = 4;
    private int playerCount_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int PlayerCount {
      get { return playerCount_; }
      set {
        playerCount_ = value;
      }
    }

    /// <summary>Field number for the "maxCount" field.</summary>
    public const int MaxCountFieldNumber = 5;
    private int maxCount_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int MaxCount {
      get { return maxCount_; }
      set {
        maxCount_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as C2S_RoomInfo);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(C2S_RoomInfo other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (ServerIP != other.ServerIP) return false;
      if (ServerPort != other.ServerPort) return false;
      if (RoomName != other.RoomName) return false;
      if (PlayerCount != other.PlayerCount) return false;
      if (MaxCount != other.MaxCount) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (ServerIP.Length != 0) hash ^= ServerIP.GetHashCode();
      if (ServerPort != 0) hash ^= ServerPort.GetHashCode();
      if (RoomName.Length != 0) hash ^= RoomName.GetHashCode();
      if (PlayerCount != 0) hash ^= PlayerCount.GetHashCode();
      if (MaxCount != 0) hash ^= MaxCount.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (ServerIP.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(ServerIP);
      }
      if (ServerPort != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(ServerPort);
      }
      if (RoomName.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(RoomName);
      }
      if (PlayerCount != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(PlayerCount);
      }
      if (MaxCount != 0) {
        output.WriteRawTag(40);
        output.WriteInt32(MaxCount);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (ServerIP.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(ServerIP);
      }
      if (ServerPort != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(ServerPort);
      }
      if (RoomName.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(RoomName);
      }
      if (PlayerCount != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(PlayerCount);
      }
      if (MaxCount != 0) {
        output.WriteRawTag(40);
        output.WriteInt32(MaxCount);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (ServerIP.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ServerIP);
      }
      if (ServerPort != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(ServerPort);
      }
      if (RoomName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(RoomName);
      }
      if (PlayerCount != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PlayerCount);
      }
      if (MaxCount != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(MaxCount);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(C2S_RoomInfo other) {
      if (other == null) {
        return;
      }
      if (other.ServerIP.Length != 0) {
        ServerIP = other.ServerIP;
      }
      if (other.ServerPort != 0) {
        ServerPort = other.ServerPort;
      }
      if (other.RoomName.Length != 0) {
        RoomName = other.RoomName;
      }
      if (other.PlayerCount != 0) {
        PlayerCount = other.PlayerCount;
      }
      if (other.MaxCount != 0) {
        MaxCount = other.MaxCount;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            ServerIP = input.ReadString();
            break;
          }
          case 16: {
            ServerPort = input.ReadInt32();
            break;
          }
          case 26: {
            RoomName = input.ReadString();
            break;
          }
          case 32: {
            PlayerCount = input.ReadInt32();
            break;
          }
          case 40: {
            MaxCount = input.ReadInt32();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            ServerIP = input.ReadString();
            break;
          }
          case 16: {
            ServerPort = input.ReadInt32();
            break;
          }
          case 26: {
            RoomName = input.ReadString();
            break;
          }
          case 32: {
            PlayerCount = input.ReadInt32();
            break;
          }
          case 40: {
            MaxCount = input.ReadInt32();
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class C2S_Local : pb::IMessage<C2S_Local>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<C2S_Local> _parser = new pb::MessageParser<C2S_Local>(() => new C2S_Local());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<C2S_Local> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::ServerMessage.MessageReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public C2S_Local() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public C2S_Local(C2S_Local other) : this() {
      clientIP_ = other.clientIP_;
      clientPort_ = other.clientPort_;
      playerName_ = other.playerName_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public C2S_Local Clone() {
      return new C2S_Local(this);
    }

    /// <summary>Field number for the "clientIP" field.</summary>
    public const int ClientIPFieldNumber = 1;
    private string clientIP_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ClientIP {
      get { return clientIP_; }
      set {
        clientIP_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "clientPort" field.</summary>
    public const int ClientPortFieldNumber = 2;
    private int clientPort_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int ClientPort {
      get { return clientPort_; }
      set {
        clientPort_ = value;
      }
    }

    /// <summary>Field number for the "playerName" field.</summary>
    public const int PlayerNameFieldNumber = 3;
    private string playerName_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string PlayerName {
      get { return playerName_; }
      set {
        playerName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as C2S_Local);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(C2S_Local other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (ClientIP != other.ClientIP) return false;
      if (ClientPort != other.ClientPort) return false;
      if (PlayerName != other.PlayerName) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (ClientIP.Length != 0) hash ^= ClientIP.GetHashCode();
      if (ClientPort != 0) hash ^= ClientPort.GetHashCode();
      if (PlayerName.Length != 0) hash ^= PlayerName.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (ClientIP.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(ClientIP);
      }
      if (ClientPort != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(ClientPort);
      }
      if (PlayerName.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(PlayerName);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (ClientIP.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(ClientIP);
      }
      if (ClientPort != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(ClientPort);
      }
      if (PlayerName.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(PlayerName);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (ClientIP.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ClientIP);
      }
      if (ClientPort != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(ClientPort);
      }
      if (PlayerName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(PlayerName);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(C2S_Local other) {
      if (other == null) {
        return;
      }
      if (other.ClientIP.Length != 0) {
        ClientIP = other.ClientIP;
      }
      if (other.ClientPort != 0) {
        ClientPort = other.ClientPort;
      }
      if (other.PlayerName.Length != 0) {
        PlayerName = other.PlayerName;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            ClientIP = input.ReadString();
            break;
          }
          case 16: {
            ClientPort = input.ReadInt32();
            break;
          }
          case 26: {
            PlayerName = input.ReadString();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            ClientIP = input.ReadString();
            break;
          }
          case 16: {
            ClientPort = input.ReadInt32();
            break;
          }
          case 26: {
            PlayerName = input.ReadString();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code