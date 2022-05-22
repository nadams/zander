// Code generated by protoc-gen-go. DO NOT EDIT.
// versions:
// 	protoc-gen-go v1.28.0
// 	protoc        v3.19.4
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

type Server struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Id        string                 `protobuf:"bytes,1,opt,name=id,proto3" json:"id,omitempty"`
	Name      string                 `protobuf:"bytes,2,opt,name=name,proto3" json:"name,omitempty"`
	Status    string                 `protobuf:"bytes,3,opt,name=status,proto3" json:"status,omitempty"`
	StartedAt *timestamppb.Timestamp `protobuf:"bytes,4,opt,name=started_at,json=startedAt,proto3" json:"started_at,omitempty"`
}

func (x *Server) Reset() {
	*x = Server{}
	if protoimpl.UnsafeEnabled {
		mi := &file_zproto_server_proto_msgTypes[2]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *Server) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*Server) ProtoMessage() {}

func (x *Server) ProtoReflect() protoreflect.Message {
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

// Deprecated: Use Server.ProtoReflect.Descriptor instead.
func (*Server) Descriptor() ([]byte, []int) {
	return file_zproto_server_proto_rawDescGZIP(), []int{2}
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

func (x *Server) GetStatus() string {
	if x != nil {
		return x.Status
	}
	return ""
}

func (x *Server) GetStartedAt() *timestamppb.Timestamp {
	if x != nil {
		return x.StartedAt
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
	0x72, 0x76, 0x65, 0x72, 0x73, 0x22, 0x7f, 0x0a, 0x06, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x12,
	0x0e, 0x0a, 0x02, 0x69, 0x64, 0x18, 0x01, 0x20, 0x01, 0x28, 0x09, 0x52, 0x02, 0x69, 0x64, 0x12,
	0x12, 0x0a, 0x04, 0x6e, 0x61, 0x6d, 0x65, 0x18, 0x02, 0x20, 0x01, 0x28, 0x09, 0x52, 0x04, 0x6e,
	0x61, 0x6d, 0x65, 0x12, 0x16, 0x0a, 0x06, 0x73, 0x74, 0x61, 0x74, 0x75, 0x73, 0x18, 0x03, 0x20,
	0x01, 0x28, 0x09, 0x52, 0x06, 0x73, 0x74, 0x61, 0x74, 0x75, 0x73, 0x12, 0x39, 0x0a, 0x0a, 0x73,
	0x74, 0x61, 0x72, 0x74, 0x65, 0x64, 0x5f, 0x61, 0x74, 0x18, 0x04, 0x20, 0x01, 0x28, 0x0b, 0x32,
	0x1a, 0x2e, 0x67, 0x6f, 0x6f, 0x67, 0x6c, 0x65, 0x2e, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x62, 0x75,
	0x66, 0x2e, 0x54, 0x69, 0x6d, 0x65, 0x73, 0x74, 0x61, 0x6d, 0x70, 0x52, 0x09, 0x73, 0x74, 0x61,
	0x72, 0x74, 0x65, 0x64, 0x41, 0x74, 0x32, 0x52, 0x0a, 0x06, 0x5a, 0x61, 0x6e, 0x64, 0x65, 0x72,
	0x12, 0x48, 0x0a, 0x0b, 0x4c, 0x69, 0x73, 0x74, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x73, 0x12,
	0x1a, 0x2e, 0x7a, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x2e, 0x4c, 0x69, 0x73, 0x74, 0x53, 0x65, 0x72,
	0x76, 0x65, 0x72, 0x73, 0x52, 0x65, 0x71, 0x75, 0x65, 0x73, 0x74, 0x1a, 0x1b, 0x2e, 0x7a, 0x70,
	0x72, 0x6f, 0x74, 0x6f, 0x2e, 0x4c, 0x69, 0x73, 0x74, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x73,
	0x52, 0x65, 0x73, 0x70, 0x6f, 0x6e, 0x73, 0x65, 0x22, 0x00, 0x42, 0x28, 0x5a, 0x26, 0x67, 0x69,
	0x74, 0x6c, 0x61, 0x62, 0x2e, 0x6e, 0x6f, 0x64, 0x65, 0x2d, 0x33, 0x2e, 0x6e, 0x65, 0x74, 0x2f,
	0x6e, 0x61, 0x64, 0x61, 0x6d, 0x73, 0x2f, 0x7a, 0x61, 0x6e, 0x64, 0x65, 0x72, 0x2f, 0x7a, 0x70,
	0x72, 0x6f, 0x74, 0x6f, 0x62, 0x06, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x33,
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

var file_zproto_server_proto_msgTypes = make([]protoimpl.MessageInfo, 3)
var file_zproto_server_proto_goTypes = []interface{}{
	(*ListServersRequest)(nil),    // 0: zproto.ListServersRequest
	(*ListServersResponse)(nil),   // 1: zproto.ListServersResponse
	(*Server)(nil),                // 2: zproto.Server
	(*timestamppb.Timestamp)(nil), // 3: google.protobuf.Timestamp
}
var file_zproto_server_proto_depIdxs = []int32{
	2, // 0: zproto.ListServersResponse.servers:type_name -> zproto.Server
	3, // 1: zproto.Server.started_at:type_name -> google.protobuf.Timestamp
	0, // 2: zproto.Zander.ListServers:input_type -> zproto.ListServersRequest
	1, // 3: zproto.Zander.ListServers:output_type -> zproto.ListServersResponse
	3, // [3:4] is the sub-list for method output_type
	2, // [2:3] is the sub-list for method input_type
	2, // [2:2] is the sub-list for extension type_name
	2, // [2:2] is the sub-list for extension extendee
	0, // [0:2] is the sub-list for field type_name
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
	}
	type x struct{}
	out := protoimpl.TypeBuilder{
		File: protoimpl.DescBuilder{
			GoPackagePath: reflect.TypeOf(x{}).PkgPath(),
			RawDescriptor: file_zproto_server_proto_rawDesc,
			NumEnums:      0,
			NumMessages:   3,
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
