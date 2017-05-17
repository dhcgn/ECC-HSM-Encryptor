using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Contract.Test
{
    [TestFixture]
    public class MessageTest
    {
        [Test]
        public void CargoData_Null()
        {
            var msg = new Message();
            Assert.That(msg.CargoData, Is.Null);
        }

        [Test]
        public void CargoData_NotNull()
        {
            var msg = new Message {Cargo = new Cargo()};
            Assert.That(msg.CargoData, Is.Not.Null);
        }

        [Test]
        public void CargoData_Full()
        {
            var msg = CreateMessage();

            Assert.That(msg.CargoData, Is.Not.Null);

            var json = JsonConvert.SerializeObject(msg);
            Console.Out.WriteLine(json);
        }

        [Test]
        public void CargoData_Convert()
        {
            var msg = CreateMessage();

            Assert.That(msg.CargoData, Is.Not.Null);

            var json = JsonConvert.SerializeObject(msg);
            var fromJson = JsonConvert.DeserializeObject<Message>(json);

            Assert.That(fromJson.Cargo.EncryptedData.Data, Is.EqualTo(msg.Cargo.EncryptedData.Data));
        }

        private static Message CreateMessage()
        {
            var rng = RandomNumberGenerator.Create();

            byte[] CreateRandom(int i)
            {
                var data = new byte[i];
                rng.GetBytes(data);
                return data;
            }

            var msg = new Message
            {
                ProofOfWork = new ProofOfWork
                {
                    Difficulty = 10,
                    Proof = Int32.MaxValue
                },
                Cargo = new Cargo
                {
                    EphemeralEcKeyPair = new EcKeyPair
                    {
                        PublicKey = new PublicKey
                        {
                            Qx = CreateRandom(320 / 2 / 8),
                            Qy = CreateRandom(320 / 2 / 8)
                        },
                        PrivateKey = CreateRandom(320 / 8)
                    },
                    EncryptedData = new EncryptedData
                    {
                        Data = CreateRandom(1024),
                        Iv = CreateRandom(128 / 8),
                    },
                    HMAC = CreateRandom(512 / 8)
                }
            };
            return msg;
        }
    }
}