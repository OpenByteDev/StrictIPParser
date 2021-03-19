using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StrictIPParser.Test {
    [TestClass]
    public class IPv4EndPointTests {
        [TestMethod]
        public void SucceedsOnValid() {
            ParseShouldSucceed("127.0.0.1:0");
            ParseShouldSucceed("127.0.0.1:65535");
            ParseShouldSucceed("8.8.4.4:42");
            ParseShouldSucceed("255.255.255.255:69");

            static void ParseShouldSucceed(string text) {
                Assert.IsTrue(IPv4EndPoint.TryParse(text, out IPv4EndPoint actual));
                IPEndPoint expected = IPEndPoint.Parse(text);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expected.ToString(), actual.ToString());
            }
        }

        [TestMethod]
        public void FailsOnInvalid() {
            ParseShouldFail("127.0.0.:0");
            ParseShouldFail(".1.1.1:65535");
            ParseShouldFail("1.1.1.1::42");
            ParseShouldFail("8.8..4.4:1234");
            ParseShouldFail("256.256.256.256:69");
            ParseShouldFail("127.0.0.:");
            ParseShouldFail(".1.1.1:65536");
            ParseShouldFail("1.1.1.1:0000000000");
            ParseShouldFail("8.8..4.4:-1");
            ParseShouldFail("256.256.256.256:+1");

            static void ParseShouldFail(string text) {
                Assert.IsFalse(IPv4EndPoint.TryParse(text, out IPv4EndPoint _));
            }
        }
    }
}
