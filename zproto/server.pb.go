// Code generated by protoc-gen-go. DO NOT EDIT.
// versions:
// 	protoc-gen-go v1.28.1
// 	protoc        v3.21.12
// source: zproto/server.proto

package zproto

import (
	protoreflect "google.golang.org/protobuf/reflect/protoreflect"
	protoimpl "google.golang.org/protobuf/runtime/protoimpl"
	timestamppb "google.golang.org/protobuf/types/known/timestamppb"
	reflect "reflect"
	sync "sync"
)

const (
	// Verify that this generated code is sufficiently up-to-date.
	_ = protoimpl.EnforceVersion(20 - protoimpl.MinVersion)
	// Verify that runtime/protoimpl is sufficiently up-to-date.
	_ = protoimpl.EnforceVersion(protoimpl.MaxVersion - 20)
)

type ListServersRequest struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields
}

func (x *ListServersRequest) Reset() {
	*x = ListServersRequest{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[0]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *ListServersRequest) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*ListServersRequest) ProtoMessage() {}

func (x *ListServersRequest) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[0]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use ListServersRequest.ProtoReflect.Descriptor instead.
func (*ListServersRequest) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{0}
}

type ListServersResponse struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Servers []*Server `protobuf:"bytes,1,rep,name=servers,proto3" json:"servers,omitempty"`
}

func (x *ListServersResponse) Reset() {
	*x = ListServersResponse{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[1]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *ListServersResponse) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*ListServersResponse) ProtoMessage() {}

func (x *ListServersResponse) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[1]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use ListServersResponse.ProtoReflect.Descriptor instead.
func (*ListServersResponse) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{1}
}

func (x *ListServersResponse) GetServers() []*Server {
	if x != nil {
		return x.Servers
	}
	return nil
}

type RestartServerRequest struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Ids []string `protobuf:"bytes,1,rep,name=ids,proto3" json:"ids,omitempty"`
}

func (x *RestartServerRequest) Reset() {
	*x = RestartServerRequest{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[2]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *RestartServerRequest) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*RestartServerRequest) ProtoMessage() {}

func (x *RestartServerRequest) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[2]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use RestartServerRequest.ProtoReflect.Descriptor instead.
func (*RestartServerRequest) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{2}
}

func (x *RestartServerRequest) GetIds() []string {
	if x != nil {
		return x.Ids
	}
	return nil
}

type RestartServerResponse struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Success []string `protobuf:"bytes,1,rep,name=success,proto3" json:"success,omitempty"`
	Failure []string `protobuf:"bytes,2,rep,name=failure,proto3" json:"failure,omitempty"`
}

func (x *RestartServerResponse) Reset() {
	*x = RestartServerResponse{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[3]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *RestartServerResponse) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*RestartServerResponse) ProtoMessage() {}

func (x *RestartServerResponse) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[3]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use RestartServerResponse.ProtoReflect.Descriptor instead.
func (*RestartServerResponse) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{3}
}

func (x *RestartServerResponse) GetSuccess() []string {
	if x != nil {
		return x.Success
	}
	return nil
}

func (x *RestartServerResponse) GetFailure() []string {
	if x != nil {
		return x.Failure
	}
	return nil
}

type StopServerRequest struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Ids []string `protobuf:"bytes,1,rep,name=ids,proto3" json:"ids,omitempty"`
}

func (x *StopServerRequest) Reset() {
	*x = StopServerRequest{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[4]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *StopServerRequest) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*StopServerRequest) ProtoMessage() {}

func (x *StopServerRequest) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[4]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use StopServerRequest.ProtoReflect.Descriptor instead.
func (*StopServerRequest) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{4}
}

func (x *StopServerRequest) GetIds() []string {
	if x != nil {
		return x.Ids
	}
	return nil
}

type StopServerResponse struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Success []string `protobuf:"bytes,1,rep,name=success,proto3" json:"success,omitempty"`
	Failure []string `protobuf:"bytes,2,rep,name=failure,proto3" json:"failure,omitempty"`
}

func (x *StopServerResponse) Reset() {
	*x = StopServerResponse{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[5]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *StopServerResponse) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*StopServerResponse) ProtoMessage() {}

func (x *StopServerResponse) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[5]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use StopServerResponse.ProtoReflect.Descriptor instead.
func (*StopServerResponse) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{5}
}

func (x *StopServerResponse) GetSuccess() []string {
	if x != nil {
		return x.Success
	}
	return nil
}

func (x *StopServerResponse) GetFailure() []string {
	if x != nil {
		return x.Failure
	}
	return nil
}

type StartServerRequest struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Ids []string `protobuf:"bytes,1,rep,name=ids,proto3" json:"ids,omitempty"`
}

func (x *StartServerRequest) Reset() {
	*x = StartServerRequest{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[6]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *StartServerRequest) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*StartServerRequest) ProtoMessage() {}

func (x *StartServerRequest) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[6]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use StartServerRequest.ProtoReflect.Descriptor instead.
func (*StartServerRequest) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{6}
}

func (x *StartServerRequest) GetIds() []string {
	if x != nil {
		return x.Ids
	}
	return nil
}

type StartServerResponse struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Success []string `protobuf:"bytes,1,rep,name=success,proto3" json:"success,omitempty"`
	Failure []string `protobuf:"bytes,2,rep,name=failure,proto3" json:"failure,omitempty"`
}

func (x *StartServerResponse) Reset() {
	*x = StartServerResponse{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[7]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *StartServerResponse) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*StartServerResponse) ProtoMessage() {}

func (x *StartServerResponse) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[7]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use StartServerResponse.ProtoReflect.Descriptor instead.
func (*StartServerResponse) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{7}
}

func (x *StartServerResponse) GetSuccess() []string {
	if x != nil {
		return x.Success
	}
	return nil
}

func (x *StartServerResponse) GetFailure() []string {
	if x != nil {
		return x.Failure
	}
	return nil
}

type Server struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Id        string                 `protobuf:"bytes,1,opt,name=id,proto3" json:"id,omitempty"`
	Name      string                 `protobuf:"bytes,2,opt,name=name,proto3" json:"name,omitempty"`
	Engine    string                 `protobuf:"bytes,3,opt,name=engine,proto3" json:"engine,omitempty"`
	Mode      string                 `protobuf:"bytes,4,opt,name=mode,proto3" json:"mode,omitempty"`
	Status    string                 `protobuf:"bytes,5,opt,name=status,proto3" json:"status,omitempty"`
	Port      int32                  `protobuf:"varint,6,opt,name=port,proto3" json:"port,omitempty"`
	Iwad      string                 `protobuf:"bytes,7,opt,name=iwad,proto3" json:"iwad,omitempty"`
	Pwads     []string               `protobuf:"bytes,8,rep,name=pwads,proto3" json:"pwads,omitempty"`
	StartedAt *timestamppb.Timestamp `protobuf:"bytes,9,opt,name=started_at,json=startedAt,proto3" json:"started_at,omitempty"`
	StoppedAt *timestamppb.Timestamp `protobuf:"bytes,10,opt,name=stopped_at,json=stoppedAt,proto3" json:"stopped_at,omitempty"`
}

func (x *Server) Reset() {
	*x = Server{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[8]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *Server) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*Server) ProtoMessage() {}

func (x *Server) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[8]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use Server.ProtoReflect.Descriptor instead.
func (*Server) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{8}
}

func (x *Server) GetId() string {
	if x != nil {
		return x.Id
	}
	return ""
}

func (x *Server) GetName() string {
	if x != nil {
		return x.Name
	}
	return ""
}

func (x *Server) GetEngine() string {
	if x != nil {
		return x.Engine
	}
	return ""
}

func (x *Server) GetMode() string {
	if x != nil {
		return x.Mode
	}
	return ""
}

func (x *Server) GetStatus() string {
	if x != nil {
		return x.Status
	}
	return ""
}

func (x *Server) GetPort() int32 {
	if x != nil {
		return x.Port
	}
	return 0
}

func (x *Server) GetIwad() string {
	if x != nil {
		return x.Iwad
	}
	return ""
}

func (x *Server) GetPwads() []string {
	if x != nil {
		return x.Pwads
	}
	return nil
}

func (x *Server) GetStartedAt() *timestamppb.Timestamp {
	if x != nil {
		return x.StartedAt
	}
	return nil
}

func (x *Server) GetStoppedAt() *timestamppb.Timestamp {
	if x != nil {
		return x.StoppedAt
	}
	return nil
}

type AttachIn struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Id      string `protobuf:"bytes,1,opt,name=id,proto3" json:"id,omitempty"`
	Content []byte `protobuf:"bytes,2,opt,name=content,proto3" json:"content,omitempty"`
}

func (x *AttachIn) Reset() {
	*x = AttachIn{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[9]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *AttachIn) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*AttachIn) ProtoMessage() {}

func (x *AttachIn) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[9]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use AttachIn.ProtoReflect.Descriptor instead.
func (*AttachIn) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{9}
}

func (x *AttachIn) GetId() string {
	if x != nil {
		return x.Id
	}
	return ""
}

func (x *AttachIn) GetContent() []byte {
	if x != nil {
		return x.Content
	}
	return nil
}

type AttachOut struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Content []byte `protobuf:"bytes,1,opt,name=content,proto3" json:"content,omitempty"`
}

func (x *AttachOut) Reset() {
	*x = AttachOut{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[10]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *AttachOut) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*AttachOut) ProtoMessage() {}

func (x *AttachOut) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[10]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use AttachOut.ProtoReflect.Descriptor instead.
func (*AttachOut) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{10}
}

func (x *AttachOut) GetContent() []byte {
	if x != nil {
		return x.Content
	}
	return nil
}

type TailIn struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Id string `protobuf:"bytes,1,opt,name=id,proto3" json:"id,omitempty"`
}

func (x *TailIn) Reset() {
	*x = TailIn{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[11]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *TailIn) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*TailIn) ProtoMessage() {}

func (x *TailIn) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[11]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use TailIn.ProtoReflect.Descriptor instead.
func (*TailIn) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{11}
}

func (x *TailIn) GetId() string {
	if x != nil {
		return x.Id
	}
	return ""
}

type TailOut struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Content []byte `protobuf:"bytes,1,opt,name=content,proto3" json:"content,omitempty"`
}

func (x *TailOut) Reset() {
	*x = TailOut{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[12]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *TailOut) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*TailOut) ProtoMessage() {}

func (x *TailOut) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[12]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use TailOut.ProtoReflect.Descriptor instead.
func (*TailOut) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{12}
}

func (x *TailOut) GetContent() []byte {
	if x != nil {
		return x.Content
	}
	return nil
}

type LogsIn struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Id  string `protobuf:"bytes,1,opt,name=id,proto3" json:"id,omitempty"`
	Num int32  `protobuf:"varint,2,opt,name=num,proto3" json:"num,omitempty"`
}

func (x *LogsIn) Reset() {
	*x = LogsIn{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[13]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *LogsIn) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*LogsIn) ProtoMessage() {}

func (x *LogsIn) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[13]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use LogsIn.ProtoReflect.Descriptor instead.
func (*LogsIn) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{13}
}

func (x *LogsIn) GetId() string {
	if x != nil {
		return x.Id
	}
	return ""
}

func (x *LogsIn) GetNum() int32 {
	if x != nil {
		return x.Num
	}
	return 0
}

type LogsOut struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Content []byte `protobuf:"bytes,1,opt,name=content,proto3" json:"content,omitempty"`
}

func (x *LogsOut) Reset() {
	*x = LogsOut{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[14]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *LogsOut) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*LogsOut) ProtoMessage() {}

func (x *LogsOut) ProtoReflect() protoreflect.Message {
	mi := &file_zproto_server_proto_msgTypes[14]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use LogsOut.ProtoReflect.Descriptor instead.
func (*LogsOut) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{14}
}

func (x *LogsOut) GetContent() []byte {
	if x != nil {
		return x.Content
	}
	return nil
}

var File_zproto_server_proto protoreflect.FileDescriptor

var file_zproto_server_proto_rawDesc = []byte{
	0x0a, 0x13, 0x7a, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x2f, 0x73, 0x65, 0x72, 0x76, 0x65, 0x72, 0x2e,
	0x70, 0x72, 0x6f, 0x74, 0x6f, 0x12, 0x06, 0x7a, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x1a, 0x1f, 0x67,
	0x6f, 0x6f, 0x67, 0x6c, 0x65, 0x2f, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x62, 0x75, 0x66, 0x2f, 0x74,
	0x69, 0x6d, 0x65, 0x73, 0x74, 0x61, 0x6d, 0x70, 0x2e, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x22, 0x14,
	0x0a, 0x12, 0x4c, 0x69, 0x73, 0x74, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x73, 0x52, 0x65, 0x71,
	0x75, 0x65, 0x73, 0x74, 0x22, 0x3f, 0x0a, 0x13, 0x4c, 0x69, 0x73, 0x74, 0x53, 0x65, 0x72, 0x76,
	0x65, 0x72, 0x73, 0x52, 0x65, 0x73, 0x70, 0x6f, 0x6e, 0x73, 0x65, 0x12, 0x28, 0x0a, 0x07, 0x73,
	0x65, 0x72, 0x76, 0x65, 0x72, 0x73, 0x18, 0x01, 0x20, 0x03, 0x28, 0x0b, 0x32, 0x0e, 0x2e, 0x7a,
	0x70, 0x72, 0x6f, 0x74, 0x6f, 0x2e, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x52, 0x07, 0x73, 0x65,
	0x72, 0x76, 0x65, 0x72, 0x73, 0x22, 0x28, 0x0a, 0x14, 0x52, 0x65, 0x73, 0x74, 0x61, 0x72, 0x74,
	0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x52, 0x65, 0x71, 0x75, 0x65, 0x73, 0x74, 0x12, 0x10, 0x0a,
	0x03, 0x69, 0x64, 0x73, 0x18, 0x01, 0x20, 0x03, 0x28, 0x09, 0x52, 0x03, 0x69, 0x64, 0x73, 0x22,
	0x4b, 0x0a, 0x15, 0x52, 0x65, 0x73, 0x74, 0x61, 0x72, 0x74, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72,
	0x52, 0x65, 0x73, 0x70, 0x6f, 0x6e, 0x73, 0x65, 0x12, 0x18, 0x0a, 0x07, 0x73, 0x75, 0x63, 0x63,
	0x65, 0x73, 0x73, 0x18, 0x01, 0x20, 0x03, 0x28, 0x09, 0x52, 0x07, 0x73, 0x75, 0x63, 0x63, 0x65,
	0x73, 0x73, 0x12, 0x18, 0x0a, 0x07, 0x66, 0x61, 0x69, 0x6c, 0x75, 0x72, 0x65, 0x18, 0x02, 0x20,
	0x03, 0x28, 0x09, 0x52, 0x07, 0x66, 0x61, 0x69, 0x6c, 0x75, 0x72, 0x65, 0x22, 0x25, 0x0a, 0x11,
	0x53, 0x74, 0x6f, 0x70, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x52, 0x65, 0x71, 0x75, 0x65, 0x73,
	0x74, 0x12, 0x10, 0x0a, 0x03, 0x69, 0x64, 0x73, 0x18, 0x01, 0x20, 0x03, 0x28, 0x09, 0x52, 0x03,
	0x69, 0x64, 0x73, 0x22, 0x48, 0x0a, 0x12, 0x53, 0x74, 0x6f, 0x70, 0x53, 0x65, 0x72, 0x76, 0x65,
	0x72, 0x52, 0x65, 0x73, 0x70, 0x6f, 0x6e, 0x73, 0x65, 0x12, 0x18, 0x0a, 0x07, 0x73, 0x75, 0x63,
	0x63, 0x65, 0x73, 0x73, 0x18, 0x01, 0x20, 0x03, 0x28, 0x09, 0x52, 0x07, 0x73, 0x75, 0x63, 0x63,
	0x65, 0x73, 0x73, 0x12, 0x18, 0x0a, 0x07, 0x66, 0x61, 0x69, 0x6c, 0x75, 0x72, 0x65, 0x18, 0x02,
	0x20, 0x03, 0x28, 0x09, 0x52, 0x07, 0x66, 0x61, 0x69, 0x6c, 0x75, 0x72, 0x65, 0x22, 0x26, 0x0a,
	0x12, 0x53, 0x74, 0x61, 0x72, 0x74, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x52, 0x65, 0x71, 0x75,
	0x65, 0x73, 0x74, 0x12, 0x10, 0x0a, 0x03, 0x69, 0x64, 0x73, 0x18, 0x01, 0x20, 0x03, 0x28, 0x09,
	0x52, 0x03, 0x69, 0x64, 0x73, 0x22, 0x49, 0x0a, 0x13, 0x53, 0x74, 0x61, 0x72, 0x74, 0x53, 0x65,
	0x72, 0x76, 0x65, 0x72, 0x52, 0x65, 0x73, 0x70, 0x6f, 0x6e, 0x73, 0x65, 0x12, 0x18, 0x0a, 0x07,
	0x73, 0x75, 0x63, 0x63, 0x65, 0x73, 0x73, 0x18, 0x01, 0x20, 0x03, 0x28, 0x09, 0x52, 0x07, 0x73,
	0x75, 0x63, 0x63, 0x65, 0x73, 0x73, 0x12, 0x18, 0x0a, 0x07, 0x66, 0x61, 0x69, 0x6c, 0x75, 0x72,
	0x65, 0x18, 0x02, 0x20, 0x03, 0x28, 0x09, 0x52, 0x07, 0x66, 0x61, 0x69, 0x6c, 0x75, 0x72, 0x65,
	0x22, 0xa4, 0x02, 0x0a, 0x06, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x12, 0x0e, 0x0a, 0x02, 0x69,
	0x64, 0x18, 0x01, 0x20, 0x01, 0x28, 0x09, 0x52, 0x02, 0x69, 0x64, 0x12, 0x12, 0x0a, 0x04, 0x6e,
	0x61, 0x6d, 0x65, 0x18, 0x02, 0x20, 0x01, 0x28, 0x09, 0x52, 0x04, 0x6e, 0x61, 0x6d, 0x65, 0x12,
	0x16, 0x0a, 0x06, 0x65, 0x6e, 0x67, 0x69, 0x6e, 0x65, 0x18, 0x03, 0x20, 0x01, 0x28, 0x09, 0x52,
	0x06, 0x65, 0x6e, 0x67, 0x69, 0x6e, 0x65, 0x12, 0x12, 0x0a, 0x04, 0x6d, 0x6f, 0x64, 0x65, 0x18,
	0x04, 0x20, 0x01, 0x28, 0x09, 0x52, 0x04, 0x6d, 0x6f, 0x64, 0x65, 0x12, 0x16, 0x0a, 0x06, 0x73,
	0x74, 0x61, 0x74, 0x75, 0x73, 0x18, 0x05, 0x20, 0x01, 0x28, 0x09, 0x52, 0x06, 0x73, 0x74, 0x61,
	0x74, 0x75, 0x73, 0x12, 0x12, 0x0a, 0x04, 0x70, 0x6f, 0x72, 0x74, 0x18, 0x06, 0x20, 0x01, 0x28,
	0x05, 0x52, 0x04, 0x70, 0x6f, 0x72, 0x74, 0x12, 0x12, 0x0a, 0x04, 0x69, 0x77, 0x61, 0x64, 0x18,
	0x07, 0x20, 0x01, 0x28, 0x09, 0x52, 0x04, 0x69, 0x77, 0x61, 0x64, 0x12, 0x14, 0x0a, 0x05, 0x70,
	0x77, 0x61, 0x64, 0x73, 0x18, 0x08, 0x20, 0x03, 0x28, 0x09, 0x52, 0x05, 0x70, 0x77, 0x61, 0x64,
	0x73, 0x12, 0x39, 0x0a, 0x0a, 0x73, 0x74, 0x61, 0x72, 0x74, 0x65, 0x64, 0x5f, 0x61, 0x74, 0x18,
	0x09, 0x20, 0x01, 0x28, 0x0b, 0x32, 0x1a, 0x2e, 0x67, 0x6f, 0x6f, 0x67, 0x6c, 0x65, 0x2e, 0x70,
	0x72, 0x6f, 0x74, 0x6f, 0x62, 0x75, 0x66, 0x2e, 0x54, 0x69, 0x6d, 0x65, 0x73, 0x74, 0x61, 0x6d,
	0x70, 0x52, 0x09, 0x73, 0x74, 0x61, 0x72, 0x74, 0x65, 0x64, 0x41, 0x74, 0x12, 0x39, 0x0a, 0x0a,
	0x73, 0x74, 0x6f, 0x70, 0x70, 0x65, 0x64, 0x5f, 0x61, 0x74, 0x18, 0x0a, 0x20, 0x01, 0x28, 0x0b,
	0x32, 0x1a, 0x2e, 0x67, 0x6f, 0x6f, 0x67, 0x6c, 0x65, 0x2e, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x62,
	0x75, 0x66, 0x2e, 0x54, 0x69, 0x6d, 0x65, 0x73, 0x74, 0x61, 0x6d, 0x70, 0x52, 0x09, 0x73, 0x74,
	0x6f, 0x70, 0x70, 0x65, 0x64, 0x41, 0x74, 0x22, 0x34, 0x0a, 0x08, 0x41, 0x74, 0x74, 0x61, 0x63,
	0x68, 0x49, 0x6e, 0x12, 0x0e, 0x0a, 0x02, 0x69, 0x64, 0x18, 0x01, 0x20, 0x01, 0x28, 0x09, 0x52,
	0x02, 0x69, 0x64, 0x12, 0x18, 0x0a, 0x07, 0x63, 0x6f, 0x6e, 0x74, 0x65, 0x6e, 0x74, 0x18, 0x02,
	0x20, 0x01, 0x28, 0x0c, 0x52, 0x07, 0x63, 0x6f, 0x6e, 0x74, 0x65, 0x6e, 0x74, 0x22, 0x25, 0x0a,
	0x09, 0x41, 0x74, 0x74, 0x61, 0x63, 0x68, 0x4f, 0x75, 0x74, 0x12, 0x18, 0x0a, 0x07, 0x63, 0x6f,
	0x6e, 0x74, 0x65, 0x6e, 0x74, 0x18, 0x01, 0x20, 0x01, 0x28, 0x0c, 0x52, 0x07, 0x63, 0x6f, 0x6e,
	0x74, 0x65, 0x6e, 0x74, 0x22, 0x18, 0x0a, 0x06, 0x54, 0x61, 0x69, 0x6c, 0x49, 0x6e, 0x12, 0x0e,
	0x0a, 0x02, 0x69, 0x64, 0x18, 0x01, 0x20, 0x01, 0x28, 0x09, 0x52, 0x02, 0x69, 0x64, 0x22, 0x23,
	0x0a, 0x07, 0x54, 0x61, 0x69, 0x6c, 0x4f, 0x75, 0x74, 0x12, 0x18, 0x0a, 0x07, 0x63, 0x6f, 0x6e,
	0x74, 0x65, 0x6e, 0x74, 0x18, 0x01, 0x20, 0x01, 0x28, 0x0c, 0x52, 0x07, 0x63, 0x6f, 0x6e, 0x74,
	0x65, 0x6e, 0x74, 0x22, 0x2a, 0x0a, 0x06, 0x4c, 0x6f, 0x67, 0x73, 0x49, 0x6e, 0x12, 0x0e, 0x0a,
	0x02, 0x69, 0x64, 0x18, 0x01, 0x20, 0x01, 0x28, 0x09, 0x52, 0x02, 0x69, 0x64, 0x12, 0x10, 0x0a,
	0x03, 0x6e, 0x75, 0x6d, 0x18, 0x02, 0x20, 0x01, 0x28, 0x05, 0x52, 0x03, 0x6e, 0x75, 0x6d, 0x22,
	0x23, 0x0a, 0x07, 0x4c, 0x6f, 0x67, 0x73, 0x4f, 0x75, 0x74, 0x12, 0x18, 0x0a, 0x07, 0x63, 0x6f,
	0x6e, 0x74, 0x65, 0x6e, 0x74, 0x18, 0x01, 0x20, 0x01, 0x28, 0x0c, 0x52, 0x07, 0x63, 0x6f, 0x6e,
	0x74, 0x65, 0x6e, 0x74, 0x32, 0xc0, 0x03, 0x0a, 0x06, 0x5a, 0x61, 0x6e, 0x64, 0x65, 0x72, 0x12,
	0x48, 0x0a, 0x0b, 0x4c, 0x69, 0x73, 0x74, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x73, 0x12, 0x1a,
	0x2e, 0x7a, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x2e, 0x4c, 0x69, 0x73, 0x74, 0x53, 0x65, 0x72, 0x76,
	0x65, 0x72, 0x73, 0x52, 0x65, 0x71, 0x75, 0x65, 0x73, 0x74, 0x1a, 0x1b, 0x2e, 0x7a, 0x70, 0x72,
	0x6f, 0x74, 0x6f, 0x2e, 0x4c, 0x69, 0x73, 0x74, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x73, 0x52,
	0x65, 0x73, 0x70, 0x6f, 0x6e, 0x73, 0x65, 0x22, 0x00, 0x12, 0x48, 0x0a, 0x0b, 0x53, 0x74, 0x61,
	0x72, 0x74, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x12, 0x1a, 0x2e, 0x7a, 0x70, 0x72, 0x6f, 0x74,
	0x6f, 0x2e, 0x53, 0x74, 0x61, 0x72, 0x74, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x52, 0x65, 0x71,
	0x75, 0x65, 0x73, 0x74, 0x1a, 0x1b, 0x2e, 0x7a, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x2e, 0x53, 0x74,
	0x61, 0x72, 0x74, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x52, 0x65, 0x73, 0x70, 0x6f, 0x6e, 0x73,
	0x65, 0x22, 0x00, 0x12, 0x45, 0x0a, 0x0a, 0x53, 0x74, 0x6f, 0x70, 0x53, 0x65, 0x72, 0x76, 0x65,
	0x72, 0x12, 0x19, 0x2e, 0x7a, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x2e, 0x53, 0x74, 0x6f, 0x70, 0x53,
	0x65, 0x72, 0x76, 0x65, 0x72, 0x52, 0x65, 0x71, 0x75, 0x65, 0x73, 0x74, 0x1a, 0x1a, 0x2e, 0x7a,
	0x70, 0x72, 0x6f, 0x74, 0x6f, 0x2e, 0x53, 0x74, 0x6f, 0x70, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72,
	0x52, 0x65, 0x73, 0x70, 0x6f, 0x6e, 0x73, 0x65, 0x22, 0x00, 0x12, 0x4e, 0x0a, 0x0d, 0x52, 0x65,
	0x73, 0x74, 0x61, 0x72, 0x74, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x12, 0x1c, 0x2e, 0x7a, 0x70,
	0x72, 0x6f, 0x74, 0x6f, 0x2e, 0x52, 0x65, 0x73, 0x74, 0x61, 0x72, 0x74, 0x53, 0x65, 0x72, 0x76,
	0x65, 0x72, 0x52, 0x65, 0x71, 0x75, 0x65, 0x73, 0x74, 0x1a, 0x1d, 0x2e, 0x7a, 0x70, 0x72, 0x6f,
	0x74, 0x6f, 0x2e, 0x52, 0x65, 0x73, 0x74, 0x61, 0x72, 0x74, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72,
	0x52, 0x65, 0x73, 0x70, 0x6f, 0x6e, 0x73, 0x65, 0x22, 0x00, 0x12, 0x33, 0x0a, 0x06, 0x41, 0x74,
	0x74, 0x61, 0x63, 0x68, 0x12, 0x10, 0x2e, 0x7a, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x2e, 0x41, 0x74,
	0x74, 0x61, 0x63, 0x68, 0x49, 0x6e, 0x1a, 0x11, 0x2e, 0x7a, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x2e,
	0x41, 0x74, 0x74, 0x61, 0x63, 0x68, 0x4f, 0x75, 0x74, 0x22, 0x00, 0x28, 0x01, 0x30, 0x01, 0x12,
	0x2b, 0x0a, 0x04, 0x54, 0x61, 0x69, 0x6c, 0x12, 0x0e, 0x2e, 0x7a, 0x70, 0x72, 0x6f, 0x74, 0x6f,
	0x2e, 0x54, 0x61, 0x69, 0x6c, 0x49, 0x6e, 0x1a, 0x0f, 0x2e, 0x7a, 0x70, 0x72, 0x6f, 0x74, 0x6f,
	0x2e, 0x54, 0x61, 0x69, 0x6c, 0x4f, 0x75, 0x74, 0x22, 0x00, 0x30, 0x01, 0x12, 0x29, 0x0a, 0x04,
	0x4c, 0x6f, 0x67, 0x73, 0x12, 0x0e, 0x2e, 0x7a, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x2e, 0x4c, 0x6f,
	0x67, 0x73, 0x49, 0x6e, 0x1a, 0x0f, 0x2e, 0x7a, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x2e, 0x4c, 0x6f,
	0x67, 0x73, 0x4f, 0x75, 0x74, 0x22, 0x00, 0x42, 0x28, 0x5a, 0x26, 0x67, 0x69, 0x74, 0x6c, 0x61,
	0x62, 0x2e, 0x6e, 0x6f, 0x64, 0x65, 0x2d, 0x33, 0x2e, 0x6e, 0x65, 0x74, 0x2f, 0x6e, 0x61, 0x64,
	0x61, 0x6d, 0x73, 0x2f, 0x7a, 0x61, 0x6e, 0x64, 0x65, 0x72, 0x2f, 0x7a, 0x70, 0x72, 0x6f, 0x74,
	0x6f, 0x62, 0x06, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x33,
}

var (
	file_zproto_server_proto_rawDescOnce sync.Once
	file_zproto_server_proto_rawDescData = file_zproto_server_proto_rawDesc
)

func file_zproto_server_proto_rawDescGZIP() []byte {
	file_zproto_server_proto_rawDescOnce.Do(func() {
		file_zproto_server_proto_rawDescData = protoimpl.X.CompressGZIP(file_zproto_server_proto_rawDescData)
	})
	return file_zproto_server_proto_rawDescData
}

var file_zproto_server_proto_msgTypes = make([]protoimpl.MessageInfo, 15)
var file_zproto_server_proto_goTypes = []interface{}{
	(*ListServersRequest)(nil),    // 0: zproto.ListServersRequest
	(*ListServersResponse)(nil),   // 1: zproto.ListServersResponse
	(*RestartServerRequest)(nil),  // 2: zproto.RestartServerRequest
	(*RestartServerResponse)(nil), // 3: zproto.RestartServerResponse
	(*StopServerRequest)(nil),     // 4: zproto.StopServerRequest
	(*StopServerResponse)(nil),    // 5: zproto.StopServerResponse
	(*StartServerRequest)(nil),    // 6: zproto.StartServerRequest
	(*StartServerResponse)(nil),   // 7: zproto.StartServerResponse
	(*Server)(nil),                // 8: zproto.Server
	(*AttachIn)(nil),              // 9: zproto.AttachIn
	(*AttachOut)(nil),             // 10: zproto.AttachOut
	(*TailIn)(nil),                // 11: zproto.TailIn
	(*TailOut)(nil),               // 12: zproto.TailOut
	(*LogsIn)(nil),                // 13: zproto.LogsIn
	(*LogsOut)(nil),               // 14: zproto.LogsOut
	(*timestamppb.Timestamp)(nil), // 15: google.protobuf.Timestamp
}
var file_zproto_server_proto_depIdxs = []int32{
	8,  // 0: zproto.ListServersResponse.servers:type_name -> zproto.Server
	15, // 1: zproto.Server.started_at:type_name -> google.protobuf.Timestamp
	15, // 2: zproto.Server.stopped_at:type_name -> google.protobuf.Timestamp
	0,  // 3: zproto.Zander.ListServers:input_type -> zproto.ListServersRequest
	6,  // 4: zproto.Zander.StartServer:input_type -> zproto.StartServerRequest
	4,  // 5: zproto.Zander.StopServer:input_type -> zproto.StopServerRequest
	2,  // 6: zproto.Zander.RestartServer:input_type -> zproto.RestartServerRequest
	9,  // 7: zproto.Zander.Attach:input_type -> zproto.AttachIn
	11, // 8: zproto.Zander.Tail:input_type -> zproto.TailIn
	13, // 9: zproto.Zander.Logs:input_type -> zproto.LogsIn
	1,  // 10: zproto.Zander.ListServers:output_type -> zproto.ListServersResponse
	7,  // 11: zproto.Zander.StartServer:output_type -> zproto.StartServerResponse
	5,  // 12: zproto.Zander.StopServer:output_type -> zproto.StopServerResponse
	3,  // 13: zproto.Zander.RestartServer:output_type -> zproto.RestartServerResponse
	10, // 14: zproto.Zander.Attach:output_type -> zproto.AttachOut
	12, // 15: zproto.Zander.Tail:output_type -> zproto.TailOut
	14, // 16: zproto.Zander.Logs:output_type -> zproto.LogsOut
	10, // [10:17] is the sub-list for method output_type
	3,  // [3:10] is the sub-list for method input_type
	3,  // [3:3] is the sub-list for extension type_name
	3,  // [3:3] is the sub-list for extension extendee
	0,  // [0:3] is the sub-list for field type_name
}

func init() { file_zproto_server_proto_init() }
func file_zproto_server_proto_init() {
	if File_zproto_server_proto != nil {
		return
	}
	if !protoimpl.UnsafeEnabled {
		file_zproto_server_proto_msgTypes[0].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*ListServersRequest); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_zproto_server_proto_msgTypes[1].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*ListServersResponse); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_zproto_server_proto_msgTypes[2].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*RestartServerRequest); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_zproto_server_proto_msgTypes[3].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*RestartServerResponse); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_zproto_server_proto_msgTypes[4].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*StopServerRequest); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_zproto_server_proto_msgTypes[5].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*StopServerResponse); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_zproto_server_proto_msgTypes[6].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*StartServerRequest); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_zproto_server_proto_msgTypes[7].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*StartServerResponse); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_zproto_server_proto_msgTypes[8].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*Server); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_zproto_server_proto_msgTypes[9].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*AttachIn); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_zproto_server_proto_msgTypes[10].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*AttachOut); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_zproto_server_proto_msgTypes[11].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*TailIn); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_zproto_server_proto_msgTypes[12].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*TailOut); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_zproto_server_proto_msgTypes[13].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*LogsIn); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_zproto_server_proto_msgTypes[14].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*LogsOut); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
	}
	type x struct{}
	out := protoimpl.TypeBuilder{
		File: protoimpl.DescBuilder{
			GoPackagePath: reflect.TypeOf(x{}).PkgPath(),
			RawDescriptor: file_zproto_server_proto_rawDesc,
			NumEnums:      0,
			NumMessages:   15,
			NumExtensions: 0,
			NumServices:   1,
		},
		GoTypes:           file_zproto_server_proto_goTypes,
		DependencyIndexes: file_zproto_server_proto_depIdxs,
		MessageInfos:      file_zproto_server_proto_msgTypes,
	}.Build()
	File_zproto_server_proto = out.File
	file_zproto_server_proto_rawDesc = nil
	file_zproto_server_proto_goTypes = nil
	file_zproto_server_proto_depIdxs = nil
}
