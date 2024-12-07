using System;

namespace Scadenze.Models.Exceptions.Application
{
    public class ScadenzaNotFoundException : Exception
    {
        public ScadenzaNotFoundException(int IDScadenza) : base($"Scadenza {IDScadenza} not found")
        {
        }
    }
}