using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StrictIPParser.Test {
    [TestClass]
    public class PortTests {
        [TestMethod]
        public void SucceedsOnValid() {
            ParseShouldSucceed("0");
            ParseShouldSucceed("65535");
            ParseShouldSucceed("42");
            ParseShouldSucceed("69");

            static void ParseShouldSucceed(string text) {
                Assert.IsTrue(Port.TryParse(text, out Port actual));
                ushort expected = ushort.Parse(text);
                Assert.IsTrue(expected == actual);
            }
        }

        [TestMethod]
        public void FailsOnInvalid() {
            ParseShouldFail("");
            ParseShouldFail("65536");
            ParseShouldFail("0000000000");
            ParseShouldFail("-1");
            ParseShouldFail("+1");

            static void ParseShouldFail(string text) {
                Assert.IsFalse(Port.TryParse(text, out Port _));
            }
        }
    }
}
