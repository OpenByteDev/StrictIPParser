using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StrictIPParser.Test {
    [TestClass]
    public class IPv4AddressTests {
        [TestMethod]
        public void SucceedsOnValid() {
            Assert.IsNotNull(IPv4Address.TryParse("127.0.0.1"));
            Assert.IsNotNull(IPv4Address.TryParse("1.1.1.1"));
            Assert.IsNotNull(IPv4Address.TryParse("8.8.4.4"));
            Assert.IsNotNull(IPv4Address.TryParse("255.255.255.255"));
        }

        [TestMethod]
        public void FailsOnInvalid() {
            Assert.IsNull(IPv4Address.TryParse("127.0.0."));
            Assert.IsNull(IPv4Address.TryParse(".1.1.1"));
            Assert.IsNull(IPv4Address.TryParse("8.8..4.4"));
            Assert.IsNull(IPv4Address.TryParse("256.256.256.256"));
        }
    }
}
