using System;
using System.Collections.Generic;
using Xunit;

namespace Undersoft.SDK.IntegrationTests.Instant
{
    public class ProxyTest
    {
        [Fact]
        public void Proxy_Test_For_Compilated_Type_With_GetByName()
        {
            var profile = new Agreement()
            {
                Comments = "fdfdsgg",
                Created = DateTime.Now,
                Kind = AgreementKind.Agree,
                Creator = "sfssd",
                VersionId = 222
            };

            var Proxy = new ProxyCreator<Agreement>();

            profile.AutoId();

            var _Proxy0 = profile.ToProxy();
            var _Proxy1 = Proxy.Create(_Proxy0);

            var _InstantSeries = new InstantSeriesCreator(_Proxy1);

            var mock = new Agreement()
            {
                Comments = "fdsfsf",
                Created = DateTime.Now,
                Kind = AgreementKind.Cancellation,
                Creator = "fsds",
                VersionId = 992
            };

            var prop = Proxy.Rubrics;
            List<IProxy> list = new();
            for (int i = 0; i < 300000; i++)
            {
                var _Proxy2 = Proxy.Create();
                foreach (var rubric in prop)
                {
                    _Proxy2[rubric.RubricName] = _Proxy0[rubric.RubricName];
                }
                list.Add(_Proxy2);
            }

            _Proxy1["TypeId"] = 1005UL;
            object o = _Proxy1[5];
        }

        [Fact]
        private void Proxy_Test_For_Compilated_Type_With_GetById()
        {
            var profile = new Agreement()
            {
                Comments = "fdfdsgg",
                Created = DateTime.Now,
                Kind = AgreementKind.Agree,
                Creator = "sfssd",
                VersionId = 222
            };

            var Proxy = new ProxyCreator<Agreement>();

            profile.AutoId();

            var _Proxy0 = Proxy.Create(profile);
            ((IInnerProxy)_Proxy0)[0] = 1;

            var _Proxy1 = Proxy.Create(_Proxy0);

            var mock = new Agreement()
            {
                Comments = "fdsfsf",
                Created = DateTime.Now,
                Kind = AgreementKind.Cancellation,
                Creator = "fsds",
                VersionId = 992
            };

            var prop = Proxy.Rubrics;
            List<IProxy> list = new();
            for (int i = 0; i < 300000; i++)
            {
                var _Proxy2 = Proxy.Create();
                for (int j = 0; j < prop.Count; j++)
                {
                    _Proxy2[j] = _Proxy0[j];
                }
                list.Add(_Proxy2);
            }

            _Proxy1["TypeId"] = 1005L;
            object o = _Proxy1[5];
        }
    }
}
