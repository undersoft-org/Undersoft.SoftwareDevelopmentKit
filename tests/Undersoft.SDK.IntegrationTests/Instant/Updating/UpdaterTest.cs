using Undersoft.SDK.Instant;
using Undersoft.SDK.Instant.Updating;
using System.Collections.Generic;
using Xunit;
using System;

namespace Undersoft.SDK.IntegrationTests.Instant.Updating
{
    public class UpdaterTest
    {
        [Fact]
        public void updater_Integrated_Test()
        {
            var profile = new Agreement()
            {
                Comments = "fdfdsgg",
                Created = DateTime.Now,
                Kind = AgreementKind.Agree,
                Creator = "sfssd",
                VersionId = 222,
                Formats = new Listing<AgreementFormat>()
                {
                    new AgreementFormat() { Id = 10, Name = "telefon" },
                    new AgreementFormat() { Id = 15, Name = "skan" }
                },
                Versions = new Listing<AgreementVersion>()
                {
                    new AgreementVersion()
                    {
                        Id = 20,
                        VersionNumber = 2,
                        Texts = new Listing<AgreementText>()
                        {
                            new AgreementText()
                            {
                                VersionId = 2,
                                Language = "en",
                                Content = "dfsdgdsdfsgfd"
                            },
                            new AgreementText()
                            {
                                VersionId = 2,
                                Language = "pl",
                                Content = "telefon"
                            }
                        }
                    },
                    new AgreementVersion()
                    {
                        Id = 25,
                        VersionNumber = 5,
                        Texts = new Listing<AgreementText>()
                        {
                            new AgreementText()
                            {
                                VersionId = 5,
                                Language = "en",
                                Content = "dfsdgdsdfsgfd"
                            },
                            new AgreementText()
                            {
                                VersionId = 5,
                                Language = "pl",
                                Content = "telefon"
                            }
                        }
                    }
                }
            };

            var updater6 = new Updater<Agreement>(profile);

            updater6.MapDevisor();

            var Proxy = new ProxyCreator<Agreement>();
            var updater0 = new Updater<UserProfile>();
            var updater1 = new Updater<User>();

            var user = new User()
            {
                Email = "jflskdfjlkdj",
                FirstName = "hfdjkfsjdkh",
                LastName = "fdlfhjdsk",
                OperationDate = DateTime.Now
            };

            user.Sign(user);

            var updater3 = new Updater<User>(user);

            var userprofile = new UserProfile()
            {
                Email = "jflskdfjlkdj",
                Name = "hfdjkfsjdkh",
                Surname = "fdlfhjdsk",
                Created = DateTime.Now
            };

            userprofile.Sign(userprofile);

            var updater5 = new Updater<UserProfile>(userprofile);

            var _Proxy0 = Proxy.Create(profile);
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

            List<IUpdater> list = new();
            for (int i = 0; i < 300000; i++)
            {
                var updater2 = new Updater<Agreement>();

                // updater2.ValueArray = _Proxy0.ValueArray;

                list.Add(updater2);
            }

            updater0.Entry.ChipNumber = 1005L;
            updater0.Entry.Name = "nnnnnnn";
            updater0.Entry.SSOID = new Guid();
            updater0.Id = 93939393L;

            updater0.Patch(updater1);
            updater0.Patch(profile);
            updater1.Put(_Proxy0);

            updater3.Patch(updater6);

            var c = updater5.Preset;
            c.City = "dfdfhdsdh";
            c.FacebookId = "dfklsdfk";

            updater5.MapDevisor();

            ((Agreement)_Proxy0).TypeId = 1005L;
            var uk = _Proxy0.Id;
            var k2 = _Proxy0["Id"];
            _Proxy0.Id = 2500L;

            _Proxy0["Id"] = 1005L;
            _Proxy0["TypeId"] = 1005L;
            object o = _Proxy1[5];

            profile.Time = DateTime.Now;

            var serial = new Uscn(profile.CodeNo);
        }
    }
}
