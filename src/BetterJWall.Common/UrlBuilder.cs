using System;
using System.Collections.Generic;
using System.Text;

namespace BetterJWall.Common
{
    public static class UrlBuilder
    {
        private static string Build(string baseUrl, string parameter)
            => baseUrl + "/" + parameter;

        public static string BuildJira(string baseUrl, string parameter)
            => Build(baseUrl + "/browse/", parameter);
    }
}
