using NETStandard.HttpListener.Util;
using Xunit;

namespace NETStandard.HttpListener.Tests.Util
{
    public class HostParserUtilTests
    {
        [Fact]
        public void HostParserUtil_Parse_With_http()
        {
            // Assign
            string host = "http://test.com:8080";

            // Act
            var uri = HostParserUtil.Parse(host, "/abc?x=0");

            // Assert
            Assert.Equal("http://test.com:8080/abc?x=0", uri.ToString());
        }

        [Fact]
        public void HostParserUtil_Parse_With_https()
        {
            // Assign
            string host = "https://test.com:8080";

            // Act
            var uri = HostParserUtil.Parse(host, "/abc?x=0");

            // Assert
            Assert.Equal("https://test.com:8080/abc?x=0", uri.ToString());
        }

        [Fact]
        public void HostParserUtil_Parse()
        {
            // Assign
            string host = "test.com:8080";

            // Act
            var uri = HostParserUtil.Parse(host, "/abc?x=0");

            // Assert
            Assert.Equal("http://test.com:8080/abc?x=0", uri.ToString());
        }
    }
}