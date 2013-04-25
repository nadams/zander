using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zander.Provider.Net.Sockets.IO;

namespace Zander.UnitTests.Provider.Net.Sockets {

	[TestClass]
	public class CustomBinaryReaderTests {

		[TestMethod]
		public void GetString_NullTerminatedStringUsed_StringReturned() {
			using(var stream = this.GetStream("this is a test string\0")) {
				var reader = new CustomBinaryReader(stream, Encoding.ASCII);
				var value = reader.ReadString();

				Assert.AreEqual("this is a test string", value);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidDataException))]
		public void GetString_StringIsNotNullTerminated_InvalidDataExceptionThrown() {
			using(var stream = this.GetStream("this is not null terminated")) {
				var reader = new CustomBinaryReader(stream, Encoding.ASCII);
				var value = reader.ReadString();
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalBufferOverflowException))]
		public void GetString_StringIsLargerThanMaxSize_InvalidDataExceptionThrown() {
			using(var stream = this.GetStream("12345678\0")) {
				const int maxStringSize = 8;

				var reader = new CustomBinaryReader(stream, Encoding.ASCII, true, maxStringSize);
				var value = reader.ReadString();
			}
		}

		[TestMethod]
		public void GetString_StringOnlyNullCharacter_EmptyStringReturned() {
			using(var stream = this.GetStream("\0")) {
				const int maxStringSize = 8;

				var reader = new CustomBinaryReader(stream, Encoding.ASCII, true, maxStringSize);
				var value = reader.ReadString();

				Assert.AreEqual(string.Empty, value);
			}
		}

		private Stream GetStream(string content) {
			var stream = new MemoryStream();

			var data = Encoding.ASCII.GetBytes(content);
			stream.Write(data, 0, data.Length);
			stream.Flush();
			stream.Position = 0;

			return stream;
		}
	}
}
