using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StrictIPParser.Test {
    [TestClass]
    public class IPv4AddressTests {
        [TestMethod]
        public void SucceedsOnValid() {
            ParseShouldSucceed("127.0.0.1");
            ParseShouldSucceed("1.1.1.1");
            ParseShouldSucceed("8.8.4.4");
            ParseShouldSucceed("255.255.255.255");

            static void ParseShouldSucceed(string text) {
                Assert.IsTrue(IPv4Address.TryParse(text, out IPv4Address actual));
                IPAddress expected = IPAddress.Parse(text);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expected.ToString(), actual.ToString());
                Assert.AreEqual(
                     string.Join(",", expected.GetAddressBytes()),
                     string.Join(",", actual.GetAddressBytes()));
            }
        }

        [TestMethod]
        public void FailsOnInvalid() {
            ParseShouldFail("127.0.0.");
            ParseShouldFail(".1.1.1");
            ParseShouldFail("8.8..4.4");
            ParseShouldFail("256.256.256.256");

            static void ParseShouldFail(string text) {
                Assert.IsFalse(IPv4Address.TryParse(text, out IPv4Address _));
            }
        }
    }
}
