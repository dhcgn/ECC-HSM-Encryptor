using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using TestHelper;

namespace Encryption.NitroKey.Test
{
    [TestFixture]
    public class EllipticCurveCryptographerTest  : TestHelper.Helper
    {
        [Test]
        public void ExtractPublicKey()
        {
            var result = Encryption.NitroKey.EllipticCurveCryptographer.GetPublicKey("Brainpool #1", Constants.TestPin);
            Console.Out.WriteLine(result.ToJson);

            Console.Out.WriteLine(base.ToHexString(result.PublicKey.Qx));
            Console.Out.WriteLine(base.ToHexString(result.PublicKey.Qy));

            Assert.That(result.PublicKey.Qx, Has.Length.EqualTo(40));
            Assert.That(result.PublicKey.Qy, Has.Length.EqualTo(40));

            Assert.That(base.ToHexString(result.PublicKey.Qx), Is.EqualTo("0defed7988b095679e5aca422382d949c2e7fd937646def25cf7eb5140d41f12d077ac615773952d"));
            Assert.That(base.ToHexString(result.PublicKey.Qy), Is.EqualTo("a53efad266c8642c6877b8b215d091ba687acafd3c33f35ffb5ca6aadaf248ef1a126cd55e0d0598"));

            Assert.DoesNotThrow(() => result.CreateECParameters());
        }

        [Test]
        public void Create_ECDiffieHellman()
        {
            var result = Encryption.NitroKey.EllipticCurveCryptographer.GetPublicKey("Brainpool #1", Constants.TestPin);
            Console.Out.WriteLine(result.ToJson);

            Assert.DoesNotThrow(() => ECDiffieHellman.Create(result.CreateECParameters()));
        }

        [Test]
        public void DeriveSecretWithHsm()
        {
            var alice = Encryption.EllipticCurveCryptographer.CreateKeyPair(true);
            var bob = Encryption.NitroKey.EllipticCurveCryptographer.GetPublicKey("Brainpool #1", Constants.TestPin);

            var salt = Random.CreateSalt();

            var derivedSecret1 = Encryption.EllipticCurveCryptographer.DeriveSecret(alice, bob.ExportPublicKey());
            var derivedSecret2 = Encryption.NitroKey.EllipticCurveCryptographer.DeriveSecret("Brainpool #1", alice.ExportPublicKey(), Constants.TestPin);

            Console.Out.WriteLine($"derivedSecret NET length: {derivedSecret1?.Length * 8} bit");
            Console.Out.WriteLine($"derivedSecret HSM length: {derivedSecret2?.Length * 8} bit");

            Console.Out.WriteLine($"derivedSecret NET : {Convert.ToBase64String(derivedSecret1).Substring(0, 16)} ...");
            Console.Out.WriteLine($"derivedSecret HSM : {Convert.ToBase64String(derivedSecret2).Substring(0, 16)} ...");

            Assert.That(derivedSecret1, Has.Length.GreaterThan(0));
            Assert.That(derivedSecret2, Has.Length.GreaterThan(0));

            Assert.That(derivedSecret1, Is.EquivalentTo(derivedSecret2));
        }

        [Test]
        public void DeriveSecretWithoutHsm()
        {
            var alice = Encryption.EllipticCurveCryptographer.CreateKeyPair(true);
            var bob = Encryption.NitroKey.EllipticCurveCryptographer.GetPublicKey("Brainpool #1", Constants.TestPin);

            var salt = Random.CreateSalt();

            var derivedSecret1 = Encryption.EllipticCurveCryptographer.DeriveSecret(alice, bob.ExportPublicKey());

            Console.Out.WriteLine($"derivedSecret length: {derivedSecret1?.Length * 8} bit");
            Console.Out.WriteLine($"derivedSecret1 : {Convert.ToBase64String(derivedSecret1).Substring(0, 16)} ...");

            Assert.That(derivedSecret1, Has.Length.GreaterThan(0));
        }

        [Test]
        public void GetEcKeyPairInfos()
        {
            var tokens = Encryption.NitroKey.EllipticCurveCryptographer.GetEcKeyPairInfos();

            for (var index = 0; index < tokens.Length; index++)
            {
                var token = tokens[index];
                Console.Out.WriteLine($"{index+1}. " + "\r\n"+
                                      $"Label:       {token.EcIdentifier.KeyLabel}, " + "\r\n" +
                                      $"EC Params:   {base.ToHexString(token.ECParamsData)}" + "\r\n" +
                                      $"Curve Desc:  {token.CurveDescription}" + "\r\n" +
                                      $"Token Label: {token.TokenLabel}" + "\r\n" +
                                      $"Token Serial Number: \"{token.EcIdentifier.TokenSerialNumber}" + "\r\n" +
                                      $"ManufacturerId:      \"{token.ManufacturerId}" + "\r\n" +
                                      $"PublicKey:\r\n" +
                                      $"{token.PublicKey.ToArmor()}");
            }

            Console.WriteLine(JsonConvert.SerializeObject(tokens,Formatting.Indented));

            Assert.That(tokens.Length, Is.GreaterThan(0));
        }
    }
}
