using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Zander.Provider.Net.Sockets.IO {
	public class CustomBinaryReader : BinaryReader {
		private readonly Encoding encoding;

		public CustomBinaryReader(Stream input) : base(input) {
			this.encoding = Encoding.Default;
		}

		public CustomBinaryReader(Stream input, Encoding encoding) : base(input, encoding) {
			this.encoding = encoding;
		}

		public CustomBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen) {
			this.encoding = encoding;
		}

		public override string ReadString() {
			var bytes = new List<byte>();

			byte b;

			while((b = this.ReadByte()) > 0) {
				bytes.Add(b);
			}

			return this.encoding.GetString(bytes.ToArray());
		}
	}
}
