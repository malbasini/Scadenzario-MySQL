using System;

namespace Scadenze.Models.Exceptions.Application
{
    public class BeneficiarioNotFoundException : Exception
    {
        public BeneficiarioNotFoundException(int IDBeneficiario) : base($"Beneficiario {IDBeneficiario} not found")
        {
        }
    }
}