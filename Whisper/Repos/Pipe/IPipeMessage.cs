using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Whisper.Repos.Pipe
{
	public interface IPipeMessage
	{
		uint TypeID { get; }
	}
}
