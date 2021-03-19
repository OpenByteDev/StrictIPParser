using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StrictIPParser.Test {
    [TestClass]
    public class IPv6EndPointTests {
        [TestMethod]
        public void SucceedsOnValid() {
            ParseShouldSucceed("[0:0:0:1:2:3:4:5]:0");
            ParseShouldSucceed("[1:2::3:4:5]:65535");
            ParseShouldSucceed("[::1:2:3:4:5]:42");
            ParseShouldSucceed("[1:2:0:0:0:3:4:5]:69");
            ParseShouldSucceed("[1:2:3:4:5::]:0");
            ParseShouldSucceed("[1:2:3:4:5:0:0:0]:65535");
            ParseShouldSucceed("[0:0:0:0:0:ffff:102:405]:42");
            ParseShouldSucceed("[::]:69");
            ParseShouldSucceed("[::0]:0");
            ParseShouldSucceed("[0:0:0::1]:65535");
            ParseShouldSucceed("[::1:2:3:4:5]:42");

            static void ParseShouldSucceed(string text) {
                Assert.IsTrue(IPv6EndPoint.TryParse(text, out IPv6EndPoint actual));
                IPEndPoint expected = IPEndPoint.Parse(text);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expected.ToString(), actual.ToString());
            }
        }

        [TestMethod]
        public void FailsOnInvalid() {
            ParseShouldFail("0:0:0:1:2:3:4:5");
            ParseShouldFail("1:2::3:4:5");
            ParseShouldFail("::1:2:3:4:5");
            ParseShouldFail("1:2:0:0:0:3:4:5");
            ParseShouldFail("1:2:3:4:5::");
            ParseShouldFail("1:2:3:4:5:0:0:0");
            ParseShouldFail("0:0:0:0:0:ffff:102:405");
            ParseShouldFail("::");
            ParseShouldFail("::0");
            ParseShouldFail("0:0:0::1");
            ParseShouldFail("::1:2:3:4:5");

            ParseShouldFail("[0:0:0:1:2:3:4:5]:");
            ParseShouldFail("[1:2::3:4:5]:65536");
            ParseShouldFail("[::1:2:3:4:5]:0000000000");
            ParseShouldFail("[1:2:0:0:0:3:4:5]:-1");
            ParseShouldFail("[1:2:3:4:5::]:+1");
            ParseShouldFail("[1:2:3:4:5:0:0:0]:");
            ParseShouldFail("[0:0:0:0:0:ffff:102:405]:65536");
            ParseShouldFail("[::]:0000000000");
            ParseShouldFail("[::0]:-1");
            ParseShouldFail("[0:0:0::1]:+1");
            ParseShouldFail("[::1:2:3:4:5]:");

            ParseShouldFail("[0:0:0:1:2:3:4:5:0]:0");
            ParseShouldFail("[0:0:0:0:0:gfff:102:405]:65535");
            ParseShouldFail("[0:0:0:0:0:fffff:102:405]:42");
            ParseShouldFail("[127.0.0.1]:69");
            ParseShouldFail("[:::]:1234");

            static void ParseShouldFail(string text) {
                Assert.IsFalse(IPv6EndPoint.TryParse(text, out IPv6EndPoint _));
            }
        }
    }
}
