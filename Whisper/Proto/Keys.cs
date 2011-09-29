﻿//
//	You may customize this code as you like
//	Report bugs to: https://silentorbit.com/protobuf/
//
//	Generated by ProtocolBuffer
//	- a pure c# code generation implementation of protocol buffers
//

using System;
using System.Collections.Generic;

namespace Whisper.Keys
{
	public partial class PublicKey
	{
		public byte[] Modulus { get; set; }
		public byte[] Exponent { get; set; }
	
		//protected virtual void BeforeSerialize() {}
		//protected virtual void AfterDeserialize() {}
	}

}
namespace Whisper.Keys
{
	public partial class PrivateKey
	{
		protected byte[] Modulus { get; set; }
		protected byte[] Exponent { get; set; }
		protected byte[] P { get; set; }
		protected byte[] Q { get; set; }
	
		//protected virtual void BeforeSerialize() {}
		//protected virtual void AfterDeserialize() {}
	}

}
