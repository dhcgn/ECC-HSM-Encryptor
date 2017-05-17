using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract;

namespace Encryption
{
    internal class Core
    {
        public Core()
        {
            
        }
    }

    interface IEncryptor
    {
        byte[] DeriveSecret(string privateKeyId, EcKeyPair ecPublicKey);
        EcKeyPair GetEcPublicKey(string privateKeyId);
    }

    public class LocalEncryptor : IEncryptor
    {
        public byte[] DeriveSecret(string privateKeyId, EcKeyPair ecPublicKey)
        {
            throw new NotImplementedException();
        }

        public EcKeyPair GetEcPublicKey(string privateKeyId)
        {
            throw new NotImplementedException();
        }
    }

}
