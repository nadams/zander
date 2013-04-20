using System.IO;
using System.Text;

namespace Zander.Provider.Net.Sockets.IO {
	public class HuffmanWriter : BinaryWriter {

		private readonly Encoding encoding;

		public HuffmanWriter(Encoding stringEncoding) {
			this.encoding = stringEncoding;
		}

		public override void Write(string value) {
			var bytes = this.encoding.GetBytes(value);

			this.Write(bytes);
			this.Write((byte)0);
		}
	}
}
