
# Getting started

This code have few included dependencies so you should be able to directly
open the Whisper.sln solution in either MonoDevelop or VisualStudio.

# Protocol Buffers

Whisper is using Googles [Protocol Buffer](http://code.google.com/apis/protocolbuffers/docs/overview.html) 
for storage and the communication protocol.

We are using the [ProtoBuf](http://silentorbit.com/protobuf/)
to generate C# code from the .proto specifications.

The generated code is included in the code repo
and you only need to run the tool if you want to modify the .proto files.


