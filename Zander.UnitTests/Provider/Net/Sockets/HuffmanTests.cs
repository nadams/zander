using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zander.Provider.Net.Sockets.IO;

namespace Zander.UnitTests.Provider.Net.Sockets {

	[TestClass]
	public class HuffmanTests {
		
		[TestMethod]
		public void Encode_EncodeData_MatchesPreEncodedData() {
			var preEncodedData = new byte[] { 255, 124, 93, 86, 0, 2, 0 };

			var dataToEncode = new byte[] { 124, 93, 86, 0, 2, 0 };

			var huffmanWrapper = new HuffmanWrapper();
			var encodedData = new byte[1024];

			var encodedLength = huffmanWrapper.Encode(dataToEncode, encodedData, dataToEncode.Length);
			encodedData = encodedData.Take(encodedLength).ToArray();

			Assert.IsTrue(Enumerable.SequenceEqual(preEncodedData, encodedData));
		}

		[TestMethod]
		public void Decode_DecodeData_MatchesPreDecodedData() {
			var rawData = new byte[] { 124, 93, 86, 0, 2, 0 };

			var huffmanWrapper = new HuffmanWrapper();
			var decodedData = new byte[1024];

			var preEncodedData = new byte[] { 255, 124, 93, 86, 0, 2, 0 };
			var decodedLength = huffmanWrapper.Decode(preEncodedData, decodedData, preEncodedData.Length);
			decodedData = decodedData.Take(decodedLength).ToArray();

			Assert.IsTrue(Enumerable.SequenceEqual(rawData, decodedData));
		}
	}
}
