#!/bin/sh
echo "Regenerates ProtoBuffer code"
echo "You only need to run this if you have modified any .proto file"
echo "To run this you will need the http://silentorbit.com/protobuf/ generator"
CodeGenerator Repos/Pipe/PipeMessages.proto
CodeGenerator Encryption/EncryptionHeader.proto
CodeGenerator Messages/Messages.proto
