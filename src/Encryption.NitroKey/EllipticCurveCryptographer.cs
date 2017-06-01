using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Contract;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Net.Pkcs11Interop.HighLevelAPI.MechanismParams;

namespace Encryption.NitroKey
{
    public class EllipticCurveCryptographer
    {
        private const string UserPin = "648219";
        private const string LibraryPath = @"C:\Windows\System32\opensc-pkcs11.dll";

        /// <summary>
        /// DER-oid 1.3.36.3.3.2.8.1.1.9 (brainpoolP320r1)
        /// </summary>
        private const string BrainpoolOid = "06092b2403030208010109";

        private static ObjectHandle GetObjectHandle(string label, Session session, CKO keyType)
        {
            var objectAttributes = new List<ObjectAttribute>
            {
                new ObjectAttribute(CKA.CKA_CLASS, keyType),
                new ObjectAttribute(CKA.CKA_LABEL, label),
                new ObjectAttribute(CKA.CKA_TOKEN, true)
            };

            return session.FindAllObjects(objectAttributes).First();
        }

        private static byte[] GetDataFromObject(ObjectHandle handle, Session session, CKA type)
        {
            var attributes = new List<ulong> { (ulong)type };
            var requiredAttributes = session.GetAttributeValue(handle, attributes);
            return requiredAttributes[0].GetValueAsByteArray();
        }

        private static string GetStringFromObject(ObjectHandle handle, Session session, CKA type)
        {
            var attributes = new List<ulong> { (ulong)type };
            var requiredAttributes = session.GetAttributeValue(handle, attributes);
            return requiredAttributes[0].GetValueAsString();
        }

        public static EcKeyPairInfo[] GetEcKeyPairInfos()
        {
            var result = new List<EcKeyPairInfo>();

            using (Pkcs11 pk = new Pkcs11(LibraryPath, false))
            {
                var slots = pk.GetSlotList(true).Where(slot1 => slot1.GetSlotInfo().ManufacturerId == "Nitrokey");
                foreach (var slot in slots)
                {
                    using (Session session = slot.OpenSession(true))
                    {
                        var slotInfo = slot.GetSlotInfo();
                        var tokenInfo = slot.GetTokenInfo();

                        var objectAttributes = new List<ObjectAttribute>
                        {
                            new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_PUBLIC_KEY),
                            new ObjectAttribute(CKA.CKA_TOKEN, true)
                        };

                        foreach (var handle in session.FindAllObjects(objectAttributes))
                        {
                            var label = GetStringFromObject(handle, session, CKA.CKA_LABEL);
                            var @params = GetDataFromObject(handle, session, CKA.CKA_EC_PARAMS);
                            var ecPoint = GetDataFromObject(handle, session, CKA.CKA_EC_POINT);

                            result.Add(new EcKeyPairInfo
                            {
                                Label = label,
                                ECParamsData=@params,
                                CurveDescription = Contract.CurveHelper.GetCurveDescriptionFromEcParam(@params),
                                ManufacturerId = slotInfo.ManufacturerId,
                                TokenLabel = tokenInfo.Label,
                                TokenSerialNumber = tokenInfo.SerialNumber,
                                PublicKey = EcKeyPair.CreateFromAnsi(ecPoint),
                        });
                        }
                    }
                }
            }

            return result.ToArray();
        }

        public static EcKeyPair GetPublicKey(string name)
        {
            byte[] ecPoint = null;

            using (Pkcs11 pk = new Pkcs11(LibraryPath, false))
            {
                var slot = pk.GetSlotList(false).First(slot1 => slot1.GetSlotInfo().ManufacturerId == "Nitrokey");

                using (Session session = slot.OpenSession(false))
                {
                    session.Login(CKU.CKU_USER, UserPin);
                    // CKO_PUBLIC_KEY, see https://www.cryptsoft.com/pkcs11doc/v220/group__SEC__12__3__3__ECDSA__PUBLIC__KEY__OBJECTS.html
                    var objectPublic = GetObjectHandle(name, session, CKO.CKO_PUBLIC_KEY);
                    var @params = GetDataFromObject(objectPublic, session, CKA.CKA_EC_PARAMS);
                    ecPoint = GetDataFromObject(objectPublic, session, CKA.CKA_EC_POINT);

                    var paramsHex = BitConverter.ToString(@params).ToLower().Replace("-", null);
                    if (paramsHex != BrainpoolOid)
                        throw new Exception();

                }
            }

            return EcKeyPair.CreateFromAnsi(ecPoint);
        }

        public static byte[] DeriveSecret(string name, EcKeyPair publicKeyPair)
        {
            using (Pkcs11 pk = new Pkcs11(LibraryPath, false))
            {
                var slot = pk.GetSlotList(false).First(slot1 => slot1.GetSlotInfo().ManufacturerId == "Nitrokey");

                using (Session session = slot.OpenSession(false))
                {
                    session.Login(CKU.CKU_USER, UserPin);

                    var objectPrivate = GetObjectHandle(name, session, CKO.CKO_PRIVATE_KEY);

                    var publicKey = publicKeyPair.ToDre();

                    byte[] data = session.GenerateRandom(32);
                    var mechanism = new Mechanism(CKM.CKM_ECDH1_DERIVE, new CkEcdh1DeriveParams(0, null, publicKey));

                    var deriveAttributes = new List<ObjectAttribute>
                    {
                        new ObjectAttribute(CKA.CKA_TOKEN, false),
                        new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_SECRET_KEY),
                        new ObjectAttribute(CKA.CKA_KEY_TYPE, CKK.CKK_GENERIC_SECRET),
                        new ObjectAttribute(CKA.CKA_SENSITIVE, false),
                        new ObjectAttribute(CKA.CKA_EXTRACTABLE, true),
                        new ObjectAttribute(CKA.CKA_ENCRYPT, true),
                        new ObjectAttribute(CKA.CKA_DECRYPT, true),
                        new ObjectAttribute(CKA.CKA_WRAP, true),
                        new ObjectAttribute(CKA.CKA_UNWRAP, true),
                        new ObjectAttribute(CKA.CKA_VALUE_LEN, 320/8),
                    };

                    var derivedKey = session.DeriveKey(mechanism, objectPrivate, deriveAttributes);

                    var derivedSecret = GetDataFromObject(derivedKey, session, CKA.CKA_VALUE);

                    return SHA512.Create().ComputeHash(derivedSecret);
                }
            }
        }
    }
}