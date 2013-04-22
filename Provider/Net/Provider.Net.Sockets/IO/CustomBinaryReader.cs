using System.IO;
using System.Linq;
using System.Text;

namespace Zander.Provider.Net.Sockets.IO {
	public class CustomBinaryReader : BinaryReader {
		private readonly Encoding encoding;
		public readonly int maxStringSize;

		public CustomBinaryReader(Stream input, Encoding encoding) : this(input, encoding, true) {
			this.encoding = encoding;
		}

		public CustomBinaryReader(Stream input, Encoding encoding, bool leaveOpen, int maxStringSize = 4096) : base(input, encoding, leaveOpen) {
			this.encoding = encoding;
			this.maxStringSize = maxStringSize;
		}

		/// <summary>
		/// Reads a null terminated string
		/// </summary>
		/// <exception cref="InvalidDataException">If the string is not null terminated</exception>
		/// <exception cref="InternalBufferOverflowException">The string being read is larger than the internal string size</exception>
		/// <returns>A string from a given stream</returns>
		public override string ReadString() {
			var bytes = new byte[maxStringSize];

			byte b;
			int index = 0;
			while((b = this.ReadByte()) > 0) {
				bytes[index] = b;

				index++;

				if(this.PeekChar() == -1) {
					throw new InvalidDataException("String is not null terminated");
				} 

				if(index >= maxStringSize) {
					throw new InternalBufferOverflowException(string.Format("String is larger than {0} bytes", maxStringSize));
				}
			}

			return this.encoding.GetString(bytes.TakeWhile(x => x > 0).ToArray());
		}
	}
}
