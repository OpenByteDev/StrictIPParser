using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StrictIPParser.Test {
    [TestClass]
    public class IPv6AddressTests {
        [TestMethod]
        public void SucceedsOnValid() {
            ParseShouldSucceed("0:0:0:1:2:3:4:5");
            ParseShouldSucceed("1:2::3:4:5");
            ParseShouldSucceed("::1:2:3:4:5");
            ParseShouldSucceed("1:2:0:0:0:3:4:5");
            ParseShouldSucceed("1:2:3:4:5::");
            ParseShouldSucceed("1:2:3:4:5:0:0:0");
            ParseShouldSucceed("0:0:0:0:0:ffff:102:405");
            ParseShouldSucceed("::");
            ParseShouldSucceed("::0");
            ParseShouldSucceed("0:0:0::1");
            ParseShouldSucceed("::1:2:3:4:5");
            ParseShouldSucceed("ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff");

            static void ParseShouldSucceed(string text) {
                Assert.IsTrue(IPv6Address.TryParse(text, out IPv6Address actual));
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
            ParseShouldFail("0:0:0:1:2:3:4:5:0");
            ParseShouldFail("0:0:0:0:0:gfff:102:405");
            ParseShouldFail("0:0:0:0:0:fffff:102:405");
            ParseShouldFail("127.0.0.1");
            ParseShouldFail(":::");

            static void ParseShouldFail(string text) {
                Assert.IsFalse(IPv6Address.TryParse(text, out IPv6Address _));
            }
        }
    }
}
